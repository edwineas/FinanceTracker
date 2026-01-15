using FinanceTracker.Data;
using FinanceTracker.Data.Models;
using FinanceTracker.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Endpoints;

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

    return app;
  }
}
