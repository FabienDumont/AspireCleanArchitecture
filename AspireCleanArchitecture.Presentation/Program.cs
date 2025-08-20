using System.Globalization;
using System.Text;
using AspireCleanArchitecture.Application;
using AspireCleanArchitecture.Application.Exceptions;
using AspireCleanArchitecture.Infrastructure;
using AspireCleanArchitecture.ServiceDefaults;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("postgresdb")
         ?? throw new InvalidOperationException("Missing ConnectionStrings:postgresdb");

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
  {
    options.AddDocumentTransformer((document, _, _) =>
      {
        var securityScheme = new OpenApiSecurityScheme
        {
          Type = SecuritySchemeType.Http,
          In = ParameterLocation.Header,
          Scheme = "bearer"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("BearerAuth", securityScheme);

        document.SecurityRequirements.Add(
          new OpenApiSecurityRequirement
          {
            [new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Id = "BearerAuth",
                Type = ReferenceType.SecurityScheme
              }
            }] = new List<string>()
          }
        );

        return Task.CompletedTask;
      }
    );
  }
);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddInfrastructure(connectionString);

builder.Services.AddApplication();

var key = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key not configured");

builder.Services.AddAuthentication("Bearer").AddJwtBearer(
  "Bearer", options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = false,
      ValidateAudience = false,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
      ClockSkew = TimeSpan.Zero
    };
  }
);

builder.Services.AddCors(options =>
  {
    options.AddDefaultPolicy(policy =>
      {
        policy.WithOrigins("http://localhost:5278", "https://localhost:7191").AllowAnyHeader().AllowAnyMethod()
          .AllowCredentials();
        ;
      }
    );
  }
);

var supportedCultures = new[] {"en-US", "fr-FR"};
builder.Services.Configure<RequestLocalizationOptions>(options =>
  {
    var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToArray();
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    options.RequestCultureProviders.Insert(
      0, new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider()
    );
  }
);

builder.WebHost.ConfigureKestrel(_ => { }).UseSetting(WebHostDefaults.DetailedErrorsKey, "true");

builder.Services.AddAuthorization();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseForwardedHeaders(
  new ForwardedHeadersOptions
  {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
  }
);

app.UseHttpsRedirection();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler(errorApp =>
  {
    errorApp.Run(async context =>
      {
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        if (error is ApiException apiEx)
        {
          context.Response.StatusCode = (int) apiEx.StatusCode;
          await context.Response.WriteAsJsonAsync(
            new
            {
              code = apiEx.Code,
              message = apiEx.Message,
              details = apiEx.ErrorMessages
            }
          );
        }
        else
        {
          context.Response.StatusCode = 500;
          await context.Response.WriteAsJsonAsync(new {message = "Unexpected error occurred."});
        }
      }
    );
  }
);

app.MapDefaultEndpoints();

app.Run();
