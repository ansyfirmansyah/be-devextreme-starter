using be_devextreme_starter.DTOs;
using FluentValidation;

namespace be_devextreme_starter.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Password saat ini harus diisi.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password baru harus diisi.")
                .MinimumLength(8).WithMessage("Password baru minimal 8 karakter.")
                .Matches("[A-Z]").WithMessage("Password baru harus mengandung setidaknya satu huruf besar.")
                .Matches("[a-z]").WithMessage("Password baru harus mengandung setidaknya satu huruf kecil.")
                .Matches("[0-9]").WithMessage("Password baru harus mengandung setidaknya satu angka.")
                .Matches("[@$!%*?&]").WithMessage("Password baru harus mengandung setidaknya satu karakter spesial (@$!%*?&).")
                .NotEqual(x => x.CurrentPassword).WithMessage("Password baru tidak boleh sama dengan password saat ini.");
        }
    }
}
