using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Implement.Algorithm;

public class PreComputedWeightedSelector : WeightedRandomSelectBase
{
    private List<double> _cumulativeWeights;
    private readonly IRandomNumber _random;

    public PreComputedWeightedSelector(IWeightedItemList list, IRandomNumber random)
    : base(list)
    {
        _random = random;
        PreComputedWeightedSelector_OnInitList();
    }

    private void PreComputedWeightedSelector_OnInitList()
    {
        double cumulative = 0;
        _cumulativeWeights = [];

        foreach (var item in Items)
        {
            cumulative += item.Weight;
            _cumulativeWeights.Add(cumulative);
        }

        if (cumulative == 0)
            throw new ArgumentException("Total weight cannot be zero");
    }

    protected override WeightedItem Select()
    {
        var randomValue = _random.NextDouble() * _cumulativeWeights.Last();

        var index = BinarySearchCumulative(randomValue);
        return Items[index];
    }

    protected override void Init()
    {
        PreComputedWeightedSelector_OnInitList();
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