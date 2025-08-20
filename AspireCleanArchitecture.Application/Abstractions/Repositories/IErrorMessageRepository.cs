namespace AspireCleanArchitecture.Application.Abstractions.Repositories;

/// <summary>
/// Provides methods for retrieving localized error messages.
/// </summary>
public interface IErrorMessageRepository
{
  /// <summary>
  /// Retrieves a localized string associated with the specified key.
  /// </summary>
  /// <param name="key">The unique identifier for the localized resource.</param>
  /// <returns>The localized string if found; otherwise, a fallback or default value.</returns>
  public string GetString(string key);

  /// <summary>
  /// Retrieves a formatted localized string associated with the specified key, using the provided arguments.
  /// </summary>
  /// <param name="key">The unique identifier for the localized resource.</param>
  /// <param name="args">Optional arguments to format the localized string (e.g., placeholders).</param>
  /// <returns>
  /// The formatted localized string if the key is found; otherwise, a fallback or default value.
  /// </returns>
  public string GetString(string key, params string[] args);
}
