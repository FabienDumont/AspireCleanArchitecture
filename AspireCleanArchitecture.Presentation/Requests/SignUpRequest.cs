namespace AspireCleanArchitecture.Presentation.Requests;

public class SignUpRequest
{
  public required string MailAddress { get; set; }
  public required string Username { get; set; }
  public required string Password { get; set; }
}
