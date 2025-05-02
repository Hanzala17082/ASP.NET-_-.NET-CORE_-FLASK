using Microsoft.EntityFrameworkCore;

namespace WebappTASK2.Models
{
    public class YourDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
        {
        }

        // You can also override OnModelCreating if needed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Example: modelBuilder.Entity<Employee>().ToTable("EmployeeTable");
        }
        
    }
}
