using be_devextreme_starter.Data.Models;

namespace be_devextreme_starter.DTOs
{
    public class BarangOutletDTO
    {
        public long barango_id { get; set; }

        public long barang_id { get; set; }

        public long outlet_id { get; set; }

        public string stsrc { get; set; } = null!;

        public string? created_by { get; set; }

        public DateTime? date_created { get; set; }

        public string? modified_by { get; set; }

        public DateTime? date_modified { get; set; }
        public string outlet_kode { get; set; } //kolom tambahan untuk menampung kode outlet pada saat proses edit data
        public string outlet_nama { get; set; } //kolom tambahan untuk menampung nama outlet pada saat proses edit data
    }
}
