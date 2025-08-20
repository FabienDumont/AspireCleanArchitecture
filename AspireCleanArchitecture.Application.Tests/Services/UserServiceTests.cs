using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Application.Exceptions;
using AspireCleanArchitecture.Application.Exceptions.Factories;
using AspireCleanArchitecture.Application.Services;
using AspireCleanArchitecture.Domain;

namespace AspireCleanArchitecture.Application.Tests.Services;

public class UserServiceTests
{
  #region Fields

  private readonly UserManager<User> _userManager = A.Fake<UserManager<User>>(options =>
    options.WithArgumentsForConstructor(() => new UserManager<User>(
        A.Fake<IUserStore<User>>(), null, null, null, null, null, null, null, null
      )
    )
  );

  private readonly IUserRepository _userRepository = A.Fake<IUserRepository>();
  private readonly IApiExceptionFactory _apiExceptionFactory = A.Fake<IApiExceptionFactory>();
  private readonly ILogger<UserService> _logger = A.Fake<ILogger<UserService>>();

  private readonly UserService _userService;

  #endregion

  #region Ctors

  public UserServiceTests()
  {
    _userService = new UserService(_userManager, _userRepository, _apiExceptionFactory, _logger);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task GetAllAsync_ShouldReturnAllUsers()
  {
    // Arrange
    var users = new List<User> {User.Load(Guid.NewGuid(), "test@test.com", "Edge", "hash")};
    A.CallTo(() => _userRepository.GetAllAsync(A<CancellationToken>._)).Returns(users);

    // Act
    var result = await _userService.GetAllAsync(CancellationToken.None);

    // Assert
    Assert.Equal(users.Count, result.Count);
    Assert.Equal(users[0].Id, result.First().Id);
  }

  [Fact]
  public async Task CreateAsync_ShouldCreateUserAndReturnIt()
  {
    // Arrange
    const string username = "Punk";
    const string mailAddress = "test@test.com";
    const string password = "pass";
    var inputUser = User.Load(Guid.NewGuid(), mailAddress, username, password);

    var newUser = User.Create(inputUser.MailAddress, inputUser.UserName, inputUser.PasswordHash);

    A.CallTo(() => _userManager.CreateAsync(
        A<User>.That.Matches(u => u.UserName == inputUser.UserName), inputUser.PasswordHash
      )
    ).Returns(IdentityResult.Success);

    A.CallTo(() => _userManager.FindByIdAsync(A<string>._)).Returns(newUser);

    // Act
    var result = await _userService.CreateAsync(mailAddress, username, password, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(newUser.MailAddress, result.MailAddress);
    Assert.Equal(newUser.UserName, result.UserName);
  }

  [Fact]
  public async Task CreateAsync_ShouldThrowConflictException_WhenUsernameAlreadyExists()
  {
    // Arrange
    const string mail = "test@example.com";
    const string username = "Edge";
    const string password = "pass";

    var existingUser = User.Create(mail, username, password);

    A.CallTo(() => _userManager.FindByNameAsync(username)).Returns(existingUser);

    var expected = new ConflictException(ErrorCode.UsernameAlreadyExists, $"Username {username} already exists");
    A.CallTo(() => _apiExceptionFactory.Create<ConflictException>(ErrorCode.UsernameAlreadyExists, username))
      .Returns(expected);

    // Act & Assert
    var ex = await Assert.ThrowsAsync<ConflictException>(() => _userService.CreateAsync(
        mail, username, password, CancellationToken.None
      )
    );

    Assert.Equal(expected, ex);
  }

  [Fact]
  public async Task CreateAsync_ShouldThrowConflictException_WhenMailAlreadyExists()
  {
    // Arrange
    const string mail = "test@example.com";
    const string username = "Edge";
    const string password = "pass";

    A.CallTo(() => _userManager.FindByNameAsync(username)).Returns<User>(null!);
    A.CallTo(() => _userManager.FindByEmailAsync(mail)).Returns(User.Create(mail, username, password));

    var expected = new ConflictException(ErrorCode.MailAddressAlreadyExists, $"Email {mail} already exists");
    A.CallTo(() => _apiExceptionFactory.Create<ConflictException>(ErrorCode.MailAddressAlreadyExists, mail))
      .Returns(expected);

    // Act & Assert
    var ex = await Assert.ThrowsAsync<ConflictException>(() => _userService.CreateAsync(
        mail, username, password, CancellationToken.None
      )
    );

    Assert.Equal(expected, ex);
  }

  [Fact]
  public async Task CreateAsync_ShouldThrowBadRequestException_WhenIdentityCreationFails()
  {
    // Arrange
    const string mail = "test@example.com";
    const string username = "Edge";
    const string password = "pass";

    A.CallTo(() => _userManager.FindByNameAsync(username)).Returns<User>(null!);
    A.CallTo(() => _userManager.FindByEmailAsync(mail)).Returns<User>(null!);

    var identityErrors = new[]
    {
      new IdentityError {Description = "Password is too short"},
      new IdentityError {Description = "Password must contain a number"}
    };

    var identityResult = IdentityResult.Failed(identityErrors);
    A.CallTo(() => _userManager.CreateAsync(A<User>._, password)).Returns(identityResult);

    var expected = new BadRequestException(
      ErrorCode.UserCreationFailed, "Password is too short, Password must contain a number"
    );

    A.CallTo(() => _apiExceptionFactory.Create<BadRequestException>(
        ErrorCode.UserCreationFailed,
        A<string[]>.That.Matches(args => args.Length == 2 &&
                                         args[0].ToString() == "Password is too short" &&
                                         args[1].ToString() == "Password must contain a number"
        )
      )
    ).Returns(expected);

    // Act & Assert
    var ex = await Assert.ThrowsAsync<BadRequestException>(() => _userService.CreateAsync(
        mail, username, password, CancellationToken.None
      )
    );

    Assert.Equal(expected, ex);
  }

  [Fact]
  public async Task SignInAsync_ShouldReturnUser_WhenCredentialsAreValid()
  {
    // Arrange
    var user = User.Create("Mox", "hash");
    A.CallTo(() => _userManager.FindByNameAsync("Mox")).Returns(user);
    A.CallTo(() => _userManager.CheckPasswordAsync(user, "hash")).Returns(true);

    // Act
    var result = await _userService.SignInAsync("Mox", "hash", CancellationToken.None);

    // Assert
    Assert.Equal(user.Id, result.Id);
  }

  [Fact]
  public async Task SignInAsync_ShouldFallbackToEmail_WhenUserNameNotFound()
  {
    // Arrange
    var user = User.Create("test@test.com", "Jericho", "hash");
    A.CallTo(() => _userManager.FindByNameAsync("test@test.com")).Returns<User>(null!);
    A.CallTo(() => _userManager.FindByEmailAsync("test@test.com")).Returns(user);
    A.CallTo(() => _userManager.CheckPasswordAsync(user, "hash")).Returns(true);

    // Act
    var result = await _userService.SignInAsync("test@test.com", "hash", CancellationToken.None);

    // Assert
    Assert.Equal(user.Id, result.Id);
  }

  [Fact]
  public async Task SignInAsync_ShouldThrowUnauthorizedException_WhenUserIsNotFoundOrPasswordInvalid()
  {
    // Arrange
    const string usernameOrMail = "fake@user.com";
    const string password = "wrongpass";

    A.CallTo(() => _userManager.FindByNameAsync(usernameOrMail)).Returns<User>(null!);
    A.CallTo(() => _userManager.FindByEmailAsync(usernameOrMail)).Returns<User>(null!);

    var expectedException = new UnauthorizedException(ErrorCode.InvalidCredentials, "Invalid credentials");

    A.CallTo(() => _apiExceptionFactory.Create<UnauthorizedException>(ErrorCode.InvalidCredentials, usernameOrMail))
      .Returns(expectedException);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<UnauthorizedException>(() =>
      _userService.SignInAsync(usernameOrMail, password, CancellationToken.None)
    );

    Assert.Equal(expectedException, exception);
    Assert.Equal(ErrorCode.InvalidCredentials, exception.Code);
  }

  [Fact]
  public async Task SignUpAsync_ShouldReturnCreatedUser_WhenSuccessful()
  {
    // Arrange
    const string username = "Danielson";
    const string mail = "american.dragon@example.com";
    const string password = "YesYesYes123";

    // No user with same username or email
    A.CallTo(() => _userManager.FindByNameAsync(username)).Returns<User>(null!);
    A.CallTo(() => _userManager.FindByEmailAsync(mail)).Returns<User>(null!);

    // Simulate success from CreateAsync
    A.CallTo(() => _userManager.CreateAsync(A<User>._, password)).Returns(IdentityResult.Success);

    // Return a user from userManager
    var expectedUser = User.Create(mail.ToLower(), username, password);
    A.CallTo(() => _userManager.FindByIdAsync(A<string>._)).Returns(expectedUser);

    // Act
    var result = await _userService.SignUpAsync(mail, username, password, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedUser.UserName, result.UserName);
    Assert.Equal(expectedUser.MailAddress.ToLower(), result.MailAddress); // check lowercasing
  }

  [Fact]
  public async Task SignUpAsync_ShouldThrowConflictException_WhenUsernameAlreadyExists()
  {
    // Arrange
    const string username = "Punk";
    const string mail = "test@example.com";
    const string password = "123";

    var existingUser = User.Create(mail, username, password);

    A.CallTo(() => _userManager.FindByNameAsync(username)).Returns(existingUser);

    var expected = new ConflictException(ErrorCode.UsernameAlreadyExists, $"Username {username} already exists");
    A.CallTo(() => _apiExceptionFactory.Create<ConflictException>(ErrorCode.UsernameAlreadyExists, username))
      .Returns(expected);

    // Act & Assert
    var ex = await Assert.ThrowsAsync<ConflictException>(() => _userService.SignUpAsync(
        mail, username, password, CancellationToken.None
      )
    );

    Assert.Equal(expected, ex);
  }

  [Fact]
  public async Task SignUpAsync_ShouldThrowConflictException_WhenMailAlreadyExists()
  {
    // Arrange
    const string username = "Jericho";
    const string mail = "test@example.com";
    const string password = "456";

    A.CallTo(() => _userManager.FindByNameAsync(username)).Returns<User>(null!);
    A.CallTo(() => _userManager.FindByEmailAsync(mail)).Returns(User.Create(mail, username, password));

    var expected = new ConflictException(ErrorCode.MailAddressAlreadyExists, $"Mail {mail} already exists");
    A.CallTo(() => _apiExceptionFactory.Create<ConflictException>(ErrorCode.MailAddressAlreadyExists, username))
      .Returns(expected);

    // Act & Assert
    var ex = await Assert.ThrowsAsync<ConflictException>(() => _userService.SignUpAsync(
        mail, username, password, CancellationToken.None
      )
    );

    Assert.Equal(expected, ex);
  }

  [Fact]
  public async Task SignUpAsync_ShouldThrowBadRequestException_WhenIdentityCreationFails()
  {
    // Arrange
    const string username = "Mox";
    const string mail = "mox@test.com";
    const string password = "789";

    A.CallTo(() => _userManager.FindByNameAsync(username)).Returns<User>(null!);
    A.CallTo(() => _userManager.FindByEmailAsync(mail)).Returns<User>(null!);

    var identityErrors = new[]
    {
      new IdentityError {Description = "Email is invalid"},
      new IdentityError {Description = "Password is weak"}
    };

    var failedResult = IdentityResult.Failed(identityErrors);
    A.CallTo(() => _userManager.CreateAsync(A<User>._, password)).Returns(failedResult);

    const string expectedMessage = "Failed to create user Mox : Email is invalid / Password is weak.";

    A.CallTo(() => _apiExceptionFactory.Create<BadRequestException>(
        ErrorCode.UserCreationFailed,
        A<string[]>.That.Matches(args =>
          args.Length == 2 && args[0] == username && args[1] == "Email is invalid / Password is weak"
        )
      )
    ).ReturnsLazily((ErrorCode code, string[] args) =>
      {
        var formatted = $"Failed to create user {args[0]} : {args[1]}.";
        return new BadRequestException(code, formatted);
      }
    );

    // Act & Assert
    var ex = await Record.ExceptionAsync(() => _userService.SignUpAsync(
        mail, username, password, CancellationToken.None
      )
    );

    var badRequest = Assert.IsType<BadRequestException>(ex, exactMatch: false);
    Assert.Equal(expectedMessage, badRequest.Message);
  }

  #endregion
}
