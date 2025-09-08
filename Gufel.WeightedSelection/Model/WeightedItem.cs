namespace Gufel.WeightedSelection.Model;

public class WeightedItem(string name, double weight)
{
    private double _used;

    public void Use()
    {
        _used++;
    }

    public bool HasRemainUsage()
    {
        return _used < Weight;
    }

    public string Name { get; } = name;
    public double Weight { get; } = weight;

    public override string ToString()
    {
        return $"{Name}: {Weight}%";
    }
}