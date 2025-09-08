using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Abstract;

public interface IWeightedItemList
{
    IReadOnlyCollection<WeightedItem> Items { get; }

    IList<WeightedItem> Clone();
}

public abstract class WeightedItemListBase : IWeightedItemList
{
    public abstract IReadOnlyCollection<WeightedItem> Items { get; }

    public IList<WeightedItem> Clone()
    {
        return Items.Select(x => new WeightedItem(x.Name, x.Weight)).ToList();
    }
}