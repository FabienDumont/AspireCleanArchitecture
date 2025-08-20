var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithDataVolume(isReadOnly: false)
  .WithLifetime(ContainerLifetime.Persistent).WithPgWeb(pg => pg.WithHostPort(5050));
var postgresdb = postgres.AddDatabase("postgresdb");

var apiService = builder.AddProject<Projects.AspireCleanArchitecture_Presentation>("apiservice")
  .WithHttpHealthCheck("/health").WithReference(postgresdb);

builder.AddProject<Projects.AspireCleanArchitecture_Web>("webfrontend").WithExternalHttpEndpoints()
  .WithHttpHealthCheck("/health").WithReference(apiService).WaitFor(apiService);

builder.Build().Run();
