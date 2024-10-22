using System.Data.SqlClient;
using PoC.TestContainers.Library;
using Testcontainers.MsSql;

namespace Poc.TestContainers.Tests;

/// <summary>
/// This test will create a new database container for each test.
/// </summary>
public class DatabaseTests : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;

    public DatabaseTests()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithPassword("yourStrong(!)Password")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        await InitializeDatabaseAsync();
    }


    private async Task InitializeDatabaseAsync()
    {
        var connectionString = _msSqlContainer.GetConnectionString();

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var createTableCommand = new SqlCommand(@"
                CREATE TABLE Foo (
                    Id INT PRIMARY KEY IDENTITY,
                    Bar NVARCHAR(100) NOT NULL,
                )", connection);
            await createTableCommand.ExecuteNonQueryAsync();

 
            var insertCommand = new SqlCommand(@"
                INSERT INTO Foo (Bar) VALUES ('dummydata')", connection);

            await insertCommand.ExecuteNonQueryAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    public async Task AddFooRecord()
    {
        var fooRepository = new FooRepository(_msSqlContainer.GetConnectionString());
        var foo = new Foo { Bar = "Test" };

        await fooRepository.AddAsync(foo);

        var count = await fooRepository.CountAsync();
        Assert.Equal(2, count);
    }


    [Fact]
    public async Task DeleteFooRecord()
    {
        var fooRepository = new FooRepository(_msSqlContainer.GetConnectionString());

        await fooRepository.DeleteAsync(1);

        var count = await fooRepository.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task UpdateFooRecord()
    {
        var fooRepository = new FooRepository(_msSqlContainer.GetConnectionString());
        var foo = new Foo { Id = 1, Bar = "Updated" };

        await fooRepository.UpdateAsync(foo);

        var afterUpdate = await fooRepository.GetByIdAsync(1);
        Assert.Equivalent(foo, afterUpdate);
    }


}