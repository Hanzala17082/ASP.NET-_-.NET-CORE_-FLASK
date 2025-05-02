namespace EmployeeAPI.Models
{
    public class ApiCredentials
    {

        public string SecretKey { get; set; }

        public int TokenExpiryMinutes
        {
            get; set;
        }


    }
}