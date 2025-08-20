using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspireCleanArchitecture.Application.Abstractions;
using AspireCleanArchitecture.Presentation.Mappers;
using AspireCleanArchitecture.Presentation.Requests;
using AspireCleanArchitecture.Presentation.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspireCleanArchitecture.Presentation.Controllers;

[Route("api/users")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController(IConfiguration configuration, IUserService userService) : ControllerBase
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<ActionResult<IEnumerable<UserViewModel>>> GetAllAsync()
  {
    var users = await userService.GetAllAsync(CancellationToken.None);

    return Ok(users.ToViewModelCollection());
  }

  [HttpPost("signin", Name = "SignIn")]
  [AllowAnonymous]
  public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
  {
    var user = await userService.SignInAsync(request.UsernameOrMailAddress, request.Password, CancellationToken.None);

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Email, user.MailAddress),
      new Claim(ClaimTypes.Name, user.UserName)
    };

    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found"))
    );

    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      claims: claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Ok(new LogInViewModel(user.ToViewModel(), tokenString));
  }

  [HttpPost("signup", Name = "SignUpTest")]
  [AllowAnonymous]
  public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
  {
    var user = await userService.SignUpAsync(
      request.MailAddress, request.Username, request.Password, CancellationToken.None
    );

    return Ok(user.ToViewModel());
  }

  [HttpPost]
  public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request)
  {
    var user = await userService.CreateAsync(
      request.MailAddress, request.Username, request.Password, CancellationToken.None
    );

    return Ok(user.ToViewModel());
  }
}
