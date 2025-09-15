using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Notice
{
    public long notice_id { get; set; }

    public string? notice_title { get; set; }

    public string? notice_sender { get; set; }

    public string? notice_to { get; set; }

    public string? notice_cc { get; set; }

    public string? notice_bcc { get; set; }

    public bool notice_batch_status { get; set; }

    public string? notice_content { get; set; }

    public string? notice_small_content { get; set; }

    public string? nott_id { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public string? notice_ref_id { get; set; }

    public string? notice_ref_id2 { get; set; }

    public string? notice_ref_id3 { get; set; }

    public string? notice_batch_users { get; set; }

    public long? appr_id { get; set; }

    public string? notice_catatan { get; set; }

    public string? notice_attach_link { get; set; }

    public string? notice_attach_link2 { get; set; }

    public string? notice_attach_link3 { get; set; }

    public string? notice_rdef_param_csv { get; set; }

    public virtual ICollection<FW_Email_Queue> FW_Email_Queues { get; set; } = new List<FW_Email_Queue>();

    public virtual FW_Notice_Template? nott { get; set; }
}
