namespace AspireCleanArchitecture.Presentation.ViewModels;

public class LogInViewModel(UserViewModel user, string bearerToken)
{
  #region Properties

  public UserViewModel User { get; set; } = user;
  public string BearerToken { get; set; } = bearerToken;

  #endregion
}
