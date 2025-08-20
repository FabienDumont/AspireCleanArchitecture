namespace AspireCleanArchitecture.Presentation.ViewModels;

/// <summary>
/// Represents a view model for the user, containing essential information
/// such as ID, mail address and username for presentation purposes.
/// </summary>
public class UserViewModel(Guid id, string mailAddress, string userName)
{
  #region Properties

  /// <summary>
  ///   Gets the unique identifier of the user.
  /// </summary>
  public Guid Id { get; } = id;

  /// <summary>
  ///   Gets the mail address of the user.
  /// </summary>
  public string MailAddress { get; } = mailAddress;

  /// <summary>
  ///   Gets the username of the user.
  /// </summary>
  public string UserName { get; } = userName;

  #endregion
}
