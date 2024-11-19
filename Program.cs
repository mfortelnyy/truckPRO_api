using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using truckPRO_api.Data;
using truckPRO_api.MappingProfiles;
using truckPRO_api.Models;
using truckPRO_api.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add console logging
builder.Logging.AddConsole();

//Add Razor pages
builder.Services.AddControllersWithViews();


// Add services to the container
builder.Services.AddControllers();

// Add DbContext for SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();

// Add and register AutoMapper service
builder.Services.AddAutoMapper(typeof(DriverMappingProfilecs));

// Register PasswordHasher for User model
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register AWS S3 Client
builder.Services.AddAWSService<IAmazonS3>();

// Register the S3Service for S3 interactions
builder.Services.AddScoped<S3Service>();

// Register custom UserService for handling user-related operations
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ILogEntryService, LogEntryService>();

builder.Services.AddScoped<IManagerService, ManagerService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<ISmsService, SmsService>();

builder.Services.AddScoped<IPdfService, PdfService>();

builder.Services.AddScoped<IUserValidationService, UserValidationService>();



// Retrieve the JWT key from configuration
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key is not configured properly in appsettings.json.");
}
else if (!string.IsNullOrEmpty(jwtKey))
{
    Console.WriteLine("Succ key parsed");
}

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("QoyLLM8SxXaUfYMJKT7svrVlAgpJD04d")),
    };
});

builder.Services.AddAuthorization(auth =>
    {
        auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());
    });

builder.Services.AddRazorPages();



// Build the application
var app = builder.Build();


// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable JWT authentication
app.UseAuthentication();

// Enable authorization for secured endpoints
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

//enable razor ages mapping
app.MapRazorPages();

//to enable Assets folder
app.UseStaticFiles();

// Start the application
app.Run();
