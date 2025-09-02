namespace SampleWeightedSelection;

public record WeightedItem(string Name, double Weight)
{
    public override string ToString()
    {
        return $"{Name}: {Weight}%";
    }
}