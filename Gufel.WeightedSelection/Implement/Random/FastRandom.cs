using System.Security.Cryptography;
using Gufel.WeightedSelection.Abstract;
using static System.Int32;

namespace Gufel.WeightedSelection.Implement.Random;

public class FastRandom : IRandomNumber
{
    private readonly System.Random _random = new(RandomNumberGenerator.GetInt32(MaxValue));

    public double NextDouble()
    {
        return _random.NextDouble();
    }

    public double NextDouble(double min, double max)
    {
        if (min >= max)
            throw new ArgumentException("min must be less than max");

        return min + (max - min) * NextDouble();
    }

    public int NextInt(int min, int max)
    {
        return _random.Next(min, max);
    }

    public int NextInt(int max)
    {
        return _random.Next(max);
    }
}