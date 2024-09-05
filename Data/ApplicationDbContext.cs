using Microsoft.EntityFrameworkCore;
using truckPRO_api.Models;

namespace truckPRO_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
    }
    
}
