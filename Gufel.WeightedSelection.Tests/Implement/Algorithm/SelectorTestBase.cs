using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;
using Moq;
using Xunit;

namespace Gufel.WeightedSelection.Tests.Implement.Algorithm;

public abstract class SelectorTestBase
{
    protected class TestWeightedItemList(IReadOnlyCollection<WeightedItem> items) : WeightedItemListBase
    {
        public override IReadOnlyCollection<WeightedItem> Items => items;
    }

    protected abstract IWeightedRandomSelect CreateSelector(IWeightedItemList list, IRandomNumber random);

    [Fact]
    public void Selector_WithValidItems_SelectsItems()
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
        mockRandom.Setup(r => r.NextDouble()).Returns(0.5);
        mockRandom.Setup(r => r.NextDouble(It.IsAny<double>(), It.IsAny<double>())).Returns(0.5);
        mockRandom.Setup(r => r.NextInt(It.IsAny<int>())).Returns(1);

        var selector = CreateSelector(list, mockRandom.Object);

        // Act
        var selected = selector.SelectItem();

        // Assert
        Assert.NotNull(selected);
        Assert.Contains(selected.Name, new[] { "A", "B", "C" });
    }

    [Fact]
    public void Selector_ItemConsumptionCycle_WorksCorrectly()
    {
        // Arrange
        var items = new List<WeightedItem>
        {
            new("A", 2.0), // Can be selected twice
            new("B", 1.0)  // Can be selected once
        };
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();
        
        // Setup to always select the first item
        mockRandom.Setup(r => r.NextDouble()).Returns(0.1);
        mockRandom.Setup(r => r.NextDouble(It.IsAny<double>(), It.IsAny<double>())).Returns(1.5);
        mockRandom.Setup(r => r.NextInt(It.IsAny<int>())).Returns(0);

        var selector = CreateSelector(list, mockRandom.Object);

        // Act & Assert
        // First selection - should get item A
        var first = selector.SelectItem();
        Assert.Equal("A", first.Name);

        // Second selection - should get item A again (still has usage)
        var second = selector.SelectItem();
        Assert.Equal("A", second.Name);

        // Third selection - A is exhausted, should get B or reset
        var third = selector.SelectItem();
        Assert.NotNull(third);
    }

    [Fact]
    public void Selector_WithZeroTotalWeight_ThrowsException()
    {
        // Arrange
        var items = new List<WeightedItem>
        {
            new("Zero1", 0.0),
            new("Zero2", 0.0)
        };
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();
        mockRandom.Setup(r => r.NextDouble()).Returns(0.5);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateSelector(list, mockRandom.Object));
    }

    [Fact]
    public void Selector_WithEmptyList_ThrowsException()
    {
        // Arrange
        var items = new List<WeightedItem>();
        var list = new TestWeightedItemList(items);
        var mockRandom = new Mock<IRandomNumber>();

        // Act & Assert - Different selectors might throw at different times
        try
        {
            var selector = CreateSelector(list, mockRandom.Object);
            Assert.Throws<InvalidOperationException>(() => selector.SelectItem());
        }
        catch (ArgumentException)
        {
            // Some selectors might throw during construction, which is also valid
        }
        catch (InvalidOperationException)
        {
            // Some selectors might throw during construction, which is also valid
        }
    }
}