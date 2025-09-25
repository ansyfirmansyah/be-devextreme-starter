using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using FluentValidation;

namespace be_devextreme_starter.Validators
{
    public class PenjualanValidator: AbstractValidator<JualUpdateDto>
    {
        private readonly DataEntities _db;

        public PenjualanValidator(DataEntities db)
        {
            _db = db;

            RuleFor(x => x.jualh_kode)
                .NotEmpty().WithMessage("Kode Penjualan harus diisi.")
                .MaximumLength(10).WithMessage("Kode Penjualan maksimal 10 karakter.")
                .Must((dto, kode) => BeUniqueCode(kode, dto.jualh_id))
                .WithMessage(dto => $"Kode '{dto.jualh_kode}' sudah digunakan. Silakan gunakan kode lain.");

            RuleFor(x => x.jualh_date)
                .NotEmpty().WithMessage("Tanggal Penjualan harus diisi.");

            RuleFor(x => x.outlet_id)
                .GreaterThan(0).WithMessage("Outlet harus dipilih.")
                .Must(x => CheckExistingOutlet(x))
                .WithMessage(dto => $"Outlet tidak ditemukan.");

            RuleFor(x => x.sales_id)
                .GreaterThan(0).WithMessage("Sales harus dipilih.")
                .Must(x => CheckExistingSales(x))
                .WithMessage(dto => $"Sales tidak ditemukan.");

            RuleFor(x => x.temptable_detail_id)
                .NotEmpty().WithMessage("Temporary table id harus diisi.");
        }

        private bool BeUniqueCode(string kode, long id)
        {
            return !_db.Jual_Headers.Any(x => x.jualh_kode == kode && x.jualh_id != id && x.stsrc == "A");
        }

        private bool CheckExistingOutlet(long id)
        {
            return _db.Outlet_Masters.Any(x => x.outlet_id == id && x.stsrc == "A");
        }

        private bool CheckExistingSales(long id)
        {
            return _db.Sales_Masters.Any(x => x.sales_id == id && x.stsrc == "A");
        }
    }
}
