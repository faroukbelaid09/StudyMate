using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyMate.API.Data;
using StudyMate.API.Interfaces;
using StudyMate.API.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//////////////////////////////////////////////////
// DATABASE (SQL SERVER)
//////////////////////////////////////////////////

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//////////////////////////////////////////////////
// SERVICES (Dependency Injection)
//////////////////////////////////////////////////

builder.Services.AddScoped<ILectureService, LectureService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddHttpClient<IAiSummaryService, GeminiSummaryService>();

//////////////////////////////////////////////////
// JWT AUTHENTICATION
//////////////////////////////////////////////////

var jwt = builder.Configuration.GetSection("Jwt");

var key = jwt["Key"]
          ?? throw new Exception("JWT Key missing");

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
});

builder.Services.AddAuthorization();

//////////////////////////////////////////////////
// CONTROLLERS + JSON
//////////////////////////////////////////////////

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

//////////////////////////////////////////////////
// SWAGGER
//////////////////////////////////////////////////

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StudyMate API",
        Version = "v1"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter JWT Bearer token ONLY",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme,
            Array.Empty<string>()
        }
    });
});

//////////////////////////////////////////////////
// CORS (frontend access)
//////////////////////////////////////////////////

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend",
        policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin();
        });
});

//////////////////////////////////////////////////
// BUILD APP
//////////////////////////////////////////////////

var app = builder.Build();

//////////////////////////////////////////////////
// DATABASE MIGRATION
//////////////////////////////////////////////////

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;

    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch
        {
            retries--;
            Thread.Sleep(5000);
        }
    }
}

//////////////////////////////////////////////////
// MIDDLEWARE PIPELINE
//////////////////////////////////////////////////

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("frontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();