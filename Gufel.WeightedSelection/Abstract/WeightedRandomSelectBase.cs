using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Abstract;

public abstract class WeightedRandomSelectBase(IWeightedItemList list) : IWeightedRandomSelect
{
    protected IList<WeightedItem> Items = list.Clone();

    protected abstract WeightedItem Select();

    protected abstract void Init();

    public WeightedItem SelectItem()
    {
        if (Items == null || !Items.Any())
            throw new InvalidOperationException("No items to select from");

        var selected = Select();

        selected.Use();

        if (selected.HasRemainUsage()) return selected;

        Items.Remove(selected);

        if (!Items.Any())
            Items = list.Clone();

        Init();

        return selected;
    }
}