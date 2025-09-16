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
}
