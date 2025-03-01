using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerAPI.Models;
using FluentValidation;

namespace ExpenseTrackerAPI.Controllers;

public class AuthController(AppDbContext dbContext, IConfiguration configuration) : BaseController
{
    private readonly AppDbContext _context = dbContext;
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Login to the application
    /// </summary>
    /// <param name="request">The username and password</param>
    /// <returns>A JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            return BadRequest();
        }

        if (!PasswordHelper.VerifyPassword(user.Password, request.Password))
        {
            return Unauthorized("Incorrect user or password!");
        }

        var token = GenerateJwtToken(new GetUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
        });

        return Ok(new LoginResponse { Token = token });
    }

    private string GenerateJwtToken(GetUserResponse user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "test_jwt_key"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginResponse
{
    public required string Token { get; set; }
}
