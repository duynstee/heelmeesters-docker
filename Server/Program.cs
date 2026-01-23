using System.Text;
using HeelmeestersAPI.Features.AdminPortal.Users.Interfaces;
using HeelmeestersAPI.Features.AdminPortal.Users.Repositories;
using HeelmeestersAPI.Features.AdminPortal.Users.Services;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.Interfaces;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.Repositories;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.Services;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Interfaces;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Repositories;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Services;
using HeelmeestersAPI.Features.Shared.Appointments;
using HeelmeestersAPI.Features.Shared.Appointments.Interfaces;
using HeelmeestersAPI.Features.Shared.Appointments.Repositories;
using HeelmeestersAPI.Features.Shared.Appointments.Services;
// [ADDED - afspraak-maken feature] Patient appointment booking services
// using HeelmeestersAPI.Features.PatientPortal.Appointments;
using HeelmeestersAPI.Features.Shared.Auth.Interfaces;
using HeelmeestersAPI.Features.Shared.Auth.Models;
using HeelmeestersAPI.Features.Shared.Auth.Repositories;
using HeelmeestersAPI.Features.Shared.Auth.Services;
using HeelmeestersAPI.Features.Shared.MedicalRecords;
using HeelmeestersAPI.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

// Bind JwtSettings (voor elders in je app)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// JWT settings (uit appsettings.json -> "Jwt")
var jwtSection = builder.Configuration.GetSection("Jwt");

var secret = jwtSection["SecretKey"]
             ?? throw new Exception("Jwt:SecretKey ontbreekt in appsettings.json");

var issuer = jwtSection["Issuer"]
             ?? throw new Exception("Jwt:Issuer ontbreekt in appsettings.json");

var audience = jwtSection["Audience"]
               ?? throw new Exception("Jwt:Audience ontbreekt in appsettings.json");

var keyBytes = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        };
    });

builder.Services.AddAuthorization();

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// CORS (Vue dev server)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Voer 'Bearer {token}' in om geautoriseerde requests te doen."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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

// Dependency injection
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<ITreatmentProvider>(sp => (AppointmentService)sp.GetRequiredService<IAppointmentService>());
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IReferralRepository, ReferralRepository>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IPatientIdentityService, PatientIdentityService>();

var app = builder.Build();

// Swagger altijd aan (handig tijdens debuggen)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HeelmeestersAPI v1");
    c.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowViteDevServer");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

