using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using AspireCleanArchitecture.Infrastructure.DataModels;
using AspireCleanArchitecture.Infrastructure.InternalUtilities;

namespace AspireCleanArchitecture.Infrastructure.Tests.InternalUtilities;

public class DapperSqlExecutorTests
{
  private readonly DapperSqlExecutor _executor = new();
  private readonly IDbConnection _connection;

  public DapperSqlExecutorTests()
  {
    _connection = new SqliteConnection("Data Source=:memory:");
    _connection.Open();

    _connection.Execute("CREATE TABLE users (id TEXT PRIMARY KEY, user_name TEXT, password_hash TEXT);");
  }

  [Fact]
  public async Task ExecuteAsync_ShouldInsertRow()
  {
    // Arrange
    const string sql = "INSERT INTO users (id, user_name, password_hash) VALUES (@Id, @Name, @Hash)";
    var parameters = new
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Fabi",
      Hash = "hashed_pw"
    };

    // Act
    var rowsAffected = await _executor.ExecuteAsync(_connection, sql, parameters, CancellationToken.None);

    // Assert
    Assert.Equal(1, rowsAffected);
  }

  [Fact]
  public async Task QueryAsync_ShouldReturnUsers()
  {
    // Arrange
    const string insertSql = "INSERT INTO users (id, user_name, password_hash) VALUES (@Id, @Name, @Hash)";
    var id = Guid.NewGuid().ToString();
    await _connection.ExecuteAsync(insertSql, new {Id = id, Name = "username", Hash = "xyz"});

    const string querySql = "SELECT * FROM users";

    // Act
    var users = await _executor.QueryAsync<dynamic>(_connection, querySql, null, CancellationToken.None);

    // Assert
    Assert.Single(users);
  }

  [Fact]
  public async Task QueryFirstOrDefaultAsync_ShouldReturnUser()
  {
    // Arrange
    const string insertSql = "INSERT INTO users (id, user_name, password_hash) VALUES (@Id, @Name, @Hash)";
    var userId = Guid.NewGuid();
    const string userName = "username";
    const string passwordHash = "hash";

    await _connection.ExecuteAsync(insertSql, new {Id = userId.ToString(), Name = userName, Hash = passwordHash});

    const string selectSql =
      "SELECT id AS Id, user_name AS UserName, password_hash AS PasswordHash FROM users WHERE user_name = @Name";
    var parameters = new {Name = userName};

    // Act
    var rawResult = await _executor.QueryFirstOrDefaultAsync<dynamic>(
      _connection, selectSql, parameters, CancellationToken.None
    );

    var result = rawResult == null
      ? null
      : new UserDataModel
      {
        Id = Guid.Parse((string) rawResult.Id),
        MailAddress = rawResult.MailAddress,
        UserName = rawResult.UserName,
        PasswordHash = rawResult.PasswordHash
      };

    // Assert
    Assert.NotNull(result);
    Assert.Equal(userName, result.UserName);
    Assert.Equal(passwordHash, result.PasswordHash);
  }
}
