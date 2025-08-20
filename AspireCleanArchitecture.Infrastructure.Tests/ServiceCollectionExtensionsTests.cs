using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Domain;
using AspireCleanArchitecture.Infrastructure.Repositories;
using AspireCleanArchitecture.Infrastructure.Stores;

namespace AspireCleanArchitecture.Infrastructure.Tests;

public class ServiceCollectionExtensionsTests
{
  #region Methods

  [Fact]
  public void AddInfrastructure_RegistersExpectedServices()
  {
    // Arrange
    var services = new ServiceCollection();

    services.AddInfrastructure("Host=fakehost;Database=fakedb;");

    // Register UserManager dependencies
    services.Configure<IdentityOptions>(_ => { });
    services.AddLogging();
    services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
    services.AddSingleton<IUserValidator<User>, UserValidator<User>>();
    services.AddSingleton<IPasswordValidator<User>, PasswordValidator<User>>();
    services.AddSingleton<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User>>();

    var provider = services.BuildServiceProvider();

    // Act & Assert
    var dbConnection = provider.GetRequiredService<IDbConnection>();
    Assert.IsType<NpgsqlConnection>(dbConnection);
    Assert.Equal("Host=fakehost;Database=fakedb;", dbConnection.ConnectionString);

    Assert.IsType<UserRepository>(provider.GetRequiredService<IUserRepository>());
    Assert.IsType<UserStore>(provider.GetRequiredService<IUserStore<User>>());

    var userManager = provider.GetService<UserManager<User>>();
    Assert.NotNull(userManager);
  }

  #endregion
}
