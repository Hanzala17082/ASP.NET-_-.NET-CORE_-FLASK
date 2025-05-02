using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    public class Employees
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

  
        [DataType(DataType.DateTime)]
        public DateTime DOJ { get; set; }
    }
}

