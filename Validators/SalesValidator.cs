using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using FluentValidation;

namespace be_devextreme_starter.Validators
{
    public class SalesValidator : AbstractValidator<SalesUpdateDto>
    {
        private readonly DataEntities _db;
        public SalesValidator(DataEntities db)
        {
            _db = db;

            RuleFor(x => x.sales_kode)
                .NotEmpty().WithMessage("Kode Sales harus diisi.")
                .MaximumLength(10).WithMessage("Kode Sales maksimal 10 karakter.")
                .Must((dto, kode) => BeUniqueCode(kode, dto.sales_id))
                .WithMessage(dto => $"Kode '{dto.sales_kode}' sudah digunakan. Silakan gunakan kode lain.");

            RuleFor(x => x.sales_nama)
                .NotEmpty().WithMessage("Nama Sales harus diisi.")
                .MaximumLength(100).WithMessage("Nama Sales maksimal 100 karakter.");

            RuleFor(x => x.outlet_id)
                .GreaterThan(0).WithMessage("Outlet harus dipilih.")
                .Must(x => CheckExistingOutlet(x))
                .WithMessage(dto => $"Outlet tidak ditemukan.");
        }

        private bool BeUniqueCode(string kode, long id)
        {
            return !_db.Sales_Masters.Any(x => x.sales_kode == kode && x.sales_id != id && x.stsrc == "A");
        }

        private bool CheckExistingOutlet(long id)
        {
            return _db.Outlet_Masters.Any(x => x.outlet_id == id && x.stsrc == "A");
        }
    }
}
