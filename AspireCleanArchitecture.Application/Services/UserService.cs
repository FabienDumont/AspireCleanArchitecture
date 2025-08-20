using AspireCleanArchitecture.Application.Abstractions;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Application.Exceptions;
using AspireCleanArchitecture.Application.Exceptions.Factories;
using AspireCleanArchitecture.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AspireCleanArchitecture.Application.Services;

/// <summary>
/// Service for managing users.
/// </summary>
public class UserService(
  UserManager<User> userManager, IUserRepository userRepository, IApiExceptionFactory apiExceptionFactory,
  ILogger<UserService> logger
) : IUserService
{
  #region Implementation of IUserService

  /// <inheritdoc />
  public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken)
  {
    return await userRepository.GetAllAsync(cancellationToken);
  }

  /// <inheritdoc />
  public async Task<User> CreateAsync(
    string mailAddress, string userName, string password, CancellationToken cancellationToken
  )
  {
    logger.LogDebug("USER - Creating user {mailAddress} {UserName}.", mailAddress, userName);

    var existingUserByUsername = await userManager.FindByNameAsync(userName).ConfigureAwait(false);
    if (existingUserByUsername is not null)
    {
      throw apiExceptionFactory.Create<ConflictException>(ErrorCode.UsernameAlreadyExists, userName);
    }

    var existingUserByEmail = await userManager.FindByEmailAsync(mailAddress).ConfigureAwait(false);
    if (existingUserByEmail is not null)
    {
      throw apiExceptionFactory.Create<ConflictException>(ErrorCode.MailAddressAlreadyExists, mailAddress);
    }

    var user = User.Create(mailAddress.ToLower(), userName);
    var result = await userManager.CreateAsync(user, password).ConfigureAwait(false);

    if (!result.Succeeded)
    {
      throw apiExceptionFactory.Create<BadRequestException>(
        ErrorCode.UserCreationFailed, result.Errors.Select(e => e.Description).ToArray()
      );
    }

    var createdUser = await userManager.FindByIdAsync(user.Id.ToString()).ConfigureAwait(false);
    return createdUser;
  }

  /// <inheritdoc />
  public async Task<User> SignInAsync(
    string usernameOrMailAddress, string password, CancellationToken cancellationToken
  )
  {
    logger.LogDebug(
      "USER - Trying to login with the {UsernameOrMailAddress} username (or mail address).", usernameOrMailAddress
    );

    var user = await userManager.FindByNameAsync(usernameOrMailAddress) ??
               await userManager.FindByEmailAsync(usernameOrMailAddress);

    if (user is null || !await userManager.CheckPasswordAsync(user, password))
    {
      throw apiExceptionFactory.Create<UnauthorizedException>(ErrorCode.InvalidCredentials, usernameOrMailAddress);
    }

    return user;
  }

  /// <inheritdoc />
  public async Task<User> SignUpAsync(
    string mailAddress, string username, string password, CancellationToken cancellationToken
  )
  {
    logger.LogDebug(
      "USER - Trying to sign up with the {Username} username and the {MailAddress} mail address.", username, mailAddress
    );

    var user = await userManager.FindByNameAsync(username).ConfigureAwait(false);

    if (user is not null)
    {
      throw apiExceptionFactory.Create<ConflictException>(ErrorCode.UsernameAlreadyExists, username);
    }

    user = await userManager.FindByEmailAsync(mailAddress).ConfigureAwait(false);

    if (user is not null)
    {
      throw apiExceptionFactory.Create<ConflictException>(ErrorCode.MailAddressAlreadyExists, username);
    }

    user = User.Create(mailAddress, username, password);

    var result = await userManager.CreateAsync(user, password).ConfigureAwait(false);

    if (result.Succeeded)
    {
      return user;
    }

    var errorMessages = string.Join(" / ", result.Errors.Select(e => e.Description));

    throw apiExceptionFactory.Create<BadRequestException>(ErrorCode.UserCreationFailed, username, errorMessages);
  }

  #endregion
}
