namespace SampleWeightedSelection;

// Method 3: Alias Method (most efficient for very frequent selections)
public class AliasMethodSelector : IWeightedItemSelect
{
    private readonly int[] _alias;
    private readonly double[] _prob;
    private readonly Random _random;
    private readonly List<WeightedItem> _items;

    public AliasMethodSelector(List<WeightedItem> items, int? seed = null)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _random = seed.HasValue ? new Random(seed.Value) : new Random();

        var n = _items.Count;
        _alias = new int[n];
        _prob = new double[n];

        BuildAliasTable();
    }

    private void BuildAliasTable()
    {
        var n = _items.Count;

        // Normalize weights
        var totalWeight = _items.Sum(item => item.Weight);
        if (totalWeight == 0)
            throw new ArgumentException("Total weight cannot be zero");

        var normalizedWeights = _items.Select(item => item.Weight * n / totalWeight).ToArray();

        var small = new Queue<int>();
        var large = new Queue<int>();

        // Classify probabilities
        for (var i = 0; i < n; i++)
        {
            if (normalizedWeights[i] < 1.0)
                small.Enqueue(i);
            else
                large.Enqueue(i);
        }

        // Build alias table
        while (small.Count > 0 && large.Count > 0)
        {
            var smallIndex = small.Dequeue();
            var largeIndex = large.Dequeue();

            _prob[smallIndex] = normalizedWeights[smallIndex];
            _alias[smallIndex] = largeIndex;

            normalizedWeights[largeIndex] -= (1.0 - normalizedWeights[smallIndex]);

            if (normalizedWeights[largeIndex] < 1.0)
                small.Enqueue(largeIndex);
            else
                large.Enqueue(largeIndex);
        }

        while (large.Count > 0)
        {
            var largeIndex = large.Dequeue();
            _prob[largeIndex] = 1.0;
        }

        while (small.Count > 0)
        {
            var smallIndex = small.Dequeue();
            _prob[smallIndex] = 1.0;
        }
    }

    public WeightedItem SelectItem()
    {
        var n = _items.Count;
        var i = _random.Next(n);

        return _random.NextDouble() < _prob[i] ? _items[i] : _items[_alias[i]];
    }
}