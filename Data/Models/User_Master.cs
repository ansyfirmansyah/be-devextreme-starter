using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class User_Master
{
    public string user_id { get; set; } = null!;

    public string user_nama { get; set; } = null!;

    public string? user_alamat { get; set; }

    public string? user_telp { get; set; }

    public string user_password { get; set; } = null!;

    public DateTime? user_last_login { get; set; }

    public string? user_email { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public string? user_kode { get; set; }

    public string? user_ldap { get; set; }

    public string? user_main_role { get; set; }

    public string? user_divisi { get; set; }

    public DateTime? user_password_lastchange { get; set; }

    public int user_failed_login_count { get; set; }

    public DateTime? user_blocked_date { get; set; }

    public string user_status { get; set; } = null!;

    public string? user_ldap_department { get; set; }

    public string? user_ldap_office { get; set; }

    public string? user_ldap_description { get; set; }

    public string? user_roles_csv { get; set; }

    public string? user_delegate { get; set; }

    public DateOnly? user_delegate_from { get; set; }

    public DateOnly? user_delegate_until { get; set; }

    public long? post_id { get; set; }

    public string? ip_address { get; set; }

    public string? user_agent { get; set; }

    public DateTime? LastTimeCookies { get; set; }

    public virtual ICollection<FW_Notice_Queue> FW_Notice_Queues { get; set; } = new List<FW_Notice_Queue>();

    public virtual ICollection<FW_User_Lost_Password> FW_User_Lost_Passwords { get; set; } = new List<FW_User_Lost_Password>();

    public virtual ICollection<FW_User_Role> FW_User_Roles { get; set; } = new List<FW_User_Role>();

    public virtual ICollection<User_Refresh_Token> User_Refresh_Tokens { get; set; } = new List<User_Refresh_Token>();

    public virtual Post? post { get; set; }
}
