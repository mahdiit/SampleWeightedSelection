using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Implement.Algorithm;

// Method 2: Pre-computed cumulative weights (more efficient for multiple selections)
public class PreComputedWeightedSelector : IWeightedRandomSelect
{
    private readonly List<WeightedItem> _items;
    private readonly List<double> _cumulativeWeights;
    private readonly IRandomNumber _random;

    public PreComputedWeightedSelector(List<WeightedItem> items, IRandomNumber random)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _random = random;
        _cumulativeWeights = [];

        BuildCumulativeWeights();
    }

    private void BuildCumulativeWeights()
    {
        double cumulative = 0;

        foreach (var item in _items)
        {
            cumulative += item.Weight;
            _cumulativeWeights.Add(cumulative);
        }

        if (cumulative == 0)
            throw new ArgumentException("Total weight cannot be zero");
    }

    public WeightedItem SelectItem()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("No items to select from");

        var randomValue = _random.NextDouble() * _cumulativeWeights.Last();

        // Binary search for better performance with large lists
        var index = BinarySearchCumulative(randomValue);
        return _items[index];
    }

    private int BinarySearchCumulative(double value)
    {
        var left = 0;
        var right = _cumulativeWeights.Count - 1;

        while (left < right)
        {
            var mid = left + (right - left) / 2;
            if (_cumulativeWeights[mid] < value)
                left = mid + 1;
            else
                right = mid;
        }

        return left;
    }
}