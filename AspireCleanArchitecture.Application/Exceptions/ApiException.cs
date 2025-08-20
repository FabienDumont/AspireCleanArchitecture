using System.Net;

namespace AspireCleanArchitecture.Application.Exceptions;

public abstract class ApiException(
  ErrorCode code, string message, IReadOnlyCollection<string>? errorMessages = null,
  HttpStatusCode statusCode = HttpStatusCode.InternalServerError
) : Exception(message)
{
  #region Properties

  public ErrorCode Code { get; } = code;

  /// <summary>
  ///   Gets the error messages.
  /// </summary>
  /// <value>
  ///   The error messages.
  /// </value>
  public IReadOnlyCollection<string> ErrorMessages { get; } = errorMessages ?? [];

  /// <summary>
  ///   Gets the status code.
  /// </summary>
  /// <value>
  ///   The status code.
  /// </value>
  public HttpStatusCode StatusCode { get; } = statusCode;

  #endregion
}
