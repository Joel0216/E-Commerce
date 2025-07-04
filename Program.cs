using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Middleware;
using Infrastructure.Data;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Application.Services.Interfaces;
using Application.Services.Implementations;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// 🔐 JWT Settings (con protección contra valores nulos)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// 💾 DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧠 Dependencias
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// 🔐 Controladores, Swagger + Token
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "E-Commerce", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Token JWT (usa: 'Bearer {tu_token}')",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

// 🌐 CORS antes del middleware
app.UseCors("AllowAll");

// 🛡️ Middleware de IP autorizada
app.Use(async (context, next) =>
{
    var allowedIp = builder.Configuration["AllowedIP"];
    var environment = builder.Environment.EnvironmentName;
    
    // Obtener IP del cliente de múltiples fuentes
    var remoteIp = context.Connection.RemoteIpAddress?.ToString();
    var forwardedIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
    var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
    
    // Usar la IP más apropiada
    var clientIp = realIp ?? forwardedIp ?? remoteIp ?? "unknown";
    
    // Log para debugging
    Console.WriteLine($"🌐 IP del cliente: {clientIp}");
    Console.WriteLine($"🔒 IP autorizada: {allowedIp}");
    Console.WriteLine($"🏗️ Entorno: {environment}");
    
    // En desarrollo, permitir localhost
    if (environment == "Development" && (clientIp == "127.0.0.1" || clientIp == "::1" || clientIp == "localhost"))
    {
        Console.WriteLine("✅ Acceso permitido (desarrollo local)");
        await next();
        return;
    }
    
    if (clientIp != allowedIp)
    {
        Console.WriteLine("❌ Acceso denegado - IP no autorizada");
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Message = "Acceso denegado",
            Error = "Tu IP no está autorizada para acceder a esta API",
            YourIP = clientIp,
            AllowedIP = allowedIp,
            Environment = environment
        });
        return;
    }

    Console.WriteLine("✅ Acceso permitido");
    await next();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 🛡️ Middleware de manejo de excepciones
app.UseMiddleware<WebApi.Middleware.ExceptionHandlingMiddleware>();

app.MapControllers(); // ✅ Solo una vez

app.Run();
