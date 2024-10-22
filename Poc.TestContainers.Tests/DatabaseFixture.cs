using System.Data.SqlClient;
using Testcontainers.MsSql;

namespace Poc.TestContainers.Tests;

public class DatabaseFixture : IAsyncLifetime
{
    public MsSqlContainer MsSqlContainer { get; }

    public DatabaseFixture()
    {
        MsSqlContainer = new MsSqlBuilder()
            .WithPassword("yourStrong(!)Password")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await MsSqlContainer.StartAsync();
        await InitializeDatabaseAsync();
    }


    private async Task InitializeDatabaseAsync()
    {
        var connectionString = MsSqlContainer.GetConnectionString();

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
        await MsSqlContainer.StopAsync();
    }

}