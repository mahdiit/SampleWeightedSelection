
using SampleWeightedSelection;
using System.Diagnostics;

Console.WriteLine("Start");


Console.WriteLine("=== Weighted Random Selection Test ===");
Console.WriteLine("Testing with items: A1: 10%, A2: 25%, A3: 0%, A4: 65%");
Console.WriteLine("Running each method exactly 100 times\n");

// Create test items as specified
var items = new List<WeightedItem>
{
    new("A1", 10),
    new WeightedItem("A2", 40),
    new WeightedItem("A3", 0),
    new WeightedItem("A4", 0)
};

const int testRuns = 10000;
const int seed = 12345; // Fixed seed for reproducible results

// Test Method 1: Simple Cumulative Selection
Console.WriteLine("=== METHOD 1: Simple Cumulative Selection ===");
TestMethod1(items, testRuns, seed);

Console.WriteLine("\n" + new string('=', 60) + "\n");

// Test Method 2: Pre-computed Cumulative Weights
Console.WriteLine("=== METHOD 2: Pre-computed Cumulative Weights ===");
TestMethod2(items, testRuns, seed);

Console.WriteLine("\n" + new string('=', 60) + "\n");

// Test Method 3: Alias Method
Console.WriteLine("=== METHOD 3: Alias Method ===");
TestMethod3(items, testRuns, seed);

Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine("Test completed. Press any key to exit...");
Console.ReadKey();


static void TestMethod1(List<WeightedItem> items, int testRuns, int seed)
{
    var selector = new WeightedRandomSelector(items, seed);
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
    PrintResults("Simple Cumulative Selection", results, testRuns, stopwatch.ElapsedTicks, items);
}

static void TestMethod2(List<WeightedItem> items, int testRuns, int seed)
{
    var stopwatch = Stopwatch.StartNew();
    var selector = new PreComputedWeightedSelector(items, seed);
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
    Console.WriteLine($"Setup time: {setupTime} ticks ({setupTime / 10000.0:F3} ms)");
    PrintResults("Pre-computed Cumulative Weights", results, testRuns, stopwatch.ElapsedTicks - setupTime, items);
}

static void TestMethod3(List<WeightedItem> items, int testRuns, int seed)
{
    var stopwatch = Stopwatch.StartNew();
    var selector = new AliasMethodSelector(items, seed);
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
    Console.WriteLine($"Setup time: {setupTime} ticks ({setupTime / 10000.0:F3} ms)");
    PrintResults("Alias Method", results, testRuns, stopwatch.ElapsedTicks - setupTime, items);
}

static void PrintResults(string methodName, Dictionary<string, int> results, int totalRuns, long elapsedTicks, List<WeightedItem> originalItems)
{
    Console.WriteLine($"Method: {methodName}");
    Console.WriteLine($"Selection time: {elapsedTicks} ticks ({elapsedTicks / 10000.0:F3} ms)");
    Console.WriteLine($"Average time per selection: {elapsedTicks / (double)totalRuns:F1} ticks ({elapsedTicks / (double)totalRuns / 10000.0:F6} ms)");
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