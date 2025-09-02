using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Implement.Algorithm;

public class WeightedRandomSelectorChatGpt : IWeightedRandomSelect
{
    private readonly List<WeightedItem> _items;
    private readonly double _totalWeight;
    private readonly IRandomNumber _random;

    public WeightedRandomSelectorChatGpt(List<WeightedItem> items, IRandomNumber random)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Items list cannot be null or empty.");

        _items = items;
        _random = random;

        // Precompute total weight
        _totalWeight = 0.0;
        foreach (var item in items)
        {
            if (item.Weight <= 0)
                throw new ArgumentException("Item weight must be positive.");

            _totalWeight += item.Weight;
        }
    }

    public WeightedItem SelectItem()
    {
        var roll = _random.NextDouble(1.0, _totalWeight + 1.0);
        var cumulative = 0.0;

        foreach (var item in _items)
        {
            cumulative += item.Weight;
            if (roll <= cumulative)
                return item;
        }

        // Fallback (should never hit this due to logic)
        return _items[^1];
    }
}