using Microsoft.EntityFrameworkCore;
using truckPRO_api.Models;

namespace truckPRO_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<LogEntry> LogEntry { get; set; }
    }

}