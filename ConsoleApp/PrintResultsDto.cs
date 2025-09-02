namespace ConsoleApp;

record PrintResultDto(string Name, long ElapsedTicks, double Ticks, double AvgSelection, double AvgSelectionTicks, long SetupTime)
{

}

internal static class PrintResult
{
    public static List<PrintResultDto> Data = [];
}