using PoC.TestContainers.Library;
using Testcontainers.MsSql;

namespace Poc.TestContainers.Tests;

/// <summary>
/// This test will share the same database container for all tests in this class.
/// </summary>
/// <param name="fixture"></param>
public class SharedDatabaseTest(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task AddFooRecord()
    {
        var fooRepository = new FooRepository(fixture.MsSqlContainer.GetConnectionString());
        var foo = new Foo { Bar = "Test" };

        await fooRepository.AddAsync(foo);

        var recordAfterInsert = await fooRepository.GetByIdAsync(2);
        Assert.Equal(foo.Bar, recordAfterInsert.Bar);
    }


    [Fact]
    public async Task DeleteFooRecord()
    {
        var fooRepository = new FooRepository(fixture.MsSqlContainer.GetConnectionString());

        await fooRepository.DeleteAsync(1);

        var recordAfterDelete = await fooRepository.GetByIdAsync(1);
        Assert.Null(recordAfterDelete);
    }

    [Fact]
    public async Task UpdateFooRecord()
    {
        var fooRepository = new FooRepository(fixture.MsSqlContainer.GetConnectionString());
        var foo = new Foo { Id = 1, Bar = "Updated" };

        await fooRepository.UpdateAsync(foo);

        var afterUpdate = await fooRepository.GetByIdAsync(1);
        Assert.Equivalent(foo, afterUpdate);
    }
}