using System.Collections.ObjectModel;
using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;
using Xunit;

namespace Gufel.WeightedSelection.Tests.Abstract;

public class WeightedItemListTests
{
    private class TestWeightedItemList(IReadOnlyCollection<WeightedItem> items) : WeightedItemListBase
    {
        public override IReadOnlyCollection<WeightedItem> Items => items;
    }

    private class TestRandomSelect(IWeightedItemList list)
        : WeightedRandomSelectBase(list)
    {
        private readonly IWeightedItemList _list = list;

        protected override WeightedItem Select()
        {
            return _list.Items.First();
        }

        protected override void Init()
        {

        }
    }

    [Fact]
    public void WeightedItemListBase_Clone_CreatesIndependentCopy()
    {
        // Arrange
        var originalItems = new List<WeightedItem>
        {
            new("Item1", 3),
            new("Item2", 5),
            new("Item3", 7)
        };
        var list = new TestWeightedItemList(originalItems);

        // Act
        var clonedItems = list.Clone();

        // Assert
        Assert.Equal(originalItems.Count, clonedItems.Count);

        // Verify items are independent copies
        for (int i = 0; i < originalItems.Count; i++)
        {
            Assert.Equal(originalItems[i].Name, clonedItems[i].Name);
            Assert.Equal(originalItems[i].Weight, clonedItems[i].Weight);
            Assert.True(clonedItems[i].HasRemainUsage()); // Cloned items should be fresh
        }

        // Use original items to verify independence
        originalItems[0].Use();
        originalItems[0].Use();
        originalItems[0].Use(); // Should exhaust usage

        Assert.False(originalItems[0].HasRemainUsage());
        Assert.True(clonedItems[0].HasRemainUsage()); // Clone should be unaffected
    }

    [Fact]
    public void WeightedItemListBase_CloneEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var emptyItems = new List<WeightedItem>();
        var list = new TestWeightedItemList(emptyItems);

        // Act
        var clonedItems = list.Clone();

        // Assert
        Assert.Empty(clonedItems);
    }

    [Fact]
    public void WeightedItemListBase_CloneSingleItem_WorksCorrectly()
    {
        // Arrange
        var singleItem = new List<WeightedItem> { new("Single", 5.0) };
        var list = new TestWeightedItemList(singleItem);

        // Act
        var clonedItems = list.Clone();

        // Assert
        Assert.Single(clonedItems);
        Assert.Equal("Single", clonedItems[0].Name);
        Assert.Equal(5.0, clonedItems[0].Weight);
        Assert.True(clonedItems[0].HasRemainUsage());
    }

    [Fact]
    public void Selector_WithEmptyList_ThrowsException()
    {
        // Arrange
        var items = new ReadOnlyCollection<WeightedItem>([]);
        var list = new TestWeightedItemList(items);
        var rnd = new TestRandomSelect(list);

        // Act & Assert - Different selectors might throw at different times
        try
        {
            Assert.Throws<InvalidOperationException>(() => rnd.SelectItem());
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