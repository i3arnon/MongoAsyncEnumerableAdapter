using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MongoAsyncEnumerableAdapter;
using MongoDB.Driver;
using Xunit;

namespace MongoAsyncEnumerableAdapterTests;

public class AsyncEnumerableTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _database;

    public AsyncEnumerableTests(DatabaseFixture database)
    {
        _database = database;
    }

    /// <summary>
    /// Shows that only the intended record gets pulled from the DB and is iterated
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AsQueryable_WhenPropertyDoesntExist_CountOnlyIteratesOverTheIntendedRecord()
    {
        // Setup
        var count = 0;
        var dataset = await InitializeDataSet(nameof(AsQueryable_WhenPropertyDoesntExist_CountOnlyIteratesOverTheIntendedRecord));
        var sut = dataset.AsQueryable();

        // Sut
        count = sut.Count(model => model.TestProperty == "123");

        // Assertions
        count.Should().Be(1);
    }

    /// <summary>
    /// Shows that every collection gets iterated
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ToAsyncEnumerable_WhenPropertyDoesntExist_ThrowsSystemFormatException()
    {
        // Setup
        var collection= await InitializeDataSet(nameof(ToAsyncEnumerable_WhenPropertyDoesntExist_ThrowsSystemFormatException));
        var sut = collection.AsQueryable().ToAsyncEnumerable();

        // Sut/Assertion
        await Assert.ThrowsAsync<System.FormatException>(async () => await sut.CountAsync(model => model.TestProperty == "123"));
    }

    private async Task<IMongoCollection<TestModel>> InitializeDataSet(string name)
    {
        var mongoCollection = _database.GetCollection<dynamic>(name);
        await mongoCollection.InsertManyAsync(new[] { new { TestProperty = "123" } });
        await mongoCollection.InsertManyAsync(new[] { new { PropertyDoesntExist = "321" } });
        var dataset = _database.GetCollection<TestModel>(name);
        return dataset;
    }
}