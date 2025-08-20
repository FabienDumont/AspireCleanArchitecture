using System.Data;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Infrastructure.Abstraction;
using AspireCleanArchitecture.Infrastructure.InternalUtilities;
using AspireCleanArchitecture.Infrastructure.Repositories;
using AspireCleanArchitecture.Infrastructure.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AspireCleanArchitecture.Infrastructure;

/// <summary>
/// Provides extension methods for configuring infrastructure-specific services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
  #region Methods

  /// <summary>
  /// Registers infrastructure services for dependency injection.
  /// </summary>
  /// <param name="services">
  /// The <see cref="IServiceCollection"/> to which the infrastructure services will be added.
  /// </param>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>
  /// The <see cref="IServiceCollection"/> with the infrastructure services registered.
  /// </returns>
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
  {
    services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

    services.Configure<IdentityOptions>(_ => { });
    services.AddLogging();
    services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
    services.AddSingleton<IUserValidator<User>, UserValidator<User>>();
    services.AddSingleton<IPasswordValidator<User>, PasswordValidator<User>>();
    services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User>>();
    services.AddSingleton<IdentityErrorDescriber>();
    services.AddScoped<UserManager<User>>();

    services.AddScoped<ISqlExecutor, DapperSqlExecutor>();

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IErrorMessageRepository, ErrorMessageRepository>();

    services.AddScoped<IUserStore<User>, UserStore>();

    return services;
  }

  #endregion
}
