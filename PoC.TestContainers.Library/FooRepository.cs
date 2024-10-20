using System.Data.SqlClient;
using Dapper;

namespace PoC.TestContainers.Library;

public class FooRepository(string connectionstring)
{
    public async Task AddAsync(Foo foo)
    {
        await using var connection = new SqlConnection(connectionstring);
        await connection.ExecuteAsync("INSERT INTO Foo (Bar) VALUES (@Bar)", foo);
    }

    public async Task<IEnumerable<Foo>> GetAllAsync()
    {
        await using var connection = new SqlConnection(connectionstring);
        return await connection.QueryAsync<Foo>("SELECT * FROM Foo");
    }

    public async Task<Foo> GetByIdAsync(int id)
    {
        await using var connection = new SqlConnection(connectionstring);
        return await connection.QueryFirstOrDefaultAsync<Foo>("SELECT * FROM Foo WHERE Id = @Id", new { Id = id });
    }

    public async Task UpdateAsync(Foo foo)
    {
        await using var connection = new SqlConnection(connectionstring);
        await connection.ExecuteAsync("UPDATE Foo SET Bar = @Bar WHERE Id = @Id", foo);
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = new SqlConnection(connectionstring);
        await connection.ExecuteAsync("DELETE FROM Foo WHERE Id = @Id", new { Id = id });
    }


    public async Task<int> CountAsync()
    {
        await using var connection = new SqlConnection(connectionstring);
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Foo");
    }
}