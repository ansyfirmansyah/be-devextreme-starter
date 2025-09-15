using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Email_Queue
{
    public long emailq_id { get; set; }

    public string emailq_to { get; set; } = null!;

    public string emailq_from { get; set; } = null!;

    public string? emailq_cc { get; set; }

    public string? emailq_bcc { get; set; }

    public string? emailq_reply_to { get; set; }

    public string emailq_subject { get; set; } = null!;

    public string? emailq_body { get; set; }

    public string? emailq_attch_name1 { get; set; }

    public byte[]? emailq_attch_file1 { get; set; }

    public string? emailq_attch_name2 { get; set; }

    public byte[]? emailq_attch_file2 { get; set; }

    public string? emailq_attch_name3 { get; set; }

    public byte[]? emailq_attch_file3 { get; set; }

    public byte? emailq_status { get; set; }

    public DateTime emailq_queue_date { get; set; }

    public DateTime emailq_scheduled_sent { get; set; }

    public DateTime? emailq_sent_date { get; set; }

    public int? emailq_sent_try { get; set; }

    public string? emailq_process { get; set; }

    public long? emailq_process_id { get; set; }

    public string? emailq_error_text { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public long? notice_id { get; set; }

    public string? emailq_message_id { get; set; }

    public virtual ICollection<FW_Notice_Queue> FW_Notice_Queues { get; set; } = new List<FW_Notice_Queue>();

    public virtual FW_Notice? notice { get; set; }
}
