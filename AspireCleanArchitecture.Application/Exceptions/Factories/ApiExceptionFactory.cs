using AspireCleanArchitecture.Application.Abstractions.Repositories;

namespace AspireCleanArchitecture.Application.Exceptions.Factories;

/// <summary>
/// A factory class responsible for creating API exception instances.
/// </summary>
/// <remarks>
/// This class uses the specified <c>IErrorMessageRepository</c> to retrieve
/// localized error messages and constructs instances of various API exception
/// types based on the provided error code and optional message arguments.
/// </remarks>
public class ApiExceptionFactory(IErrorMessageRepository errorMessageRepository) : IApiExceptionFactory
{
  #region Implementation of IApiExceptionFactory

  /// <inheritdoc />
  public TException Create<TException>(ErrorCode code, params string[] messageArgs) where TException : ApiException
  {
    var message = errorMessageRepository.GetString(code.ToString(), messageArgs);

    ApiException result = typeof(TException) switch
    {
      var t when t == typeof(ConflictException) => new ConflictException(code, message),
      var t when t == typeof(UnauthorizedException) => new UnauthorizedException(code, message),
      var t when t == typeof(BadRequestException) => new BadRequestException(code, message),
      _ => throw new InvalidOperationException($"Unsupported exception type {typeof(TException).Name}")
    };

    return (TException) result;
  }

  #endregion
}
