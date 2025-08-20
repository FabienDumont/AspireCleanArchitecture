using System.Data;
using AspireCleanArchitecture.Infrastructure.Abstraction;
using Dapper;

namespace AspireCleanArchitecture.Infrastructure.InternalUtilities;

public class DapperSqlExecutor : ISqlExecutor
{
  public async Task<T?> QueryFirstOrDefaultAsync<T>(
    IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken
  )
  {
    var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
    return await connection.QueryFirstOrDefaultAsync<T>(command);
  }

  public async Task<IEnumerable<T>> QueryAsync<T>(
    IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken
  )
  {
    var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
    return await connection.QueryAsync<T>(command);
  }

  public async Task<int> ExecuteAsync(
    IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken
  )
  {
    var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
    return await connection.ExecuteAsync(command);
  }
}
