using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinanceTracker.Infrasturecture.Data;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.API.Endpoints;

public static class AuthEndpoints
{
  public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/auth");

    group.MapPost("/register", async (RegisterUserRequest newUser, AppDbContext db, IPasswordHasher<User> passwordHasher) =>
    {
      var exists = await db.Users.AnyAsync(user => user.Email == newUser.Email);
      if (exists)
      {
        return Results.Conflict("User already exists");
      }

      var user = new User
      {
        Id = Guid.NewGuid(),
        Name = newUser.Name,
        Email = newUser.Email,
        CreatedAt = DateTime.UtcNow
      };

      user.PasswordHash = passwordHasher.HashPassword(user, newUser.Password);

      db.Users.Add(user);
      await db.SaveChangesAsync();

      return Results.Created();

    });

    group.MapPost("/login", async (LoginUserRequest loginUser, AppDbContext db, IPasswordHasher<User> passwordHasher, IConfiguration config) =>
    {
      var user = await db.Users.FirstOrDefaultAsync(user => user.Email == loginUser.Email);
      if (user == null)
      {
        return Results.Unauthorized();
      }


      var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUser.Password);
      if (result == PasswordVerificationResult.Failed)
      {
        return Results.Unauthorized();
      }


      var issuer = config["Jwt:Issuer"];
      var audience = config["Jwt:Audience"];
      var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
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
      return Results.Ok(new { accessToken = tokenHandler.WriteToken(token) });
    });

    return app;
  }
}
