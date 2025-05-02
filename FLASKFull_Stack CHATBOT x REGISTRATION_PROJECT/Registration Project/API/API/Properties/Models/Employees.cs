
using System.ComponentModel.DataAnnotations;
using EmployeeAPI;
using EmployeeAPI.Services;
using EmployeeAPI.Controllers;
using EmployeeAPI.Data;
using EmployeeAPI.Models;
using EmployeeAPI.Settings;

namespace EmployeeAPI.Models
{
    public class Employees
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 100, ErrorMessage = "Age must be between 1 and 100.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }


        [DataType(DataType.DateTime)]
        public DateTime? DOJ { get; set; } = DateTime.Now;



        public string SecretKey { get; set; }

        public int TokenExpiryMinutes
        {
            get; set;
        }

    }
}
    

