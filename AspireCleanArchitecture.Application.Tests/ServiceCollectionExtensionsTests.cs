using AspireCleanArchitecture.Application.Abstractions;
using AspireCleanArchitecture.Application.Abstractions.Repositories;
using AspireCleanArchitecture.Application.Exceptions.Factories;
using AspireCleanArchitecture.Application.Services;
using AspireCleanArchitecture.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspireCleanArchitecture.Application.Tests;

public class ServiceCollectionExtensionsTests
{
  #region Methods

  [Fact]
  public void AddApplication_ShouldRegisterTraitServiceAsSingleton()
  {
    // Arrange
    IServiceCollection services = new ServiceCollection();
    var fakeUserRepository = A.Fake<IUserRepository>();
    services.AddSingleton(fakeUserRepository);
    var fakeErrorMessageRepository = A.Fake<IErrorMessageRepository>();
    services.AddSingleton(fakeErrorMessageRepository);
    var fakeUserManager = A.Fake<UserManager<User>>();
    services.AddSingleton(fakeUserManager);
    var fakeApiExceptionFactory = A.Fake<IApiExceptionFactory>();
    services.AddSingleton(fakeApiExceptionFactory);
    var fakeLogger = A.Fake<ILogger<UserService>>();
    services.AddSingleton(fakeLogger);

    // Act
    services.AddApplication();
    var provider = services.BuildServiceProvider();

    // Assert
    var service1 = provider.GetService<IUserService>();
    var service2 = provider.GetService<IUserService>();

    Assert.NotNull(service1);
    Assert.IsType<UserService>(service1);
    Assert.Same(service1, service2);
  }

  #endregion
}
