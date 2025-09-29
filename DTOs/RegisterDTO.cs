using System.ComponentModel.DataAnnotations;

namespace be_devextreme_starter.DTOs
{
    public class RegisterStep1Dto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class RegisterStep2Dto
    {
        public string TempToken { get; set; } // Token temporer dari langkah 1

        public string Password { get; set; }

        public string? DeviceInfo { get; set; }
    }
}
