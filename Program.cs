using System.Text;
using FinanceTracker.Domain.Entities;
using FinanceTracker.API.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinanceTracker.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddValidation();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true
  };
});
builder.Services.AddAuthorization();

builder.Services.AddScoped<FinanceTracker.Infrastructure.Repositories.Interfaces.IAuthRepository, FinanceTracker.Infrastructure.Repositories.AuthRepository>();
builder.Services.AddScoped<FinanceTracker.Application.Services.Interfaces.IAuthService, FinanceTracker.Application.Services.AuthService>();
builder.Services.AddScoped<FinanceTracker.Infrastructure.Repositories.Interfaces.IAccountRepository, FinanceTracker.Infrastructure.Repositories.AccountRepository>();
builder.Services.AddScoped<FinanceTracker.Application.Services.Interfaces.IAccountService, FinanceTracker.Application.Services.AccountService>();
builder.Services.AddScoped<FinanceTracker.Infrastructure.Repositories.Interfaces.ICategoryRepository, FinanceTracker.Infrastructure.Repositories.CategoryRepository>();
builder.Services.AddScoped<FinanceTracker.Application.Services.Interfaces.ICategoryService, FinanceTracker.Application.Services.CategoryService>();
builder.Services.AddScoped<FinanceTracker.Infrastructure.Repositories.Interfaces.ITransactionRepository, FinanceTracker.Infrastructure.Repositories.TransactionRepository>();
builder.Services.AddScoped<FinanceTracker.Application.Services.Interfaces.ITransactionService, FinanceTracker.Application.Services.TransactionService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapAccountEndpoints();
app.MapCategoryEndpoints();
app.MapTransactionEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceTracker.Infrastructure.Data.AppDbContext>();
    db.Database.Migrate();
}

app.Run();
