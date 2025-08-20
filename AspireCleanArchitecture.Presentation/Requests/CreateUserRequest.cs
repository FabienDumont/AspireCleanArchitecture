namespace AspireCleanArchitecture.Presentation.Requests;

public class CreateUserRequest
{
  public string MailAddress { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
}
