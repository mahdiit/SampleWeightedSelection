using System.Security.Cryptography;
using Gufel.WeightedSelection.Abstract;

namespace Gufel.WeightedSelection.Implement.Random;

public class SecureRandom : IRandomNumber
{
    /// <summary>
    /// Returns a cryptographically secure double in [0.0, 1.0).
    /// </summary>
    public double NextDouble()
    {
        var bytes = NextBytes(8);

        var value = BitConverter.ToUInt64(bytes, 0);
        return value / (ulong.MaxValue + 1.0);
    }

    /// <summary>
    /// Returns a cryptographically secure double in [min, max).
    /// </summary>
    public double NextDouble(double min, double max)
    {
        if (min >= max)
            throw new ArgumentException("min must be less than max");

        return min + (max - min) * NextDouble();
    }

    /// <summary>
    /// Returns a cryptographically secure int in [min, max).
    /// </summary>
    public int NextInt(int min, int max)
    {
        return RandomNumberGenerator.GetInt32(min, max);
    }

    public int NextInt(int max)
    {
        return RandomNumberGenerator.GetInt32(max);
    }

    /// <summary>
    /// Returns a cryptographically secure random byte array.
    /// </summary>
    private static byte[] NextBytes(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}
