using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Infrastructure.Stores;

namespace AspireCleanArchitecture.Infrastructure.Tests.Stores;

public class UserStoreTests
{
  #region Fields

  private readonly IUserRepository _repo = A.Fake<IUserRepository>();
  private readonly UserStore _store;

  #endregion

  #region Ctors

  public UserStoreTests()
  {
    _store = new UserStore(_repo);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task GetUserIdAsync_ShouldReturnIdAsString()
  {
    // Arrange
    var user = User.Create("username", "hash");

    // Act
    var id = await _store.GetUserIdAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal(user.Id.ToString(), id);
  }

  [Fact]
  public async Task GetUserNameAsync_ShouldReturnUserName()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    var userName = await _store.GetUserNameAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal("username", userName);
  }

  [Fact]
  public async Task SetUserNameAsync_ShouldUpdateUserNameAndNormalize()
  {
    // Arrange
    var user = User.Create("test@test.com", "OldName", "hash");

    // Act
    await _store.SetUserNameAsync(user, "NewName", CancellationToken.None);

    // Assert
    Assert.Equal("NewName", user.UserName);
    Assert.Equal("NEWNAME", user.NormalizedUserName);
  }

  [Fact]
  public async Task GetNormalizedUserNameAsync_ShouldReturnNormalizedName()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    var normalized = await _store.GetNormalizedUserNameAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal("USERNAME", normalized);
  }

  [Fact]
  public async Task SetNormalizedUserNameAsync_ShouldSetNormalizedNameOnly()
  {
    // Arrange
    var user = User.Create("test@test.com", "USERNAME", "hash");

    // Act
    await _store.SetNormalizedUserNameAsync(user, "NORMUSERNAME", CancellationToken.None);

    // Assert
    Assert.Equal("NORMUSERNAME", user.NormalizedUserName);
    Assert.Equal("USERNAME", user.UserName); // not changed
  }

  [Fact]
  public async Task GetMailAddressAsync_ShouldReturnMailAddress()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    var mailAddress = await _store.GetEmailAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal("test@test.com", mailAddress);
  }

  [Fact]
  public async Task SetMailAddressAsync_ShouldUpdateMailAddressAndNormalize()
  {
    // Arrange
    var user = User.Create("test@test.com", "OldName", "hash");

    // Act
    await _store.SetEmailAsync(user, "test1@test.com", CancellationToken.None);

    // Assert
    Assert.Equal("test1@test.com", user.MailAddress);
    Assert.Equal("TEST1@TEST.COM", user.NormalizedMailAddress);
  }

  [Fact]
  public async Task GetNormalizedMailAddressAsync_ShouldReturnNormalized()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    var normalized = await _store.GetNormalizedEmailAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal("TEST@TEST.COM", normalized);
  }

  [Fact]
  public async Task SetNormalizedMailAddressAsync_ShouldSetNormalizedOnly()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    await _store.SetNormalizedEmailAsync(user, "TEST1@TEST.COM", CancellationToken.None);

    // Assert
    Assert.Equal("TEST1@TEST.COM", user.NormalizedMailAddress);
    Assert.Equal("test@test.com", user.MailAddress);
  }

  [Fact]
  public async Task GetEmailConfirmedAsync_ShouldAlwaysReturnTrue()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    var result = await _store.GetEmailConfirmedAsync(user, CancellationToken.None);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public async Task SetEmailConfirmedAsync_ShouldNotThrow()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    var ex = await Record.ExceptionAsync(() => _store.SetEmailConfirmedAsync(user, true, CancellationToken.None));

    // Assert
    Assert.Null(ex); // No-op but still should not throw
  }

  [Fact]
  public async Task FindByNMailAddressAsync_ShouldReturnUser()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");
    A.CallTo(() => _repo.GetByNormalizedMailAddressAsync("TEST@TEST.COM", A<CancellationToken>._)).Returns(user);

    // Act
    var result = await _store.FindByEmailAsync("TEST@TEST.COM", CancellationToken.None);

    // Assert
    Assert.Equal(user.MailAddress, result.MailAddress);
  }

  [Fact]
  public void Dispose_ShouldNotThrow()
  {
    // Act
    var ex = Record.Exception(() => _store.Dispose());

    // Assert
    Assert.Null(ex);
  }

  [Fact]
  public async Task SetPasswordHashAsync_ShouldUpdateHash()
  {
    // Arrange
    var user = User.Create("username", "old");

    // Act
    await _store.SetPasswordHashAsync(user, "newHash", CancellationToken.None);

    // Assert
    Assert.Equal("newHash", user.PasswordHash);
  }

  [Fact]
  public async Task GetPasswordHashAsync_ShouldReturnHash()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "123");

    // Act
    var hash = await _store.GetPasswordHashAsync(user, CancellationToken.None);

    // Assert
    Assert.Equal("123", hash);
  }

  [Fact]
  public async Task HasPasswordAsync_ShouldReturnTrueIfNotEmpty()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hash");

    // Act
    var result = await _store.HasPasswordAsync(user, CancellationToken.None);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public async Task CreateAsync_ShouldCallRepositoryAndReturnSuccess()
  {
    // Arrange
    var user = User.Create("username", "hash");

    // Act
    var result = await _store.CreateAsync(user, CancellationToken.None);

    // Assert
    A.CallTo(() => _repo.CreateAsync(user, A<CancellationToken>._)).MustHaveHappened();
    Assert.True(result.Succeeded);
  }

  [Fact]
  public async Task CreateAsync_ShouldReturnFailureIfException()
  {
    // Arrange
    var user = User.Create("username", "hash");
    A.CallTo(() => _repo.CreateAsync(user, A<CancellationToken>._)).Throws<Exception>();

    // Act
    var result = await _store.CreateAsync(user, CancellationToken.None);

    // Assert
    Assert.False(result.Succeeded);
    Assert.Contains(result.Errors, e => e.Code == "USER_CREATE_FAILED");
  }

  [Fact]
  public async Task FindByIdAsync_ShouldReturnUser()
  {
    // Arrange
    var user = User.Create("username", "hash");
    A.CallTo(() => _repo.GetByIdAsync(user.Id, A<CancellationToken>._)).Returns(user);

    // Act
    var result = await _store.FindByIdAsync(user.Id.ToString(), CancellationToken.None);

    // Assert
    Assert.Equal(user.Id, result.Id);
  }

  [Fact]
  public async Task FindByIdAsync_ShouldReturnNull_WhenGuidInvalid()
  {
    // Act
    var result = await _store.FindByIdAsync("not-a-guid", CancellationToken.None);

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public async Task FindByNameAsync_ShouldReturnUser()
  {
    // Arrange
    var user = User.Create("username", "hash");
    A.CallTo(() => _repo.GetByNormalizedUserNameAsync("USERNAME", A<CancellationToken>._)).Returns(user);

    // Act
    var result = await _store.FindByNameAsync("USERNAME", CancellationToken.None);

    // Assert
    Assert.Equal(user.UserName, result.UserName);
  }

  [Fact]
  public async Task UpdateAsync_ShouldCallRepository()
  {
    // Arrange
    var user = User.Create("username", "hash");

    // Act
    var result = await _store.UpdateAsync(user, CancellationToken.None);

    // Assert
    A.CallTo(() => _repo.UpdateAsync(user, A<CancellationToken>._)).MustHaveHappened();
    Assert.True(result.Succeeded);
  }

  [Fact]
  public async Task UpdateAsync_ShouldReturnFailureIfException()
  {
    // Arrange
    var user = User.Create("username", "hash");
    A.CallTo(() => _repo.UpdateAsync(user, A<CancellationToken>._)).Throws<Exception>();

    // Act
    var result = await _store.UpdateAsync(user, CancellationToken.None);

    // Assert
    Assert.False(result.Succeeded);
    Assert.Contains(result.Errors, e => e.Code == "USER_UPDATE_FAILED");
  }

  [Fact]
  public async Task DeleteAsync_ShouldCallRepository()
  {
    // Arrange
    var user = User.Create("username", "hash");

    // Act
    var result = await _store.DeleteAsync(user, CancellationToken.None);

    // Assert
    A.CallTo(() => _repo.DeleteAsync(user, A<CancellationToken>._)).MustHaveHappened();
    Assert.True(result.Succeeded);
  }

  [Fact]
  public async Task DeleteAsync_ShouldReturnFailureIfException()
  {
    // Arrange
    var user = User.Create("username", "hash");
    A.CallTo(() => _repo.DeleteAsync(user, A<CancellationToken>._)).Throws<Exception>();

    // Act
    var result = await _store.DeleteAsync(user, CancellationToken.None);

    // Assert
    Assert.False(result.Succeeded);
    Assert.Contains(result.Errors, e => e.Code == "USER_DELETE_FAILED");
  }

  #endregion
}
