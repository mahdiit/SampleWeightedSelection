using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Implement.Algorithm;
using Gufel.WeightedSelection.Model;
using Moq;
using Xunit;

namespace Gufel.WeightedSelection.Tests.Implement.Algorithm;

public class PreComputedWeightedSelectorTests : SelectorTestBase
{
    protected override IWeightedRandomSelect CreateSelector(IWeightedItemList list, IRandomNumber random)
    {
        return new PreComputedWeightedSelector(list, random);
    }

    [Fact]
    public void PreComputedWeightedSelector_BinarySearch_WorksCorrectly()
    {
        // Arrange
        var items = new List<WeightedItem>
        {
            new("A", 10.0), // Cumulative: 10.0
            new("B", 20.0), // Cumulative: 30.0  
            new("C", 30.0)  // Cumulative: 60.0
        };
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();

        // Test different ranges to verify binary search
        mockRandom.SetupSequence(r => r.NextDouble())
            .Returns(0.1)   // Should select A (0.1 * 60 = 6, which is < 10)
            .Returns(0.4)   // Should select B (0.4 * 60 = 24, which is between 10 and 30)
            .Returns(0.8);  // Should select C (0.8 * 60 = 48, which is between 30 and 60)

        var selector = new PreComputedWeightedSelector(list, mockRandom.Object);

        // Act & Assert
        var first = selector.SelectItem();
        Assert.Equal("A", first.Name);

        var second = selector.SelectItem();
        Assert.Equal("B", second.Name);

        var third = selector.SelectItem();
        Assert.Equal("C", third.Name);
    }

    [Fact]
    public void PreComputedWeightedSelector_EdgeCaseRandomValues_HandlesCorrectly()
    {
        // Arrange
        var items = new List<WeightedItem>
        {
            new("Single", 50.0)
        };
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();

        // Test edge cases
        mockRandom.SetupSequence(r => r.NextDouble())
            .Returns(0.0)   // Minimum value
            .Returns(0.999); // Near maximum value

        var selector = new PreComputedWeightedSelector(list, mockRandom.Object);

        // Act & Assert
        var first = selector.SelectItem();
        Assert.Equal("Single", first.Name);

        var second = selector.SelectItem();
        Assert.Equal("Single", second.Name);
    }
}