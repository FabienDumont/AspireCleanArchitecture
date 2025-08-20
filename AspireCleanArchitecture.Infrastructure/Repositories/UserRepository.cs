using System.Data;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Infrastructure.Abstraction;
using AspireCleanArchitecture.Infrastructure.DataModels;
using AspireCleanArchitecture.Infrastructure.Mappers;

namespace AspireCleanArchitecture.Infrastructure.Repositories;

public class UserRepository(IDbConnection dbConnection, ISqlExecutor executor) : IUserRepository
{
  #region Implementation of IUserRepository

  /// <inheritdoc />
  public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
  {
    const string sql = $"""
        SELECT
          id AS {nameof(UserDataModel.Id)},
          mail_address AS {nameof(UserDataModel.MailAddress)},
          user_name AS {nameof(UserDataModel.UserName)},
          password_hash AS {nameof(UserDataModel.PasswordHash)}
        FROM users
        WHERE id = @Id LIMIT 1
      """;

    var dataModel = await executor.QueryFirstOrDefaultAsync<UserDataModel>(
      dbConnection, sql, new {Id = id}, cancellationToken
    );
    return dataModel?.ToDomain();
  }

  public async Task<User?> GetByNormalizedMailAddressAsync(
    string normalizedMailAddress, CancellationToken cancellationToken
  )
  {
    const string sql = $"""
        SELECT
          id AS {nameof(UserDataModel.Id)},
          mail_address AS {nameof(UserDataModel.MailAddress)},
          user_name AS {nameof(UserDataModel.UserName)},
          password_hash AS {nameof(UserDataModel.PasswordHash)}
        FROM users
      WHERE UPPER(mail_address) = @NormalizedMailAddress LIMIT 1
      """;

    var dataModel = await executor.QueryFirstOrDefaultAsync<UserDataModel>(
      dbConnection, sql, new {NormalizedMailAddress = normalizedMailAddress}, cancellationToken
    );

    return dataModel?.ToDomain();
  }

  /// <inheritdoc />
  public async Task<User?> GetByNormalizedUserNameAsync(string normalizedUserName, CancellationToken cancellationToken)
  {
    const string sql = $"""
        SELECT
          id AS {nameof(UserDataModel.Id)},
          mail_address AS {nameof(UserDataModel.MailAddress)},
          user_name AS {nameof(UserDataModel.UserName)},
          password_hash AS {nameof(UserDataModel.PasswordHash)}
        FROM users
      WHERE UPPER(user_name) = @NormalizedUserName LIMIT 1
      """;

    var dataModel = await executor.QueryFirstOrDefaultAsync<UserDataModel>(
      dbConnection, sql, new {NormalizedUserName = normalizedUserName}, cancellationToken
    );

    return dataModel?.ToDomain();
  }

  /// <inheritdoc />
  public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken)
  {
    const string sql = $"""
        SELECT
          id AS {nameof(UserDataModel.Id)},
          mail_address AS {nameof(UserDataModel.MailAddress)},
          user_name AS {nameof(UserDataModel.UserName)},
          password_hash AS {nameof(UserDataModel.PasswordHash)}
        FROM users
      """;

    var results = await executor.QueryAsync<UserDataModel>(dbConnection, sql, null, cancellationToken);

    return results.ToDomainCollection();
  }

  /// <inheritdoc />
  public async Task<User> CreateAsync(User domain, CancellationToken cancellationToken)
  {
    const string sql = """
        INSERT INTO users (id, mail_address, user_name, password_hash)
        VALUES (@Id, @MailAddress, @UserName, @PasswordHash)
      """;

    await executor.ExecuteAsync(
      dbConnection, sql, new
      {
        domain.Id,
        domain.MailAddress,
        domain.UserName,
        domain.PasswordHash
      }, cancellationToken
    );

    return domain;
  }

  /// <inheritdoc />
  public async Task<User> UpdateAsync(User domain, CancellationToken cancellationToken)
  {
    const string sql = """
        UPDATE users SET
            mail_address = @MailAddress,
            user_name = @UserName,
            password_hash = @PasswordHash,
            password_salt = @PasswordSalt
        WHERE id = @Id
      """;

    await executor.ExecuteAsync(
      dbConnection, sql, new
      {
        domain.Id,
        domain.MailAddress,
        domain.UserName,
        domain.PasswordHash
      }, cancellationToken
    );

    return domain;
  }

  /// <inheritdoc />
  public async Task DeleteAsync(User domain, CancellationToken cancellationToken)
  {
    const string sql = "DELETE FROM users WHERE id = @Id";

    await executor.ExecuteAsync(
      dbConnection, sql, new
      {
        domain.Id
      }, cancellationToken
    );
  }

  #endregion
}
