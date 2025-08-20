using AspireCleanArchitecture.Presentation.ViewModels;

namespace AspireCleanArchitecture.Presentation.Tests.ViewModels;

public class LogInViewModelTests
{
  [Fact]
  public void Constructor_ShouldSetPropertiesCorrectly()
  {
    // Arrange
    var user = new UserViewModel(Guid.NewGuid(), "username", "test@test.com");
    var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

    // Act
    var viewModel = new LogInViewModel(user, token);

    // Assert
    Assert.Equal(user, viewModel.User);
    Assert.Equal(token, viewModel.BearerToken);
  }

  [Fact]
  public void Properties_ShouldBeSettable()
  {
    // Arrange
    var user1 = new UserViewModel(Guid.NewGuid(), "username", "test@test.com");
    var user2 = new UserViewModel(Guid.NewGuid(), "username2", "test2@test.com");
    var viewModel = new LogInViewModel(user1, "token1")
    {
      // Act
      User = user2,
      BearerToken = "token2"
    };

    // Assert
    Assert.Equal(user2, viewModel.User);
    Assert.Equal("token2", viewModel.BearerToken);
  }
}
