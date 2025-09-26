using System.ComponentModel.DataAnnotations;

namespace be_devextreme_starter.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Password { get; set; }

        public string? DeviceInfo { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
