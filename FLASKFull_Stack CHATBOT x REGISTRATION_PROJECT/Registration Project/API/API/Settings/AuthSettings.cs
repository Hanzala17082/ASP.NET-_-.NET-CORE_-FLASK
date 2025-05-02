namespace EmployeeAPI.Settings
{
    public class AuthSettings
    {
        public string SecretKey { get; set; }
        public int TokenExpiryMinutes { get; set; }
    }
}