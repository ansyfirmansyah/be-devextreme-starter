namespace be_devextreme_starter.DTOs
{
    public class ContactDTO
    {
        public string full_name { get; set; }
        public string email { get; set; }
        public string? phone_number { get; set; }
        public string? company { get; set; }
        public string? job_title { get; set; }
        public string? address { get; set; }
        public int? city_id { get; set; }
        public int? postal_code { get; set; }
        public DateTime date_added { get; set; }
        public DateTime? last_contacted_date { get; set; }
        public int? lead_source_id { get; set; }
        public int? contact_status_id { get; set; }
        public decimal? estimated_value { get; set; }
        public bool is_subscribed { get; set; }
        public string? notes { get; set; }
    }
}
