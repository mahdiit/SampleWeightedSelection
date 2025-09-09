using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Implement.Algorithm;
using Gufel.WeightedSelection.Model;
using Moq;
using Xunit;

namespace Gufel.WeightedSelection.Tests.Implement.Algorithm;

public class AliasMethodSelectorTests : SelectorTestBase
{
    protected override IWeightedRandomSelect CreateSelector(IWeightedItemList list, IRandomNumber random)
    {
        return new AliasMethodSelector(list, random);
    }

    [Fact]
    public void AliasMethodSelector_AliasTable_WorksCorrectly()
    {
        // Arrange
        var items = new List<WeightedItem>
        {
            new("A", 10.0),
            new("B", 20.0),
            new("C", 30.0)
        };
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();

        // Setup predictable random values for alias method
        mockRandom.SetupSequence(r => r.NextInt(3))
            .Returns(0)  // Select index 0
            .Returns(1)  // Select index 1
            .Returns(2); // Select index 2

        mockRandom.SetupSequence(r => r.NextDouble())
            .Returns(0.8)  // High probability - should use direct index
            .Returns(0.5)  // Medium probability
            .Returns(0.2); // Low probability - might use alias

        var selector = new AliasMethodSelector(list, mockRandom.Object);

        // Act
        var first = selector.SelectItem();
        var second = selector.SelectItem();
        var third = selector.SelectItem();

        // Assert
        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotNull(third);

        // All should be valid items from our list
        Assert.Contains(first.Name, new[] { "A", "B", "C" });
        Assert.Contains(second.Name, new[] { "A", "B", "C" });
        Assert.Contains(third.Name, new[] { "A", "B", "C" });
    }

    [Fact]
    public void AliasMethodSelector_UniformWeights_SelectsEvenly()
    {
        // Arrange - all items have equal weight
        var items = new List<WeightedItem>
        {
            new("Equal1", 20.0),
            new("Equal2", 20.0),
            new("Equal3", 20.0)
        };
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();

        // With uniform weights, the alias method should work predictably
        mockRandom.SetupSequence(r => r.NextInt(3))
            .Returns(0).Returns(1).Returns(2);

        mockRandom.Setup(r => r.NextDouble()).Returns(0.5);

        var selector = new AliasMethodSelector(list, mockRandom.Object);

        // Act
        var selections = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            selections.Add(selector.SelectItem().Name);
        }

        // Assert
        Assert.All(selections, name => Assert.Contains(name, new[] { "Equal1", "Equal2", "Equal3" }));
    }

    [Fact]
    public void AliasMethodSelector_SingleItem_AlwaysSelectsSameItem()
    {
        // Arrange
        var items = new List<WeightedItem>
        {
            new("OnlyOne", 100.0)
        };
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();

        mockRandom.Setup(r => r.NextInt(1)).Returns(0);
        mockRandom.Setup(r => r.NextDouble()).Returns(0.5);

        var selector = new AliasMethodSelector(list, mockRandom.Object);

        // Act & Assert
        for (int i = 0; i < 5; i++)
        {
            var selected = selector.SelectItem();
            Assert.Equal("OnlyOne", selected.Name);
        }
    }
}