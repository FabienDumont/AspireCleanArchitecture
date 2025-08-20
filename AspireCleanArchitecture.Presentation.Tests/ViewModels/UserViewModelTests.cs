using AspireCleanArchitecture.Presentation.ViewModels;

namespace AspireCleanArchitecture.Presentation.Tests.ViewModels;

public class UserViewModelTests
{
  #region Methods

  [Fact]
  public void Constructor_ShouldSetPropertiesCorrectly()
  {
    // Arrange
    var id = Guid.NewGuid();
    const string mailAddress = "test@test.com";
    const string userName = "username";

    // Act
    var viewModel = new UserViewModel(id, mailAddress, userName);

    // Assert
    Assert.Equal(id, viewModel.Id);
    Assert.Equal(mailAddress, viewModel.MailAddress);
    Assert.Equal(userName, viewModel.UserName);
  }

  #endregion
}
