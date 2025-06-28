using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Data
{
    // This class represents the Entity Framework Core database context for the application
    public class ApplicationDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes them to the base class
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        // Configures the database provider and connection settings
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use an in-memory database named "UserDb" for development/testing
            optionsBuilder.UseInMemoryDatabase(databaseName: "UserDb");
        }

        // DbSet representing the Users table
        public DbSet<User> Users { get; set; }

        // DbSet representing the Tasks table
        public DbSet<TaskItem> Tasks { get; set; }
    }
}
