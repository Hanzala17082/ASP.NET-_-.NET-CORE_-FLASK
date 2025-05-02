namespace EmployeeAPI.Services
{
    public interface IAuthService
    {
        string GenerateToken(IConfiguration configuration);
    }
}