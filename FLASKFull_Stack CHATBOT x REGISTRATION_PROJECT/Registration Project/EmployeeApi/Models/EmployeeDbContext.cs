using Microsoft.EntityFrameworkCore;
using EmployeeAPI;
using EmployeeAPI.Models;

public class EmployeeDbContext : DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }

    public DbSet<Employees> Employees { get; set; }
}
