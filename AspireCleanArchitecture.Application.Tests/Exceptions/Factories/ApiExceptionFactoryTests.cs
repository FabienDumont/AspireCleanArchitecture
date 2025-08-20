using System.Net;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Application.Exceptions;
using AspireCleanArchitecture.Application.Exceptions.Factories;

namespace AspireCleanArchitecture.Application.Tests.Exceptions.Factories;

public class ApiExceptionFactoryTests
{
  #region Fields

  private readonly IErrorMessageRepository _errorMessageRepository;
  private readonly ApiExceptionFactory _factory;

  #endregion

  #region Ctors

  public ApiExceptionFactoryTests()
  {
    _errorMessageRepository = A.Fake<IErrorMessageRepository>();
    _factory = new ApiExceptionFactory(_errorMessageRepository);
  }

  #endregion

  #region Methods

  [Fact]
  public void Create_WithConflictExceptionType_ReturnsConflictException()
  {
    // Arrange
    const ErrorCode code = ErrorCode.UsernameAlreadyExists;
    const string expectedMessage = "Username already exists";
    A.CallTo(() => _errorMessageRepository.GetString(code.ToString(), A<string[]>._)).Returns(expectedMessage);

    // Act
    var exception = _factory.Create<ConflictException>(code, "Punk");

    // Assert
    Assert.IsType<ConflictException>(exception);
    Assert.Equal(code, exception.Code);
    Assert.Equal(expectedMessage, exception.Message);
  }

  [Fact]
  public void Create_WithUnauthorizedExceptionType_ReturnsUnauthorizedException()
  {
    // Arrange
    const ErrorCode code = ErrorCode.InvalidCredentials;
    const string expectedMessage = "Invalid credentials";
    A.CallTo(() => _errorMessageRepository.GetString(code.ToString(), A<string[]>._)).Returns(expectedMessage);

    // Act
    var exception = _factory.Create<UnauthorizedException>(code, "CM Punk");

    // Assert
    Assert.IsType<UnauthorizedException>(exception);
    Assert.Equal(code, exception.Code);
    Assert.Equal(expectedMessage, exception.Message);
  }

  [Fact]
  public void Create_WithBadRequestExceptionType_ReturnsBadRequestException()
  {
    // Arrange
    const ErrorCode code = ErrorCode.UserCreationFailed;
    const string expectedMessage = "User creation failed";
    A.CallTo(() => _errorMessageRepository.GetString(code.ToString(), A<string[]>._)).Returns(expectedMessage);

    // Act
    var exception = _factory.Create<BadRequestException>(code);

    // Assert
    Assert.IsType<BadRequestException>(exception);
    Assert.Equal(code, exception.Code);
    Assert.Equal(expectedMessage, exception.Message);
    Assert.Equal([expectedMessage], exception.ErrorMessages);
    Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
  }

  [Fact]
  public void Create_WithUnsupportedType_ThrowsInvalidOperationException()
  {
    // Act & Assert
    var ex = Assert.Throws<InvalidOperationException>(() =>
      _factory.Create<CustomApiExceptionStub>(ErrorCode.UserCreationFailed)
    );

    Assert.Equal("Unsupported exception type CustomApiExceptionStub", ex.Message);
  }

  #endregion

  #region Helpers

  private class CustomApiExceptionStub() : ApiException(ErrorCode.UserCreationFailed, "stub");

  #endregion
}
