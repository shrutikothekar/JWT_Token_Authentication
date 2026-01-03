using doctor_app_api.Models;

namespace doctor_app_api.Dtos
{
    //public class AuthDtos
    //{
    //}

    public record RegisterDto(string FullName, string Email, string Password, UserRole Role);
    public record LoginDto(string Email, string Password);

    public record AuthResult(
        string Token,
        Guid UserId,
        string Name,
        string Email,
        string Role
    );
}



