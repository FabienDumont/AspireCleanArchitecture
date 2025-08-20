using AspireCleanArchitecture.Application.InternalUtilities;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Presentation.ViewModels;

namespace AspireCleanArchitecture.Presentation.Mappers;

/// <summary>
/// Provides methods to map User domain entities to UserViewModel objects.
/// </summary>
public static class UserMapper
{
  #region Methods

  /// <summary>
  /// Maps a <see cref="User"/> object to a <see cref="UserViewModel"/> object.
  /// </summary>
  /// <param name="domain">The <see cref="User"/> instance to map from.</param>
  /// <returns>A <see cref="UserViewModel"/> instance populated with data from the provided <see cref="User"/>.</returns>
  public static UserViewModel ToViewModel(this User domain)
  {
    return domain.Map(u => new UserViewModel(u.Id, u.MailAddress, u.UserName));
  }

  /// <summary>
  /// Converts a collection of <see cref="User"/> objects into a collection of <see cref="UserViewModel"/> objects.
  /// </summary>
  /// <param name="domains">The collection of <see cref="User"/> instances to map from.</param>
  /// <returns>A collection of <see cref="UserViewModel"/> objects, each populated with data from the corresponding <see cref="User"/>.</returns>
  public static List<UserViewModel> ToViewModelCollection(this IEnumerable<User> domains)
  {
    return domains.MapCollection(ToViewModel);
  }

  #endregion
}
