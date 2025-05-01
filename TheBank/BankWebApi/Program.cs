using BankBusinessLogic.Implementations;
using BankContracts.AdapterContracts;
using BankContracts.BusinessLogicContracts;
using BankContracts.Infrastructure;
using BankContracts.StorageContracts;
using BankDatabase;
using BankDatabase.Implementations;
using BankWebApi;
using BankWebApi.Adapters;
using BankWebApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
using var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());
builder.Services.AddSingleton(loggerFactory.CreateLogger("Any"));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,

            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,

            ValidAudience = AuthOptions.AUDIENCE,

            ValidateLifetime = true,

            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

            ValidateIssuerSigningKey = true,
        };
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IConfigurationDatabase, ConfigurationDatabase>();
// бизнес логика
builder.Services.AddTransient<IClerkBusinessLogicContract, ClerkBusinessLogicContract>();
// бд
builder.Services.AddTransient<BankDbContext>();
builder.Services.AddTransient<IClerkStorageContract, ClerkStorageContract>();
// адаптеры
builder.Services.AddTransient<IClerkAdapter, ClerkAdapter>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
if (app.Environment.IsProduction())
{
    var dbContext = app.Services.GetRequiredService<BankDbContext>();
    if (dbContext.Database.CanConnect())
    {
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Map("/login/{username}", (string username) =>
{
    return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: [new(ClaimTypes.Name, username)],
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)));
});

app.MapControllers();

app.Run();
