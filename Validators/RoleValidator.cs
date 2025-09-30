using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using FluentValidation;

namespace be_devextreme_starter.Validators
{
    public class RoleValidator : AbstractValidator<RoleDto>
    {
        private readonly DataEntities _db;

        public RoleValidator(DataEntities db)
        {
            _db = db;

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID tidak boleh kosong.")
                .MaximumLength(40).WithMessage("Role ID maksimal 40 karakter.")
                .Must(roleId => !_db.FW_Ref_Roles.Any(r => r.role_id == roleId))
                .WithMessage("Role ID sudah digunakan.")
                .When(x => string.IsNullOrEmpty(x.RoleId)); // Validasi unik hanya saat membuat baru

            RuleFor(x => x.RoleCatatan)
                .NotEmpty().WithMessage("Catatan tidak boleh kosong.");
        }

    }
}
