namespace Gufel.WeightedSelection.Abstract;

public interface IRandomNumber
{
    double NextDouble();

    double NextDouble(double min, double max);

    int NextInt(int min, int max);

    int NextInt(int max);
}