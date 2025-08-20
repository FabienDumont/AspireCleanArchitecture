using AspireCleanArchitecture.Application.Abstractions;
using AspireCleanArchitecture.Application.Exceptions.Factories;
using AspireCleanArchitecture.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspireCleanArchitecture.Application;

/// <summary>
/// Provides extension methods for configuring application-specific services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
  #region Methods

  /// <summary>
  /// Registers application services for dependency injection.
  /// </summary>
  /// <param name="services">
  /// The <see cref="IServiceCollection"/> to which the application services will be added.
  /// </param>
  /// <returns>
  /// The <see cref="IServiceCollection"/> with the application services registered.
  /// </returns>
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<IUserService, UserService>();

    services.AddScoped<IApiExceptionFactory, ApiExceptionFactory>();

    return services;
  }

  #endregion
}
