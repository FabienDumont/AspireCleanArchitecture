namespace AspireCleanArchitecture.Domain;

/// <summary>
///   Represents a user in the system with authentication details.
/// </summary>
public class User
{
  #region Properties

  /// <summary>
  ///   Gets the identifier of the user.
  /// </summary>
  public Guid Id { get; private set; }

  /// <summary>
  ///   Gets the mail address of the user.
  /// </summary>
  public string MailAddress { get; private set; }

  /// <summary>
  ///   Gets the normalized mail address of the user.
  /// </summary>
  public string NormalizedMailAddress { get; private set; }

  /// <summary>
  ///   Gets the username of the user.
  /// </summary>
  public string UserName { get; private set; }

  /// <summary>
  ///   Gets the normalized username of the user.
  /// </summary>
  public string NormalizedUserName { get; private set; }

  /// <summary>
  ///   Gets the hashed password of the user.
  /// </summary>
  public string? PasswordHash { get; private set; }

  #endregion

  #region Ctors

  /// <summary>
  /// Private constructor used internally.
  /// </summary>
  private User(Guid id, string mailAddress, string userName, string? passwordHash)
  {
    Id = id;
    MailAddress = mailAddress;
    NormalizedMailAddress = mailAddress.ToUpperInvariant();
    UserName = userName;
    NormalizedUserName = userName.ToUpperInvariant();
    PasswordHash = passwordHash;
  }

  #endregion

  #region Methods

  /// <summary>
  /// Factory method to create a new instance.
  /// </summary>
  public static User Create(string mailAddress, string userName, string? passwordHash = null)
  {
    return new User(Guid.NewGuid(), mailAddress, userName, passwordHash);
  }

  /// <summary>
  /// Factory method to load an existing instance from persistence.
  /// </summary>
  public static User Load(Guid id, string mailAddress, string userName, string passwordHash)
  {
    return new User(id, mailAddress, userName, passwordHash);
  }

  /// <summary>
  /// Updates the user's mail address with a new value.
  /// </summary>
  /// <param name="mailAddress">The new mail address value to be set.</param>
  public void UpdateMailAddress(string mailAddress)
  {
    MailAddress = mailAddress;
    NormalizedMailAddress = mailAddress.ToUpperInvariant();
  }

  /// <summary>
  /// Updates the user's normalized mail address with a new value.
  /// </summary>
  /// <param name="normalizedMailAddress">The new normalized mail address to be set.</param>
  public void UpdateNormalizedMailAddress(string normalizedMailAddress)
  {
    NormalizedMailAddress = normalizedMailAddress;
  }

  /// <summary>
  /// Updates the user's username with a new value.
  /// </summary>
  /// <param name="userName">The new username value to be set.</param>
  public void UpdateUserName(string userName)
  {
    UserName = userName;
    NormalizedUserName = userName.ToUpperInvariant();
  }

  /// <summary>
  /// Updates the user's normalized username with a new value.
  /// </summary>
  /// <param name="normalizedUserName">The new normalized username to be set.</param>
  public void UpdateNormalizedUserName(string normalizedUserName)
  {
    NormalizedUserName = normalizedUserName;
  }

  /// <summary>
  /// Updates the user's password hash with a new value.
  /// </summary>
  /// <param name="passwordHash">The new password hash value to be set.</param>
  public void UpdatePasswordHash(string passwordHash)
  {
    PasswordHash = passwordHash;
  }

  #endregion
}
