using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Employees
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 100)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Date of Joining")]
        [DataType(DataType.Date)]
        public DateTime Date_Of_Joining { get; set; }
        public static class ConnectionString
        {
            public static string dbcs { get; set; }
        }
    }
}