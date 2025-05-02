using Microsoft.EntityFrameworkCore;
using EmployeeAPI;
using EmployeeAPI.Services;
using EmployeeAPI.Controllers;
using EmployeeAPI.Data;

using EmployeeAPI.Models;
using EmployeeAPI.Settings;



namespace EmployeeAPI.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employees> Employees { get; set; }
    }
}