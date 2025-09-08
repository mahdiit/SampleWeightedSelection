using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Model;

namespace ConsoleApp;

record PrintResultDto(string Name, long ElapsedTicks, double Ticks, double AvgSelection, double AvgSelectionTicks, long SetupTime)
{

}

internal static class PrintResult
{
    public static List<PrintResultDto> Data = [];
}

internal class ListData(IReadOnlyCollection<WeightedItem> items) : WeightedItemListBase
{
    public override IReadOnlyCollection<WeightedItem> Items => items;
}