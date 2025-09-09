using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Abstract;

public interface IWeightedItemList
{
    IReadOnlyCollection<WeightedItem> Items { get; }

    IList<WeightedItem> Clone();
}