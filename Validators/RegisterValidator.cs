using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using FluentValidation;

namespace be_devextreme_starter.Validators
{
    public class RegisterStep1Validator : AbstractValidator<RegisterStep1Dto>
    {
        private readonly DataEntities _db;

        public RegisterStep1Validator(DataEntities db)
        {
            _db = db;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Nama harus diisi.")
                .Must(userName => !_db.User_Masters.Any(u => u.user_nama == userName))
                .WithMessage("Nama sudah digunakan, silakan gunakan nama lain.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email harus diisi.")
                .EmailAddress().WithMessage("Format email tidak valid.")
                .Must(email => !_db.User_Masters.Any(u => u.user_email == email))
                .WithMessage("Email sudah terdaftar.");
        }
    }

    public class RegisterStep2Validator : AbstractValidator<RegisterStep2Dto>
    {
        public RegisterStep2Validator()
        {
            RuleFor(x => x.TempToken)
                .NotEmpty().WithMessage("Sesi registrasi tidak valid.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password harus diisi.")
                .MinimumLength(8).WithMessage("Password minimal 8 karakter.")
                .Matches("[A-Z]").WithMessage("Password harus mengandung setidaknya satu huruf besar.")
                .Matches("[a-z]").WithMessage("Password harus mengandung setidaknya satu huruf kecil.")
                .Matches("[0-9]").WithMessage("Password harus mengandung setidaknya satu angka.")
                .Matches("[@$!%*?&]").WithMessage("Password harus mengandung setidaknya satu karakter spesial (@$!%*?&).");
        }
    }
}
