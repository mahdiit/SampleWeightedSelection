using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Implement.Algorithm;


// Method 1: Simple cumulative weight approach
public class WeightedRandomSelector : IWeightedRandomSelect
{
    private readonly List<WeightedItem> _originItems;
    private List<WeightedItem> _items;
    private readonly IRandomNumber _random;

    public WeightedRandomSelector(List<WeightedItem> items, IRandomNumber random)
    {
        _originItems = items.Select(x => new WeightedItem(x.Name, x.Weight)).ToList();
        _items = [];
        CopyFromOrigin();

        _random = random;
    }

    private void CopyFromOrigin()
    {
        _items = _originItems.Select(x => new WeightedItem(x.Name, x.Weight)).ToList();
    }

    public WeightedItem SelectItem()
    {
        if (_items == null || _items.Count == 0)
            throw new ArgumentException("Items list cannot be null or empty");

        var totalWeight = _items.Sum(item => item.Weight);
        if (totalWeight == 0)
            throw new ArgumentException("Total weight cannot be zero");

        var randomValue = _random.NextDouble() * totalWeight;
        double cumulativeWeight = 0;

        foreach (var item in _items)
        {
            cumulativeWeight += item.Weight;
            if (randomValue <= cumulativeWeight)
            {
                item.Use();

                if (item.HasAny()) return item;

                _items.Remove(item);

                if (!_items.Any())
                    CopyFromOrigin();

                return item;
            }
        }

        // Fallback (should not reach here if weights sum to totalWeight)
        return _items.Last();
    }
}