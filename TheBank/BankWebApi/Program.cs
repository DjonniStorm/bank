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
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
using var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());
builder.Services.AddSingleton(loggerFactory.CreateLogger("Any"));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });

    // Включение XML-комментариев (если они есть)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    // Поддержка JWT-аутентификации в Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
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
builder.Services.AddTransient<IPeriodBusinessLogicContract, PeriodBusinessLogicContract>();
builder.Services.AddTransient<IDepositBusinessLogicContract, DepositBusinessLogicContract>();
builder.Services.AddTransient<IClientBusinessLogicContract, ClientBusinessLogicContract>();
builder.Services.AddTransient<ICreditProgramBusinessLogicContract, CreditProgramBusinessLogicContract>();
builder.Services.AddTransient<ICurrencyBusinessLogicContract, CurrencyBusinessLogicContract>();
builder.Services.AddTransient<IStorekeeperBusinessLogicContract, StorekeeperBusinessLogicContract>();
builder.Services.AddTransient<IReplenishmentBusinessLogicContract, ReplenishmentBusinessLogicContract>();
// бд
builder.Services.AddTransient<BankDbContext>();
builder.Services.AddTransient<IClerkStorageContract, ClerkStorageContract>();
builder.Services.AddTransient<IPeriodStorageContract, PeriodStorageContract>();
builder.Services.AddTransient<IDepositStorageContract, DepositStorageContract>();
builder.Services.AddTransient<IClientStorageContract, ClientStorageContract>();
builder.Services.AddTransient<ICreditProgramStorageContract, CreditProgramStorageContract>();
builder.Services.AddTransient<ICurrencyStorageContract, CurrencyStorageContract>();
builder.Services.AddTransient<IStorekeeperStorageContract, StorekeeperStorageContract>();
builder.Services.AddTransient<IReplenishmentStorageContract, ReplenishmentStorageContract>();
// адаптеры
builder.Services.AddTransient<IClerkAdapter, ClerkAdapter>();
builder.Services.AddTransient<IPeriodAdapter, PeriodAdapter>();
builder.Services.AddTransient<IDepositAdapter, DepositAdapter>();
builder.Services.AddTransient<IClientAdapter, ClientAdapter>();
builder.Services.AddTransient<ICreditProgramAdapter, CreditProgramAdapter>();
builder.Services.AddTransient<ICurrencyAdapter, CurrencyAdapter>();
builder.Services.AddTransient<IStorekeeperAdapter, StorekeeperAdapter>();
builder.Services.AddTransient<IReplenishmentAdapter, ReplenishmentAdapter>();

builder.Services.AddTransient<IReportContract, ReportContract>();
builder.Services.AddTransient<IReportAdapter, ReportAdapter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank API V1");
        c.RoutePrefix = "swagger"; // Swagger UI будет доступен по /swagger
    });
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
app.UseAuthentication();
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
