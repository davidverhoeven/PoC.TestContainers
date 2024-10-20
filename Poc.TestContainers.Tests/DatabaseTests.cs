using System.Data.SqlClient;
using PoC.TestContainers.Library;
using Testcontainers.MsSql;

namespace Poc.TestContainers.Tests;

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

            // Create table
            var createTableCommand = new SqlCommand(@"
                CREATE TABLE Foo (
                    Id INT PRIMARY KEY IDENTITY,
                    Bar NVARCHAR(100) NOT NULL,
                )", connection);
            await createTableCommand.ExecuteNonQueryAsync();
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
        Assert.Equal(1, count);
    }
}