using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinanceTracker.Application.DTOs.Request;
using FinanceTracker.Application.Services.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.Application.Services;

public class AuthService : IAuthService
{
  private readonly IAuthRepository _repo;
  private readonly IPasswordHasher<User> _passwordHasher;
  private readonly IConfiguration _config;

  public AuthService(IAuthRepository repo, IPasswordHasher<User> passwordHasher, IConfiguration config)
  {
    _repo = repo;
    _passwordHasher = passwordHasher;
    _config = config;
  }

  public async Task<(bool Success, object? Error)> RegisterUserAsync(RegisterUserRequest request)
  {
    if (await _repo.EmailExistsAsync(request.Email))
    {
      return (false, new {Email = "User already exists"});
    }

    var user = new User
    {
      Id = Guid.NewGuid(),
      Name = request.Name,
      Email = request.Email,
      CreatedAt = DateTime.UtcNow
    };

    user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

    await _repo.AddUserAsync(user);
    return (true, null);
  }

  public async Task<(bool Success, string? Token)> LoginUserAsync(LoginUserRequest request)
  {
    var user = await _repo.GetUserByEmailAsync(request.Email);

    if (user == null) return (false, null);

    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
    if (result == PasswordVerificationResult.Failed) return (false, null);

    var issuer = _config["Jwt:Issuer"];
    var audience = _config["Jwt:Audience"];
    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
      {
          new Claim("Id", Guid.NewGuid().ToString()),
          new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
      Expires = DateTime.UtcNow.AddMinutes(5),
      Issuer = issuer,
      Audience = audience,
      SigningCredentials =
        new SigningCredentials(
          new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
    };
    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return (true, tokenHandler.WriteToken(token));
  }
}
