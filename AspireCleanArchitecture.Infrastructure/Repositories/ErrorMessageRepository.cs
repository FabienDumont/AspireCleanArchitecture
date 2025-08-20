using System.Globalization;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Infrastructure.Resources;

namespace AspireCleanArchitecture.Infrastructure.Repositories;

/// <summary>
/// Provides functionality for retrieving localized error messages.
/// </summary>
public class ErrorMessageRepository : IErrorMessageRepository
{
  #region Implementation of IErrorMessageRepository

  /// <inheritdoc />
  public string GetString(string key)
  {
    return ErrorMessageLocalization.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? $"[{key}]";
  }

  /// <inheritdoc />
  public string GetString(string key, params string[] args)
  {
    var message = ErrorMessageLocalization.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);

    if (string.IsNullOrEmpty(message)) return $"[{key}]";

    return args.Length == 0 ? message : string.Format(CultureInfo.CurrentUICulture, message, args);
  }

  #endregion
}
