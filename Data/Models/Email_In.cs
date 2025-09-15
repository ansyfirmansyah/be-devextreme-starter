using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Email_In
{
    public long emin_id { get; set; }

    public string emin_to { get; set; } = null!;

    public string emin_from { get; set; } = null!;

    public string? emin_cc { get; set; }

    public string? emin_reply_to { get; set; }

    public string emin_subject { get; set; } = null!;

    public string? emin_body { get; set; }

    public string? emin_header { get; set; }

    public DateTime? emin_date { get; set; }

    public DateTime? emin_receive_date { get; set; }

    public bool? emin_is_bounce { get; set; }

    public bool emin_is_read { get; set; }

    public string? emin_message_id { get; set; }

    public string? emin_bounce_message_id { get; set; }

    public string? emin_bounce_from { get; set; }

    public string? emin_bounce_subject { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }
}
