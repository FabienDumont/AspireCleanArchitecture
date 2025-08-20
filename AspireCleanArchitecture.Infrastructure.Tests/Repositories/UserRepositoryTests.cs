using System.Data;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Infrastructure.Abstraction;
using AspireCleanArchitecture.Infrastructure.DataModels;
using AspireCleanArchitecture.Infrastructure.Repositories;

namespace AspireCleanArchitecture.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
  #region Fields

  private readonly IDbConnection _connection = A.Fake<IDbConnection>();
  private readonly ISqlExecutor _sqlExecutor = A.Fake<ISqlExecutor>();
  private readonly UserRepository _repository;

  #endregion

  #region Ctors

  public UserRepositoryTests()
  {
    _repository = new UserRepository(_connection, _sqlExecutor);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task GetByIdAsync_ShouldReturnMappedUser()
  {
    // Arrange
    var id = Guid.NewGuid();
    var userData = new UserDataModel
    {
      Id = id,
      MailAddress = "test@test.com",
      UserName = "username",
      PasswordHash = "hash"
    };

    A.CallTo(() => _sqlExecutor.QueryFirstOrDefaultAsync<UserDataModel>(
        _connection, A<string>.Ignored, A<object>.Ignored, A<CancellationToken>.Ignored
      )
    ).Returns(userData);

    // Act
    var result = await _repository.GetByIdAsync(id, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(id, result.Id);
    Assert.Equal("username", result.UserName);
  }

  [Fact]
  public async Task GetByNormalizedMailAddressAsync_ShouldReturnMappedUser()
  {
    // Arrange
    const string normalizedMailAddress = "TEST@TEST.COM";
    var userData = new UserDataModel
    {
      Id = Guid.NewGuid(),
      MailAddress = "test@test.com",
      UserName = "username",
      PasswordHash = "hashed_pw"
    };

    A.CallTo(() => _sqlExecutor.QueryFirstOrDefaultAsync<UserDataModel>(
        _connection, A<string>.Ignored,
        A<object>.That.Matches(p =>
          (string) p.GetType().GetProperty("NormalizedMailAddress")!.GetValue(p)! == normalizedMailAddress
        ), A<CancellationToken>.Ignored
      )
    ).Returns(userData);

    // Act
    var result = await _repository.GetByNormalizedMailAddressAsync(normalizedMailAddress, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
  }

  [Fact]
  public async Task GetByNormalizedUserNameAsync_ShouldReturnMappedUser()
  {
    // Arrange
    const string normalizedUserName = "USERNAME";
    var userData = new UserDataModel
    {
      Id = Guid.NewGuid(),
      MailAddress = "test@test.com",
      UserName = "username",
      PasswordHash = "hashed_pw"
    };

    A.CallTo(() => _sqlExecutor.QueryFirstOrDefaultAsync<UserDataModel>(
        _connection, A<string>.Ignored,
        A<object>.That.Matches(p =>
          (string) p.GetType().GetProperty("NormalizedUserName")!.GetValue(p)! == normalizedUserName
        ), A<CancellationToken>.Ignored
      )
    ).Returns(userData);

    // Act
    var result = await _repository.GetByNormalizedUserNameAsync(normalizedUserName, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
  }

  [Fact]
  public async Task GetAllAsync_ShouldReturnAllMappedUsers()
  {
    // Arrange
    var userDataModels = new List<UserDataModel>
    {
      new() {Id = Guid.NewGuid(), MailAddress = "test@test.com", UserName = "username", PasswordHash = "hash1"},
      new() {Id = Guid.NewGuid(), MailAddress = "test2@test.com", UserName = "username2", PasswordHash = "hash2"}
    };

    A.CallTo(() => _sqlExecutor.QueryAsync<UserDataModel>(
        _connection, A<string>.Ignored, null, A<CancellationToken>.Ignored
      )
    ).Returns(userDataModels);

    // Act
    var result = await _repository.GetAllAsync(CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count);
    Assert.Contains(result, u => u.UserName == "username");
    Assert.Contains(result, u => u.UserName == "username2");
  }

  [Fact]
  public async Task CreateAsync_ShouldInsertUserAndReturnDomainObject()
  {
    // Arrange
    var user = User.Load(Guid.NewGuid(), "test@test.com", "username", "hash");

    A.CallTo(() => _sqlExecutor.ExecuteAsync(
        _connection, A<string>.Ignored,
        A<object>.That.Matches(p =>
          p.GetType().GetProperty("Id")!.GetValue(p)!.Equals(user.Id) &&
          p.GetType().GetProperty("UserName")!.GetValue(p)!.Equals(user.UserName) &&
          p.GetType().GetProperty("PasswordHash")!.GetValue(p)!.Equals(user.PasswordHash)
        ), A<CancellationToken>.Ignored
      )
    ).Returns(1);

    // Act
    var result = await _repository.CreateAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal(user, result);
  }

  [Fact]
  public async Task UpdateAsync_ShouldUpdateUserAndReturnDomainObject()
  {
    // Arrange
    var user = User.Load(Guid.NewGuid(), "test@test.com", "username", "hashedPassword");

    A.CallTo(() => _sqlExecutor.ExecuteAsync(
        _connection, A<string>.Ignored,
        A<object>.That.Matches(p =>
          p.GetType().GetProperty("Id")!.GetValue(p)!.Equals(user.Id) &&
          p.GetType().GetProperty("UserName")!.GetValue(p)!.Equals(user.UserName) &&
          p.GetType().GetProperty("PasswordHash")!.GetValue(p)!.Equals(user.PasswordHash)
        ), A<CancellationToken>.Ignored
      )
    ).Returns(1);

    // Act
    var result = await _repository.UpdateAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal(user, result);
  }

  [Fact]
  public async Task DeleteAsync_ShouldExecuteDeleteWithCorrectParameters()
  {
    // Arrange
    var user = User.Load(Guid.NewGuid(), "test@test.com", "username", "hashedPassword");

    const string expectedSql = "DELETE FROM users WHERE id = @Id";

    A.CallTo(() => _sqlExecutor.ExecuteAsync(
        _connection, expectedSql,
        A<object>.That.Matches(p => p.GetType().GetProperty("Id")!.GetValue(p)!.Equals(user.Id)),
        A<CancellationToken>.Ignored
      )
    ).Returns(1);

    // Act
    await _repository.DeleteAsync(user, CancellationToken.None);

    // Assert
    A.CallTo(() => _sqlExecutor.ExecuteAsync(
        _connection, expectedSql,
        A<object>.That.Matches(p => p.GetType().GetProperty("Id")!.GetValue(p)!.Equals(user.Id)),
        A<CancellationToken>.Ignored
      )
    ).MustHaveHappenedOnceExactly();
  }

  #endregion
}
