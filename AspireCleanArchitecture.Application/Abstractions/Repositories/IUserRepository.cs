using AspireCleanArchitecture.Domain;

namespace AspireCleanArchitecture.Application.Abstractions.Repositories;

public interface IUserRepository
{
  /// <summary>
  /// Retrieves a user entity by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the user.</param>
  /// <param name="cancellationToken">A token to cancel the operation if required.</param>
  /// <returns>The user entity with the specified identifier, or null if not found.</returns>
  Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

  /// <summary>
  /// Retrieves a user entity by their normalized email address.
  /// </summary>
  /// <param name="normalizedMailAddress">The normalized email address of the user to retrieve.</param>
  /// <param name="cancellationToken">A token to cancel the operation if required.</param>
  /// <returns>The user entity with the specified normalized email address, or null if not found.</returns>
  Task<User?> GetByNormalizedMailAddressAsync(string normalizedMailAddress, CancellationToken cancellationToken);

  /// <summary>
  /// Retrieves a user entity by their normalized username.
  /// </summary>
  /// <param name="normalizedUserName">The normalized username of the user to retrieve.</param>
  /// <param name="cancellationToken">A token to cancel the operation if required.</param>
  /// <returns>The user entity with the specified username, or null if not found.</returns>
  Task<User?> GetByNormalizedUserNameAsync(string normalizedUserName, CancellationToken cancellationToken);

  /// <summary>
  /// Retrieves all user entities from the system.
  /// </summary>
  /// <param name="cancellationToken">A token to cancel the operation if required.</param>
  /// <returns>A read-only collection of user entities.</returns>
  Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken);

  /// <summary>
  /// Creates a new user in the system.
  /// </summary>
  /// <param name="domain">The user entity to be created.</param>
  /// <param name="cancellationToken">A token to cancel the operation if required.</param>
  /// <returns>The created user entity.</returns>
  Task<User> CreateAsync(User domain, CancellationToken cancellationToken);

  /// <summary>
  /// Updates an existing user entity in the system.
  /// </summary>
  /// <param name="domain">The user entity with updated details.</param>
  /// <param name="cancellationToken">A token to cancel the operation if required.</param>
  /// <returns>The updated user entity.</returns>
  Task<User> UpdateAsync(User domain, CancellationToken cancellationToken);

  /// <summary>
  /// Deletes the specified user entity from the system.
  /// </summary>
  /// <param name="domain">The user entity to be deleted.</param>
  /// <param name="cancellationToken">A token to cancel the operation if required.</param>
  /// <returns>A Task representing the asynchronous operation.</returns>
  Task DeleteAsync(User domain, CancellationToken cancellationToken);
}
