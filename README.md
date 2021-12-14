# MongoAsyncEnumerableAdapter
[![NuGet](https://img.shields.io/nuget/dt/MongoAsyncEnumerableAdapter.svg)](https://www.nuget.org/packages/MongoAsyncEnumerableAdapter)
[![NuGet](https://img.shields.io/nuget/v/MongoAsyncEnumerableAdapter.svg)](https://www.nuget.org/packages/MongoAsyncEnumerableAdapter)
[![license](https://img.shields.io/github/license/i3arnon/MongoAsyncEnumerableAdapter.svg)](LICENSE)

Provides an adapter from MongoDB's `IAsyncCursor<TDocument>` and `IAsyncCursorSource<TDocument>` to `IAsyncEnumerable<T>`

This allows plugging MongoDB's custom async iterators into the async LINQ ecosystem by wrapping `IAsyncCursorSource<TDocument>` or `IAsyncCursor<TDocument>` in an `IAsyncEnumerable<T>` implementation.

For example, iterating on a result with `await foreach`:

```csharp
IMongoCollection<Hamster> collection = // ...
IFindFluent<Hamster, Hamster> findFluent = collection.Find(hamster => hamster.Name == "bar");

await foreach (var hamster in findFluent.ToAsyncEnumerable())
{
    Console.WriteLine(hamster.Age);
}
```

Or any other async LINQ operator:

```csharp
IMongoCollection<Hamster> collection = // ...

int count = 
    await collection.Find(_ => true).
        ToAsyncEnumerable().
        GroupBy(hamster => hamster.Age).
        CountAsync();
```