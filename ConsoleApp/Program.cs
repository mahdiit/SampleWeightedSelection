using ConsoleApp;
using ConsoleTables;
using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Implement.Algorithm;
using Gufel.WeightedSelection.Implement.Random;
using Gufel.WeightedSelection.Model;
using System.Diagnostics;

Console.WriteLine("Start");


Console.WriteLine("=== Weighted Random Selection Test ===");
Console.WriteLine("Testing with items: A1: 10%, A2: 25%, A3: 0%, A4: 65%");
Console.WriteLine("Running each method exactly 100 times\n");

// Create test items as specified
var items = new List<WeightedItem>
{
    new("A1", 10),
    new("A2", 40),
    new("A3", 25),
    new("A4", 25)
};

const int testRuns = 100;
IRandomNumber random = new FastRandom();

// Test Method 1: Simple Cumulative Selection
Console.WriteLine("=== METHOD 1: Simple Cumulative Selection ===");
TestMethod1(items, testRuns, random);

Console.WriteLine("\n" + new string('=', 60) + "\n");

// Test Method 2: Pre-computed Cumulative Weights
Console.WriteLine("=== METHOD 2: Pre-computed Cumulative Weights ===");
TestMethod2(items, testRuns, random);

Console.WriteLine("\n" + new string('=', 60) + "\n");

// Test Method 3: Alias Method
Console.WriteLine("=== METHOD 3: Alias Method ===");
TestMethod3(items, testRuns, random);

TotalPrintResults();

Console.WriteLine("\n" + new string('=', 60));

Console.WriteLine("Test completed. Press any key to exit...");
Console.ReadKey();


static void TestMethod1(List<WeightedItem> items, int testRuns, IRandomNumber random)
{
    var selector = new WeightedRandomSelector(items, random);
    var results = new Dictionary<string, int>();
    var stopwatch = Stopwatch.StartNew();

    // Perform selections
    for (int i = 0; i < testRuns; i++)
    {
        var selected = selector.SelectItem();
        results[selected.Name] = results.GetValueOrDefault(selected.Name, 0) + 1;
    }

    stopwatch.Stop();

    // Print results
    PrintResults("Simple Cumulative Selection", results, testRuns, stopwatch.ElapsedTicks, items, 0);
}

static void TestMethod2(List<WeightedItem> items, int testRuns, IRandomNumber random)
{
    var stopwatch = Stopwatch.StartNew();
    var selector = new PreComputedWeightedSelector(items, random);
    var setupTime = stopwatch.ElapsedTicks;

    var results = new Dictionary<string, int>();

    // Perform selections
    for (int i = 0; i < testRuns; i++)
    {
        var selected = selector.SelectItem();
        results[selected.Name] = results.GetValueOrDefault(selected.Name, 0) + 1;
    }

    stopwatch.Stop();

    // Print results
    //Console.WriteLine($"Setup time: {setupTime} ticks ({setupTime / 10000.0:F3} ms)");
    PrintResults("Pre-computed Cumulative Weights", results, testRuns, stopwatch.ElapsedTicks - setupTime, items, setupTime);
}

static void TestMethod3(List<WeightedItem> items, int testRuns, IRandomNumber random)
{
    var stopwatch = Stopwatch.StartNew();
    var selector = new AliasMethodSelector(items, random);
    var setupTime = stopwatch.ElapsedTicks;

    var results = new Dictionary<string, int>();

    // Perform selections
    for (int i = 0; i < testRuns; i++)
    {
        var selected = selector.SelectItem();
        results[selected.Name] = results.GetValueOrDefault(selected.Name, 0) + 1;
    }

    stopwatch.Stop();

    // Print results
    //Console.WriteLine($"Setup time: {setupTime} ticks ({setupTime / 10000.0:F3} ms)");
    PrintResults("Alias Method", results, testRuns, stopwatch.ElapsedTicks - setupTime, items, setupTime);
}

static void PrintResults(string methodName, Dictionary<string, int> results, int totalRuns, long elapsedTicks, List<WeightedItem> originalItems, long setupTime)
{
    Console.WriteLine($"Method: {methodName}");

    PrintResult.Data.Add(new PrintResultDto(methodName,
        elapsedTicks,
        elapsedTicks / 10000.0,
        elapsedTicks / (double)totalRuns,
        elapsedTicks / (double)totalRuns / 10000.0, setupTime));

    //Console.WriteLine($"Selection time: {elapsedTicks} ticks ({elapsedTicks / 10000.0:F3} ms)");
    //Console.WriteLine($"Average time per selection: {elapsedTicks / (double)totalRuns:F1} ticks ({elapsedTicks / (double)totalRuns / 10000.0:F6} ms)");
    Console.WriteLine();

    Console.WriteLine("Results:");
    Console.WriteLine("Item\tExpected\tActual\tDifference");
    Console.WriteLine(new string('-', 45));

    foreach (var item in originalItems)
    {
        int actualCount = results.GetValueOrDefault(item.Name, 0);
        double expectedPercentage = item.Weight;
        double actualPercentage = (actualCount / (double)totalRuns) * 100;
        double difference = actualPercentage - expectedPercentage;

        Console.WriteLine($"{item.Name}\t{expectedPercentage:F1}%\t\t{actualCount} ({actualPercentage:F1}%)\t{difference:+F1;-F1}%");
    }

    // Verify total
    int totalSelections = results.Values.Sum();
    Console.WriteLine(new string('-', 45));
    Console.WriteLine($"Total:\t100.0%\t\t{totalSelections} (100.0%)");

    if (totalSelections != totalRuns)
    {
        Console.WriteLine($"WARNING: Total selections ({totalSelections}) doesn't match expected ({totalRuns})!");
    }
}

static void TotalPrintResults()
{
    Console.WriteLine();
    Console.WriteLine("Total result:");
    var table = new ConsoleTable("Name", "Elapsed", "Ticks", "Avg Selection", "Avg Selection Ticks", "Setup", "Setup Ticks");
    foreach (var item in PrintResult.Data)
    {
        table.AddRow(item.Name, item.ElapsedTicks, $"{item.Ticks:F3}", $"{item.AvgSelection:F1}",
            $"{item.AvgSelectionTicks:F6}", item.SetupTime, $"{item.SetupTime / 10000.0:F3}");
    }

    table.Write();
    Console.WriteLine();
}