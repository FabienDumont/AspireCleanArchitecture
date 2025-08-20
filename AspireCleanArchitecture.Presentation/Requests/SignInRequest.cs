namespace AspireCleanArchitecture.Presentation.Requests;

public class SignInRequest
{
  public required string UsernameOrMailAddress { get; set; }
  public required string Password { get; set; }
}
