using Gufel.WeightedSelection.Model;

namespace Gufel.WeightedSelection.Abstract;

public interface IWeightedRandomSelect
{
    WeightedItem SelectItem();
}