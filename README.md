# Gufel.WeightedSelection

[![CI](https://github.com/mahdiit/SampleWeightedSelection/actions/workflows/main.yml/badge.svg?branch=master)](https://github.com/mahdiit/SampleWeightedSelection/actions/workflows/main.yml) [![codecov](https://codecov.io/gh/mahdiit/SampleWeightedSelection/branch/master/graph/badge.svg)](https://codecov.io/gh/mahdiit/SampleWeightedSelection)



A small .NET library for weighted random selection with multiple algorithms and pluggable RNGs.

- **Target framework**: .NET 9.0
- **Namespaces**: `Gufel.WeightedSelection.*`

## Core concepts

- **WeightedItem**: simple model with `Name` and `Weight`.
- **IWeightedItemList / WeightedItemListBase**: abstraction over a readonly source list plus a `Clone()` to create a working copy.
- **IRandomNumber**: RNG abstraction with two implementations:
  - `FastRandom` (wrapper around `System.Random`)
  - `SecureRandom` (cryptographically strong, uses `RandomNumberGenerator`)
- **IWeightedRandomSelect / WeightedRandomSelectBase**: selection abstraction and base class. The base class:
  - Works on a cloned list of items
  - Calls `Use()` on the selected item each time
  - Removes an item from the working set once `used >= Weight`
  - Rebuilds the working set (clone + re-init) when all items are consumed

Note: In this design an item's numeric `Weight` acts both as a relative selection weight and as a per-cycle usage budget. When the sum of weights is exhausted, the selector resets. Keep that in mind if you do not want cyclical consumption semantics.

## Algorithms

- **WeightedRandomSelector** (simple cumulative)
  - Setup: O(1)
  - Select: O(n)
  - Good baseline and easy to understand

- **PreComputedWeightedSelector** (prefix sums + binary search)
  - Setup: O(n)
  - Select: O(log n)
  - Suitable when the set is relatively static during a cycle

- **AliasMethodSelector** (Vose alias method)
  - Setup: O(n)
  - Select: O(1)
  - Best for very frequent selections on a fixed set during a cycle

All algorithms require total weight > 0 and will throw if total weight is zero (some explicitly).

## Quick start

```csharp
using Gufel.WeightedSelection.Abstract;
using Gufel.WeightedSelection.Implement.Algorithm;
using Gufel.WeightedSelection.Implement.Random;
using Gufel.WeightedSelection.Model;

// 1) Define your items (weights are relative; need not sum to 100)
var items = new List<WeightedItem>
{
    new("A1", 10),
    new("A2", 40),
    new("A3", 25),
    new("A4", 25)
};

// 2) Implement a list wrapper (or reuse an existing one in your app)
internal class MyList(IReadOnlyCollection<WeightedItem> src) : WeightedItemListBase
{
    public override IReadOnlyCollection<WeightedItem> Items => src;
}

var list = new MyList(items);

// 3) Choose an RNG
IRandomNumber rng = new FastRandom(); // or new SecureRandom();

// 4) Choose an algorithm
IWeightedRandomSelect selector = new AliasMethodSelector(list, rng);
// Alternatives:
// IWeightedRandomSelect selector = new PreComputedWeightedSelector(list, rng);
// IWeightedRandomSelect selector = new WeightedRandomSelector(list, rng);

// 5) Select items
for (int i = 0; i < 10; i++)
{
    var chosen = selector.SelectItem();
    Console.WriteLine($"Picked: {chosen.Name}");
}
```

## Notes and gotchas

- **Total weight must be > 0**; zero total weight is invalid.
- **Zero-weight items** will never be selected.
- **Cycle behavior**: each selection increases an internal "used" counter; an item is temporarily removed for the rest of the cycle once `used >= Weight`. When every item in the working set is consumed, the selector resets (clones the original list and re-initializes the chosen algorithm tables).
- If you need different lifecycle semantics, adjust `WeightedRandomSelectBase` to skip consumption or to use a different rule.

## Referencing the library

- As a project reference within the same solution:
  1. Right-click your app project > Add > Project Reference…
  2. Select `Gufel.WeightedSelection`.
- Or via `dotnet` CLI from your app project directory:
  ```bash
  dotnet add reference ../Gufel.WeightedSelection/Gufel.WeightedSelection.csproj
  ```

## License

See `LICENSE.txt` at the repository root.
