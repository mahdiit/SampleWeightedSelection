using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Implement.Algorithm;

public class WeightedRandomSelector : WeightedRandomSelectBase
{
    private double _totalWeight;
    private readonly IRandomNumber _random;

    public WeightedRandomSelector(IWeightedItemList list, IRandomNumber random)
    : base(list)
    {
        _random = random;
        WeightedRandomSelector_OnInitList();
    }

    private void WeightedRandomSelector_OnInitList()
    {
        _totalWeight = 0.0;
        foreach (var item in Items)
        {
            _totalWeight += item.Weight;
        }

        if (_totalWeight == 0)
            throw new ArgumentException("Total weight cannot be zero");
    }

    protected override WeightedItem Select()
    {
        var roll = _random.NextDouble(1.0, _totalWeight + 1.0);
        var cumulative = 0.0;

        foreach (var item in Items)
        {
            cumulative += item.Weight;
            if (roll <= cumulative)
                return item;
        }

        return Items[^1];
    }

    protected override void Init()
    {
        WeightedRandomSelector_OnInitList();
    }
}