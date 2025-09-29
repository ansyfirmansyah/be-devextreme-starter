using be_devextreme_starter.DTOs;
using FluentValidation;

namespace be_devextreme_starter.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator() {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId harus diisi.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password harus diisi.");
        }
    }
}
