using Gufel.WeightedSelection.Implement.Random;
using Xunit;

namespace Gufel.WeightedSelection.Tests.Implement.Random;

public class FastRandomTests
{
    [Fact]
    public void FastRandom_NextDouble_ReturnsValueBetweenZeroAndOne()
    {
        // Arrange
        var random = new FastRandom();

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var value = random.NextDouble();
            Assert.True(value >= 0.0 && value < 1.0, $"Value {value} is not in range [0.0, 1.0)");
        }
    }

    [Fact]
    public void FastRandom_NextDoubleWithRange_ReturnsValueInRange()
    {
        // Arrange
        var random = new FastRandom();
        const double min = 5.0;
        const double max = 10.0;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var value = random.NextDouble(min, max);
            Assert.True(value >= min && value < max, $"Value {value} is not in range [{min}, {max})");
        }
    }

    [Fact]
    public void FastRandom_NextDoubleWithInvalidRange_ThrowsException()
    {
        // Arrange
        var random = new FastRandom();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => random.NextDouble(10.0, 5.0));
        Assert.Throws<ArgumentException>(() => random.NextDouble(5.0, 5.0));
    }

    [Fact]
    public void FastRandom_NextInt_ReturnsValueInRange()
    {
        // Arrange
        var random = new FastRandom();
        const int min = 1;
        const int max = 10;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var value = random.NextInt(min, max);
            Assert.True(value >= min && value < max, $"Value {value} is not in range [{min}, {max})");
        }
    }

    [Fact]
    public void FastRandom_NextIntWithMaxOnly_ReturnsValueInRange()
    {
        // Arrange
        var random = new FastRandom();
        const int max = 10;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var value = random.NextInt(max);
            Assert.True(value >= 0 && value < max, $"Value {value} is not in range [0, {max})");
        }
    }
}