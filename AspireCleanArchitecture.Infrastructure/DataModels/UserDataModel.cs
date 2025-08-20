namespace AspireCleanArchitecture.Infrastructure.DataModels;

public class UserDataModel
{
  public Guid Id { get; set; }
  public required string MailAddress { get; set; }
  public required string UserName { get; set; }
  public required string PasswordHash { get; set; }
}
