namespace be_devextreme_starter.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message, StatusCode = 200 };
        }

        public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message, StatusCode = 201 };
        }

        public static ApiResponse<object> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<object> { Success = false, Data = null, Message = message, StatusCode = 404 };
        }

        public static ApiResponse<object> BadRequest(string message = "Bad request")
        {
            return new ApiResponse<object> { Success = false, Data = null, Message = message, StatusCode = 400 };
        }

        public static ApiResponse<object> Error(string message = "An unexpected error occurred", int statusCode = 500)
        {
            return new ApiResponse<object> { Success = false, Data = null, Message = message, StatusCode = statusCode };
        }
    }

    public class KodeOutletForReportDto
    {
        public string outlet_kode { get; set; }
        public string display { get; set; }
    }

    public class SalesUploadPreviewDto
    {
        public int RowNumber { get; set; }
        public string sales_kode { get; set; }
        public string sales_nama { get; set; }
        public string outlet_kode { get; set; }
        public bool IsValid { get; set; }
        public string? ValidationMessage { get; set; }
    }

    public class JualhUploadPreviewDto
    {
        public int RowNumber { get; set; }
        public string Jualh_kode { get; set; }
        public DateTime? Jualh_date { get; set; }
        public string Outlet_kode { get; set; }
        public string Outlet_nama { get; set; }
        public string? Outlet { get; set; }
        public long? Outlet_id { get; set; }
        public string Sales_kode { get; set; }
        public string Sales_nama { get; set; }
        public string? Sales { get; set; }
        public long? Sales_id { get; set; }
        public bool IsValid { get; set; }
        public string? ValidationMessage { get; set; }
    }

    public class JualdUploadPreviewDto
    {
        public int RowNumber { get; set; }
        public string Jualh_kode { get; set; }
        public string Barang_kode { get; set; }
        public string Barang_nama { get; set; }
        public string Barang { get; set; }
        public long? Barang_id { get; set; }
        public decimal Juald_harga { get; set; }
        public int Juald_qty { get; set; }
        public string? Diskon { get; set; }
        public long? Barangd_id { get; set; }
        public decimal? Juald_disk { get; set; }
        public bool IsValid { get; set; }
        public string? ValidationMessage { get; set; }
    }

    public class PenjualanUploadPreviewDto
    {
        public List<JualhUploadPreviewDto> Header { get; set; }
        public List<JualdUploadPreviewDto> Detail { get; set; }
    }
}