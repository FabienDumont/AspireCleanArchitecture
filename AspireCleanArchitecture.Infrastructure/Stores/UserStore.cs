using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Domain;
using Microsoft.AspNetCore.Identity;

namespace AspireCleanArchitecture.Infrastructure.Stores;

public class UserStore(IUserRepository userRepository) : IUserPasswordStore<User>, IUserEmailStore<User>
{
  #region Implementation of IUserPasswordStore<User>

  /// <inheritdoc />
  public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
  {
    user.UpdatePasswordHash(passwordHash);
    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(user.PasswordHash);
  }

  /// <inheritdoc />
  public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
  }

  #endregion

  #region Implementation of IUserStore<User>

  /// <inheritdoc />
  public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(user.Id.ToString());
  }

  /// <inheritdoc />
  public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(user.UserName);
  }

  /// <inheritdoc />
  public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
  {
    user.UpdateUserName(userName);
    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(user.NormalizedUserName);
  }

  /// <inheritdoc />
  public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
  {
    user.UpdateNormalizedUserName(normalizedName);
    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
  {
    try
    {
      await userRepository.CreateAsync(user, cancellationToken).ConfigureAwait(false);
      return IdentityResult.Success;
    }
    catch (Exception e)
    {
      return IdentityResult.Failed(new IdentityError {Code = "USER_CREATE_FAILED", Description = e.Message});
    }
  }

  /// <inheritdoc />
  public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
  {
    try
    {
      await userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
      return IdentityResult.Success;
    }
    catch (Exception e)
    {
      return IdentityResult.Failed(new IdentityError {Code = "USER_UPDATE_FAILED", Description = e.Message});
    }
  }

  /// <inheritdoc />
  public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
  {
    try
    {
      await userRepository.DeleteAsync(user, cancellationToken).ConfigureAwait(false);
      return IdentityResult.Success;
    }
    catch (Exception e)
    {
      return IdentityResult.Failed(new IdentityError {Code = "USER_DELETE_FAILED", Description = e.Message});
    }
  }

  /// <inheritdoc />
  public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
  {
    if (!Guid.TryParse(userId, out var id)) return null!;

    var user = await userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    return user!;
  }

  /// <inheritdoc />
  public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
  {
    var user = await userRepository.GetByNormalizedUserNameAsync(normalizedUserName, cancellationToken)
      .ConfigureAwait(false);
    return user!;
  }

  #endregion

  #region Implementation of IUserEmailStore<User>

  public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
  {
    user.UpdateMailAddress(email);
    return Task.CompletedTask;
  }

  public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(user.MailAddress);
  }

  public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(true);
  }

  public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
  {
    var user = await userRepository.GetByNormalizedMailAddressAsync(normalizedEmail, cancellationToken);
    return user;
  }

  public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
  {
    return Task.FromResult(user.NormalizedMailAddress);
  }

  public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
  {
    user.UpdateNormalizedMailAddress(normalizedEmail!);
    return Task.CompletedTask;
  }

  #endregion

  #region Implementation of IDisposable

  /// <inheritdoc />
  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }

  #endregion
}
