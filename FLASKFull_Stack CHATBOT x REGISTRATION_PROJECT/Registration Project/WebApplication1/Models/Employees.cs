
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Employees
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

      

        [DataType(DataType.DateTime)]
        public DateTime? DOJ { get; set; } = DateTime.Now;

      
    }
    public class TokenRequest
    {
        public string Token { get; set; }

        public string ApiKey { get; set; }
    }

}


