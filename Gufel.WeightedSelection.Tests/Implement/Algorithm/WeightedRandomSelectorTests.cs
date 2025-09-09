using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Implement.Algorithm;
using Gufel.WeightedSelection.Model;
using Xunit;

namespace Gufel.WeightedSelection.Tests.Implement.Algorithm;

public class WeightedRandomSelectorTests : SelectorTestBase
{
    protected override IWeightedRandomSelect CreateSelector(IWeightedItemList list, IRandomNumber random)
    {
        return new WeightedRandomSelector(list, random);
    }

    [Fact]
    public void WeightedRandomSelector_SpecificImplementation_WorksCorrectly()
    {
        // This test is specific to WeightedRandomSelector implementation details
        // Additional tests specific to this algorithm can be added here

        // Arrange
        var items = new List<WeightedItem>
        {
            new("First", 25.0),
            new("Second", 75.0)
        };
        var list = new TestWeightedItemList(items);

        // Create a predictable random that should select the second item
        var mockRandom = new Moq.Mock<IRandomNumber>();
        mockRandom.Setup(r => r.NextDouble(1.0, 101.0)).Returns(50.0); // Should hit second item

        var selector = new WeightedRandomSelector(list, mockRandom.Object);

        // Act
        var selected = selector.SelectItem();

        // Assert
        Assert.Equal("Second", selected.Name);
    }
}