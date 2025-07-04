using System.Security.Claims;
using System.Text;
using Mazad.Api.Infrastrcture;
using Mazad.Core.Domain.Users.Authentication;
using Mazad.Core.Shared.Contexts;
using Mazad.UseCases.UsersDomain.Otp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.Configure<OtpServiceSettings>(
    builder.Configuration.GetSection("OtpServiceSettings")
);

builder.Services.AddSingleton<IOtpService, OtpService>();

builder.Services.AddScoped<ISaveChangesInterceptor, BaseEntityInterceptor>();

builder.Services.AddDbContext<MazadDbContext>(
    (serviceProvider, options) =>
        options
            .UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>())
);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<DelayMiddleware>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mazad API Documentation", Version = "v1" });

    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );

    c.OperationFilter<AcceptLanguageHeaderFilter>();
});

builder.Services.AddScoped<JwtService>();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null)
        {
            throw new InvalidOperationException("JwtSettings section not found in configuration.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(jwtSettings.Secret)
            ),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User"));

    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<DelayMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();

public class DelayMiddleware : IMiddleware
{
    private readonly int _delay = 500;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await Task.Delay(_delay);
        await next(context);
    }
}
