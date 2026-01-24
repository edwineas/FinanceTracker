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
  private readonly IRefreshTokenRepository _refreshTokenRepo;

  public AuthService(IAuthRepository repo, IPasswordHasher<User> passwordHasher, IConfiguration config, IRefreshTokenRepository refreshTokenRepo)
  {
    _repo = repo;
    _passwordHasher = passwordHasher;
    _config = config;
    _refreshTokenRepo = refreshTokenRepo;
  }

  public async Task<(bool Success, object? Error, string? AccessToken, string? RefreshToken)> RegisterUserAsync(RegisterUserRequest request)
  {
    if (await _repo.EmailExistsAsync(request.Email))
    {
      return (false, new {Email = new[] {"User already exists"}}, null, null);
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

    var accessToken = GenerateJwtToken(user);
    var refreshToken = GenerateRefreshToken();
    await _refreshTokenRepo.AddAsync(new RefreshToken { Token = refreshToken, UserId = user.Id, ExpiresAt = DateTime.UtcNow.AddDays(30), CreatedAt = DateTime.UtcNow });

    return (true, null, accessToken, refreshToken);
  }

  public async Task<(bool Success, string? AccessToken, string? RefreshToken)> LoginUserAsync(LoginUserRequest request)
  {
    var user = await _repo.GetUserByEmailAsync(request.Email);

    if (user == null) return (false, null, null);

    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
    if (result == PasswordVerificationResult.Failed) return (false, null, null);

    var accessToken = GenerateJwtToken(user);
    var refreshToken = GenerateRefreshToken();
    await _refreshTokenRepo.AddAsync(new RefreshToken { Token = refreshToken, UserId = user.Id, ExpiresAt = DateTime.UtcNow.AddDays(30), CreatedAt = DateTime.UtcNow });

    return (true, accessToken, refreshToken);
  }

  public async Task<(bool Success, string? AccessToken)> RefreshTokenAsync(string refreshToken)
  {
    var storedToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
    if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
    {
      return (false, null);
    }

    var user = await _repo.GetUserByIdAsync(storedToken.UserId);
    if (user == null) return (false, null);

    var newAccessToken = GenerateJwtToken(user);
    return (true, newAccessToken);
  }

  private string GenerateJwtToken(User user)
  {
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
    return tokenHandler.WriteToken(token);
  }

  private string GenerateRefreshToken() => Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
}
