using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudyMate.API.Data;
using StudyMate.API.Interfaces;
using StudyMate.API.Services;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

//////////////////////////////////////////////////
// DATABASE (SQL SERVER)
//////////////////////////////////////////////////

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


builder.Services.AddScoped<ILectureService, LectureService>();

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
// SERVICES (Dependency Injection)
//////////////////////////////////////////////////

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddHttpClient<IAiSummaryService, GeminiSummaryService>();

//////////////////////////////////////////////////
// CONTROLLERS + SWAGGER
//////////////////////////////////////////////////

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

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
// BUILD APP
//////////////////////////////////////////////////

var app = builder.Build();

//////////////////////////////////////////////////
// MIDDLEWARE PIPELINE
//////////////////////////////////////////////////

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();