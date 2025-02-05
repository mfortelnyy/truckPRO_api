using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using truckPRO_api.Data;
using truckPRO_api.MappingProfiles;
using truckPRO_api.Models;
using truckPRO_api.Services;
using truckPro_api.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Firebase
var firebaseCredentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
if (string.IsNullOrEmpty(firebaseCredentialsPath))
{
    //Console.WriteLine("Firebase credentials path not found in environment variables.");
    throw new Exception("Firebase credentials path not found in environment variables.");
}
else
{
    Console.WriteLine($"Firebase credentials path: {firebaseCredentialsPath}");
}

var creds = false;
if (firebaseCredentialsPath != null)
{
    creds = true;
}


FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
    //Credential = GoogleCredential.FromJson(gJson)
});
//Console.WriteLine($"Firebase initialized: {firebaseCredentialsPath}");


// Add Razor Pages and Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services

// Registers AutoMapper for object-to-object mapping, which helps in converting between DTOs and entities.
builder.Services.AddAutoMapper(typeof(DriverMappingProfilecs));

// Registers a password hashing service for securely storing and verifying user passwords.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Registers AWS S3 service for handling file uploads and storage in Amazon S3.
builder.Services.AddAWSService<IAmazonS3>();

// Registers a custom S3 service that interacts with AWS S3 for file management.
builder.Services.AddScoped<S3Service>();

// Registers a user service that likely handles user-related business logic and operations.
builder.Services.AddScoped<IUserService, UserService>();

// Registers a logging service to manage log entries, possibly for auditing or debugging purposes.
builder.Services.AddScoped<ILogEntryService, LogEntryService>();

// Registers a manager service, likely responsible for handling business logic related to managers.
builder.Services.AddScoped<IManagerService, ManagerService>();

// Registers an email service for sending emails, such as notifications, verification, or password resets.
builder.Services.AddScoped<IEmailService, EmailService>();

// Registers an admin service that probably contains functionality specific to admin-related tasks.
builder.Services.AddScoped<IAdminService, AdminService>();

// Registers an SMS service for sending text messages, possibly for OTP verification or notifications.
builder.Services.AddScoped<ISmsService, SmsService>();

// Registers a PDF service, likely used for generating and managing PDF documents.
builder.Services.AddScoped<IPdfService, PdfService>();

// Registers a user validation service, which may be responsible for validating user input or credentials.
builder.Services.AddScoped<IUserValidationService, UserValidationService>();

// Registers a Firebase service, which might be used for push notifications, authentication, or real-time database operations.
builder.Services.AddScoped<IFirebaseService, FirebaseService>();

// Registers SignalR to enable real-time communication between the server and clients.
builder.Services.AddSignalR();

// Registers HttpClient for making HTTP requests to external APIs or microservices.
builder.Services.AddHttpClient();

// Registers antiforgery protection to help prevent Cross-Site Request Forgery (CSRF) attacks.
builder.Services.AddAntiforgery();

// Registers a hosted background service that monitors or logs user activity.
builder.Services.AddHostedService<UserActivityService>();


// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key is not configured properly.");
}
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build());
});

// Build App
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

// Enable antiforgery middleware
app.Use(next => async context =>
{
    if (context.Request.Path == "/Login")
    {
        var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
        var tokens = antiforgery.GetAndStoreTokens(context);
        context.Response.Headers.Add("RequestVerificationToken", tokens.RequestToken);
    }

    await next(context);
});


app.MapFallbackToPage("/Index");
app.Run();