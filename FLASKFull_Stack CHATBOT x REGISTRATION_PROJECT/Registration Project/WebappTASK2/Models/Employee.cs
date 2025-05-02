using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebappTASK2.Models
{
    public class Employee
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Designation { get; set; }
        public string City { get; set; }
        public DateTime Date_Of_Joining { get; set; }
    }
}


