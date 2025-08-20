using System.Data;

namespace AspireCleanArchitecture.Infrastructure.Abstraction;

public interface ISqlExecutor
{
  Task<T?> QueryFirstOrDefaultAsync<T>(
    IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken
  );

  Task<IEnumerable<T>> QueryAsync<T>(
    IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken
  );

  Task<int> ExecuteAsync(IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken);
}
