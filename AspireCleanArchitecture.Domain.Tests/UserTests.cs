namespace AspireCleanArchitecture.Domain.Tests;

public class UserTests
{
  #region Methods

  [Fact]
  public void Create_ShouldInitializeUser_WithCorrectValues()
  {
    // Arrange
    const string mailAddress = "test@test.com";
    const string userName = "username";
    const string passwordHash = "hash";

    // Act
    var user = User.Create(mailAddress, userName, passwordHash);

    // Assert
    Assert.NotNull(user);
    Assert.NotEqual(Guid.Empty, user.Id);
    Assert.Equal(mailAddress, user.MailAddress);
    Assert.Equal(userName, user.UserName);
    Assert.Equal(userName.ToUpperInvariant(), user.NormalizedUserName);
    Assert.Equal(passwordHash, user.PasswordHash);
  }

  [Fact]
  public void Load_ShouldInitializeUser_WithGivenValues()
  {
    // Arrange
    var id = Guid.NewGuid();
    const string mailAddress = "test@test.com";
    const string userName = "username";
    const string passwordHash = "oldhash";

    // Act
    var user = User.Load(id, mailAddress, userName, passwordHash);

    // Assert
    Assert.Equal(id, user.Id);
    Assert.Equal(userName, user.UserName);
    Assert.Equal(userName.ToUpperInvariant(), user.NormalizedUserName);
    Assert.Equal(passwordHash, user.PasswordHash);
  }

  [Fact]
  public void UpdateMailAddress_ShouldUpdateMailAddressAndNormalizedMailAddress()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "pass");

    // Act
    user.UpdateMailAddress("test1@test.com");

    // Assert
    Assert.Equal("test1@test.com", user.MailAddress);
    Assert.Equal("TEST1@TEST.COM", user.NormalizedMailAddress);
  }

  [Fact]
  public void UpdateNormalizedMailAddress_ShouldSetNormalizedMailAddressOnly()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "pass");

    // Act
    user.UpdateNormalizedMailAddress("TEST1@TEST.COM");

    // Assert
    Assert.Equal("test@test.com", user.MailAddress);
    Assert.Equal("TEST1@TEST.COM", user.NormalizedMailAddress);
  }

  [Fact]
  public void UpdateUserName_ShouldUpdateUsernameAndNormalizedName()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "pass");

    // Act
    user.UpdateUserName("Username2");

    // Assert
    Assert.Equal("Username2", user.UserName);
    Assert.Equal("USERNAME2", user.NormalizedUserName);
  }

  [Fact]
  public void UpdateNormalizedUserName_ShouldSetNormalizedNameOnly()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "pass");

    // Act
    user.UpdateNormalizedUserName("USERNAME");

    // Assert
    Assert.Equal("username", user.UserName);
    Assert.Equal("USERNAME", user.NormalizedUserName);
  }

  [Fact]
  public void UpdatePasswordHash_ShouldChangeHash()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "oldhash");

    // Act
    user.UpdatePasswordHash("newhash");

    // Assert
    Assert.Equal("newhash", user.PasswordHash);
  }

  #endregion
}
