using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AspireCleanArchitecture.Application.Abstractions;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Presentation.Controllers;
using AspireCleanArchitecture.Presentation.Requests;
using AspireCleanArchitecture.Presentation.ViewModels;

namespace AspireCleanArchitecture.Presentation.Tests.Controllers;

public class UsersControllerTests
{
  #region Fields

  private readonly IConfiguration _configuration = A.Fake<IConfiguration>();
  private readonly IUserService _userService = A.Fake<IUserService>();
  private readonly UsersController _controller;

  #endregion

  #region Ctors

  public UsersControllerTests()
  {
    _controller = new UsersController(_configuration, _userService);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task GetAllAsync_ShouldReturnOkWithUserViewModels()
  {
    // Arrange
    var users = new List<User>
    {
      User.Create("test@test.com", "username", "hash"),
      User.Create("test2@test.com", "username2", "hash")
    };

    A.CallTo(() => _userService.GetAllAsync(CancellationToken.None)).Returns(users);

    // Act
    var result = await _controller.GetAllAsync();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var returned = Assert.IsType<IEnumerable<UserViewModel>>(okResult.Value, exactMatch: false);

    var list = returned.ToList();

    Assert.Equal(users.Count, list.Count);
    Assert.Contains(list, u => u.UserName == "username");
    Assert.Contains(list, u => u.UserName == "username2");
  }

  [Fact]
  public async Task CreateAsync_ShouldReturnOkWithUserViewModel()
  {
    // Arrange
    var request = new CreateUserRequest
    {
      MailAddress = "test@test.com",
      Username = "User",
      Password = "password"
    };
    var user = User.Create(request.Username, request.Password);

    A.CallTo(() => _userService.CreateAsync(
        request.MailAddress, request.Username, request.Password, CancellationToken.None
      )
    ).Returns(user);

    // Act
    var result = await _controller.CreateAsync(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var viewModel = Assert.IsType<UserViewModel>(okResult.Value);

    Assert.Equal(user.Id, viewModel.Id);
    Assert.Equal(user.UserName, viewModel.UserName);
    Assert.Equal(user.MailAddress, viewModel.MailAddress);
  }

  [Fact]
  public async Task SignIn_ShouldReturnToken_WhenCredentialsAreValid()
  {
    // Arrange
    var user = User.Create("test@test.com", "username", "hashed");
    var request = new SignInRequest
    {
      UsernameOrMailAddress = user.UserName,
      Password = "irrelevant"
    };

    A.CallTo(() => _userService.SignInAsync(
        request.UsernameOrMailAddress, request.Password, CancellationToken.None
      )
    ).Returns(user);

    A.CallTo(() => _configuration["Jwt:Key"]).Returns("SuperSecretSigningKey1234567890!");

    // Act
    var result = await _controller.SignIn(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<LogInViewModel>(okResult.Value);

    Assert.Equal(user.Id, response.User.Id);
    Assert.Equal(user.UserName, response.User.UserName);
    Assert.Equal(user.MailAddress, response.User.MailAddress);
    Assert.False(string.IsNullOrWhiteSpace(response.BearerToken));
  }

  [Fact]
  public async Task SignUpAsync_ShouldReturnOkWithUserViewModel()
  {
    // Arrange
    var request = new SignUpRequest()
    {
      MailAddress = "test@test.com",
      Username = "User",
      Password = "password"
    };
    var user = User.Create(request.Username, request.Password);

    A.CallTo(() => _userService.SignUpAsync(
        request.MailAddress, request.Username, request.Password, CancellationToken.None
      )
    ).Returns(user);

    // Act
    var result = await _controller.SignUp(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var viewModel = Assert.IsType<UserViewModel>(okResult.Value);

    Assert.Equal(user.Id, viewModel.Id);
    Assert.Equal(user.UserName, viewModel.UserName);
    Assert.Equal(user.MailAddress, viewModel.MailAddress);
  }


  #endregion
}
