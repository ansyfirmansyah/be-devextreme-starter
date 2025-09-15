using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Notice_Template
{
    public string nott_id { get; set; } = null!;

    public string nott_title { get; set; } = null!;

    public string? nott_group { get; set; }

    public string? nott_to { get; set; }

    public string? nott_cc { get; set; }

    public string? nott_bcc { get; set; }

    public string? nott_batch { get; set; }

    public string? nott_sender { get; set; }

    public string nott_content { get; set; } = null!;

    public string? nott_catatan { get; set; }

    public bool nott_one_email_per_user { get; set; }

    public string? nott_ref_id { get; set; }

    public string? nott_ref_id2 { get; set; }

    public string? nott_ref_id3 { get; set; }

    public string? nott_model_type { get; set; }

    public string? nott_key_type { get; set; }

    public string? nott_last_test_id { get; set; }

    public string? nott_small_content { get; set; }

    public string? rdef_kode { get; set; }

    public string? nott_rdef_param_csv { get; set; }

    public virtual ICollection<FW_Notice_Queue> FW_Notice_Queues { get; set; } = new List<FW_Notice_Queue>();

    public virtual ICollection<FW_Notice> FW_Notices { get; set; } = new List<FW_Notice>();

    public virtual ReportDef? rdef_kodeNavigation { get; set; }
}
