
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;  
using Microsoft.IdentityModel.Tokens;   
using System.Security.Claims;           
using truckPRO_api.Data;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    // Background service to track user activity and update their status
    public class UserActivityService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory; // Factory for creating service scopes

        // Inject IServiceScopeFactory for handling scoped services
        public UserActivityService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // Overrides the ExecuteAsync method to define background task logic
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Fetch all active tokens of users who have an "Active" status
                    var activeUserTokens = await dbContext.UserTokens
                        .Include(ut => ut.User)
                        .Where(ut => ut.User.Status == ActivityStatus.Active && !ut.IsRevoked)
                        .ToListAsync();

                    foreach (var userToken in activeUserTokens)
                    {
                        if (TokenExpired(userToken.Token))
                        {
                            userToken.IsRevoked = true; // Revoke expired tokens
                        }
                    }

                    await dbContext.SaveChangesAsync(); 
                }

                // Wait for 5 minutes before next execution
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        // Determines if a user's token has expired - True if expd
        private bool TokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);

            if (expClaim != null && long.TryParse(expClaim.Value, out var expUnix))
            {
                var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                return expDate <= DateTime.UtcNow; // If exp time is in the future than false - not expd, true(expd) otherwise
            }

            return true; // If no exp claim found, assume expired
        }
    }
}