using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Implement.Algorithm;

public class AliasMethodSelector : WeightedRandomSelectBase
{
    private int[] _alias;
    private double[] _prob;
    private readonly IRandomNumber _random;

    public AliasMethodSelector(IWeightedItemList list, IRandomNumber random)
    : base(list)
    {
        _random = random;
        InternalInit();
    }

    protected override WeightedItem Select()
    {
        var n = Items.Count;
        var i = _random.NextInt(n);

        return _random.NextDouble() < _prob[i] ? Items[i] : Items[_alias[i]];
    }

    protected override void Init()
    {
        InternalInit();
    }

    private void InternalInit()
    {
        var n = Items.Count;
        _alias = new int[n];
        _prob = new double[n];

        // Normalize weights
        var totalWeight = Items.Sum(item => item.Weight);
        if (totalWeight == 0)
            throw new ArgumentException("Total weight cannot be zero");

        var normalizedWeights = Items.Select(item => item.Weight * n / totalWeight).ToArray();

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

            normalizedWeights[largeIndex] -= 1.0 - normalizedWeights[smallIndex];

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
}