using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MSPR_bloc_4_orders.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
bool isTesting = builder.Environment.IsEnvironment("Testing");

// DbContext conditionnel
builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    if (!isTesting)
        options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb"));
});

// Swagger avec JWT pour documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ex: Bearer eyJhbGciOiJIUzI1NiIs..."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// Authentification JWT hors tests
if (!isTesting)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
}
else
{
    // ✅ Déclare un schéma "Test" comme par défaut sans handler (autorise UseAuthorization sans erreur)
    builder.Services.AddAuthentication("Test");
}

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Safe même en test car un schéma est défini
app.UseAuthorization();

if (isTesting)
{
    // Injecte un utilisateur factice pendant les tests
    app.Use(async (context, next) =>
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "admin")
        }, "TestAuth");

        context.User = new ClaimsPrincipal(identity);
        await next();
    });
}

app.MapControllers();
app.Run();

public partial class Program { }
