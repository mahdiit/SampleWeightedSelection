namespace SampleWeightedSelection;


// Method 1: Simple cumulative weight approach
public class WeightedRandomSelector(List<WeightedItem> items, int seed) : IWeightedItemSelect
{
    private readonly Random _random = new(seed);

    public WeightedItem SelectItem()
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Items list cannot be null or empty");

        var totalWeight = items.Sum(item => item.Weight);
        if (totalWeight == 0)
            throw new ArgumentException("Total weight cannot be zero");

        var randomValue = _random.NextDouble() * totalWeight;
        double cumulativeWeight = 0;

        foreach (var item in items)
        {
            cumulativeWeight += item.Weight;
            if (randomValue <= cumulativeWeight)
                return item;
        }

        // Fallback (should not reach here if weights sum to totalWeight)
        return items.Last();
    }
}