namespace be_devextreme_starter.DTOs
{
    public class SalesUpdateDto
    {
        public int sales_id { get; set; }
        public string sales_kode { get; set; }
        public string sales_nama { get; set; }
        public int outlet_id { get; set; }
    }

    public class KlasifikasiUpdateDto
    {
        public long klas_id { get; set; }
        public string klas_kode { get; set; }
        public string klas_nama { get; set; }
        public long? klas_parent_id { get; set; }
    }

    public class BarangMasterUpdateDto
    {
        public long barang_id { get; set; }
        public string barang_kode { get; set; }
        public string barang_nama { get; set; }
        public decimal barang_harga { get; set; }
        public long klas_id { get; set; }

        public string temptable_outlet_id { get; set; }
        public string temptable_diskon_id { get; set; }
    }

    public class JualUpdateDto
    {
        public long jualh_id { get; set; }

        public string jualh_kode { get; set; } = null!;

        public DateTime jualh_date { get; set; }

        public long sales_id { get; set; }

        public long outlet_id { get; set; }

        public string stsrc { get; set; } = null!;

        public string? created_by { get; set; }

        public DateTime? date_created { get; set; }

        public string? modified_by { get; set; }

        public DateTime? date_modified { get; set; }

        public string temptable_detail_id { get; set; }
    }
}
