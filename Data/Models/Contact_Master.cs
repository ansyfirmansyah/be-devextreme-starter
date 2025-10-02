using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Contact_Master
{
    public long contact_id { get; set; }

    public string full_name { get; set; } = null!;

    public string email { get; set; } = null!;

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

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual City_Master? city { get; set; }

    public virtual Contact_Status_Master? contact_status { get; set; }

    public virtual Lead_Source_Master? lead_source { get; set; }
}
