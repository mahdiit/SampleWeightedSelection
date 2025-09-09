using Gufel.WeightedSelection.Model;
using Xunit;

namespace Gufel.WeightedSelection.Tests.Model;

public class WeightedItemTests
{
    [Fact]
    public void WeightedItem_BasicUsage_WorksCorrectly()
    {
        // Arrange
        var item = new WeightedItem("Test Item", 3.0);

        // Act & Assert
        Assert.Equal("Test Item", item.Name);
        Assert.Equal(3.0, item.Weight);
        Assert.True(item.HasRemainUsage());

        // Use the item once
        item.Use();
        Assert.True(item.HasRemainUsage());

        // Use the item second time
        item.Use();
        Assert.True(item.HasRemainUsage());

        // Use the item third time (should reach weight limit)
        item.Use();
        Assert.False(item.HasRemainUsage());
    }

    [Fact]
    public void WeightedItem_ZeroWeight_NeverHasRemainUsage()
    {
        // Arrange
        var item = new WeightedItem("Zero Weight", 0.0);

        // Act & Assert
        Assert.False(item.HasRemainUsage());

        // Even after using, still no remain usage
        item.Use();
        Assert.False(item.HasRemainUsage());
    }

    [Fact]
    public void WeightedItem_FractionalWeight_HandlesCorrectly()
    {
        // Arrange
        var item = new WeightedItem("Fractional", 1.5);

        // Act & Assert
        Assert.True(item.HasRemainUsage());

        item.Use(); // Used: 1.0, Weight: 1.5
        Assert.True(item.HasRemainUsage());

        item.Use(); // Used: 2.0, Weight: 1.5
        Assert.False(item.HasRemainUsage());
    }

    [Fact]
    public void WeightedItem_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var item = new WeightedItem("Sample", 25.5);

        // Act
        var result = item.ToString();

        // Assert
        Assert.Equal("Sample: 25.5", result);
    }

    [Fact]
    public void WeightedItem_WhenDifferentAddedValue_HandlesCorrectly()
    {
        // Arrange
        var item = new WeightedItem("Sample", 1.5);

        //Act
        for (var i = 1; i <= 3; i++)
        {
            item.Use(0.5);
        }

        //Assert
        Assert.False(item.HasRemainUsage());
    }
}