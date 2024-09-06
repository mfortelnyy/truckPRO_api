using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using truckPRO_api.Data;
using truckPRO_api.MappingProfiles;
using truckPRO_api.Models;
using truckPRO_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddEndpointsApiExplorer();
//Add and register  Automapper service SignUpDTO -> User; LoginDTO -> User
builder.Services.AddAutoMapper(typeof(DriverMappingProfilecs));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


//register userservice
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Auidence"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

    };
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//minimal api
/*Routing
app.MapGet("/signIn", () =>
{
   return "Signing in!";
});

app.MapPost("/register/{userName}", (RegistrationModel registration) =>
{
    return $"Signing Up! {registration.userName} with password: {registration.password}";
});
*/

app.Run();
