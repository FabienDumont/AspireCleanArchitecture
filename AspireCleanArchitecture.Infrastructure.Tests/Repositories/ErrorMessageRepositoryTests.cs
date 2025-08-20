using System.Globalization;
using AspireCleanArchitecture.Infrastructure.Repositories;

namespace AspireCleanArchitecture.Infrastructure.Tests.Repositories;

public class ErrorMessageRepositoryTests
{
  #region Fields

  private readonly ErrorMessageRepository _repository;

  #endregion

  #region Ctors

  public ErrorMessageRepositoryTests()
  {
    CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    _repository = new ErrorMessageRepository();
  }

  #endregion

  #region Methods

  [Fact]
  public void GetString_KnownKey_ShouldReturnExpectedMessage()
  {
    // Arrange
    const string key = "InvalidCredentials";
    const string expected = "Invalid credentials.";

    // Act
    var result = _repository.GetString(key);

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetString_UnknownKey_ShouldReturnKeyInBrackets()
  {
    // Arrange
    const string key = "NonExistentKey";

    // Act
    var result = _repository.GetString(key);

    // Assert
    Assert.Equal("[NonExistentKey]", result);
  }

  [Fact]
  public void GetString_WithArguments_ShouldFormatMessageCorrectly()
  {
    // Arrange
    const string key = "UserCreationFailed";
    string[] args = ["username", "Error"];
    var expected = $"Failed to create user {args[0]} : {args[1]}.";

    // Act
    var result = _repository.GetString(key, args);

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetString_WithArguments_UnknownKey_ShouldReturnKeyInBrackets()
  {
    // Arrange
    const string key = "SomeMissingKey";

    // Act
    var result = _repository.GetString(key, "arg");

    // Assert
    Assert.Equal("[SomeMissingKey]", result);
  }

  [Theory]
  [InlineData("fr-FR")]
  [InlineData("en-US")]
  public void GetString_ShouldRespectCulture(string culture)
  {
    // Arrange
    CultureInfo.CurrentUICulture = new CultureInfo(culture);
    const string key = "InvalidCredentials";

    // Act
    var result = _repository.GetString(key);

    // Assert
    Assert.False(string.IsNullOrWhiteSpace(result));
  }

  #endregion
}
