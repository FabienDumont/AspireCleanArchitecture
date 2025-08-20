using AspireCleanArchitecture.Application.InternalUtilities;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Infrastructure.DataModels;

namespace AspireCleanArchitecture.Infrastructure.Mappers;

public static class UserMapper
{
  #region Methods

  /// <summary>
  /// Maps an EF data model to its domain counterpart.
  /// </summary>
  public static User ToDomain(this UserDataModel dataModel)
  {
    return dataModel.Map(i => User.Load(i.Id, i.MailAddress, i.UserName, i.PasswordHash));
  }

  /// <summary>
  /// Maps a collection of EF data models to domain models.
  /// </summary>
  public static List<User> ToDomainCollection(this IEnumerable<UserDataModel> dataModels)
  {
    return dataModels.MapCollection(ToDomain);
  }

  #endregion
}
