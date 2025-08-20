using AspireCleanArchitecture.Domain;

namespace AspireCleanArchitecture.Application.Abstractions;

/// <summary>
/// Service interface for managing users.
/// </summary>
public interface IUserService
{
  /// <summary>
  /// Retrieves all users as a read-only collection asynchronously.
  /// </summary>
  /// <param name="cancellationToken">
  /// A token to observe while waiting for the task to complete.
  /// Can be used to cancel the operation.
  /// </param>
  /// <returns>
  /// A task that represents the asynchronous operation.
  /// The task result contains a read-only collection of <see cref="AspireCleanArchitecture.Domain.User"/> objects.
  /// </returns>
  Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken);

  /// <summary>
  /// Creates a new user asynchronously with the specified username and password.
  /// </summary>
  /// <param name="mailAddress">The mail address of the user to be created.</param>
  /// <param name="userName">
  /// The username of the user to be created.
  /// </param>
  /// <param name="password">
  /// The password for the user to be created.
  /// </param>
  /// <param name="cancellationToken">
  /// A token to observe while waiting for the task to complete.
  /// Can be used to cancel the operation.
  /// </param>
  /// <returns>
  /// A task that represents the asynchronous operation.
  /// The task result contains the newly created <see cref="AspireCleanArchitecture.Domain.User"/> object.
  /// </returns>
  Task<User> CreateAsync(string mailAddress, string userName, string password, CancellationToken cancellationToken);

  /// <summary>
  /// Authenticates a user asynchronously using the provided username and password.
  /// </summary>
  /// <param name="usernameOrMailAddress">
  /// The username or the mail address of the user attempting to log in.
  /// </param>
  /// <param name="password">
  /// The password associated with the specified username.
  /// </param>
  /// <param name="cancellationToken">
  /// A token to observe while waiting for the task to complete.
  /// Can be used to cancel the operation.
  /// </param>
  /// <returns>
  /// A task that represents the asynchronous operation.
  /// The task result contains the authenticated <see cref="AspireCleanArchitecture.Domain.User"/> instance if successful.
  /// </returns>
  Task<User> SignInAsync(string usernameOrMailAddress, string password, CancellationToken cancellationToken);

  /// <summary>
  /// Registers a new user in the system with the provided details asynchronously.
  /// </summary>
  /// <param name="mailAddress">
  /// The email address of the new user.
  /// </param>
  /// <param name="username">
  /// The username of the new user.
  /// </param>
  /// <param name="password">
  /// The password for the new user.
  /// </param>
  /// <param name="cancellationToken">
  /// A token to observe while waiting for the task to complete.
  /// Can be used to cancel the operation.
  /// </param>
  /// <returns>
  /// A task that represents the asynchronous operation.
  /// The task result contains the created <see cref="AspireCleanArchitecture.Domain.User"/> object,
  /// or null if the registration fails due to validation or other constraints.
  /// </returns>
  Task<User> SignUpAsync(string mailAddress, string username, string password, CancellationToken cancellationToken);
}
