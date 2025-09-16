using be_devextreme_starter.Data.Models;

namespace be_devextreme_starter.DTOs
{
    public class JualDetailDTO
    {
        public long juald_id { get; set; }

        public long jualh_id { get; set; }

        public long barang_id { get; set; }

        public int juald_qty { get; set; }

        public decimal juald_harga { get; set; }

        public decimal juald_disk { get; set; }

        public string stsrc { get; set; } = null!;

        public string? created_by { get; set; }

        public DateTime? date_created { get; set; }

        public string? modified_by { get; set; }

        public DateTime? date_modified { get; set; }
        public string barang_kode { get; set; }

        public string barang_nama { get; set; }
    }
}
