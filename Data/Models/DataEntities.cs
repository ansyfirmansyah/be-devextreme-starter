using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace be_devextreme_starter.Data.Models;

public partial class DataEntities : DbContext
{
    public DataEntities(DbContextOptions<DataEntities> options)
        : base(options)
    {
    }

    public virtual DbSet<Barang_Diskon> Barang_Diskons { get; set; }

    public virtual DbSet<Barang_Master> Barang_Masters { get; set; }

    public virtual DbSet<Barang_Outlet> Barang_Outlets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryLog> CategoryLogs { get; set; }

    public virtual DbSet<Category_DiffDate> Category_DiffDates { get; set; }

    public virtual DbSet<DocumentTransitionHistory> DocumentTransitionHistories { get; set; }

    public virtual DbSet<Email_In> Email_Ins { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FW_Attachment> FW_Attachments { get; set; }

    public virtual DbSet<FW_Calendar> FW_Calendars { get; set; }

    public virtual DbSet<FW_Email_Queue> FW_Email_Queues { get; set; }

    public virtual DbSet<FW_Grid_Setting> FW_Grid_Settings { get; set; }

    public virtual DbSet<FW_Holiday> FW_Holidays { get; set; }

    public virtual DbSet<FW_Kode_Counter> FW_Kode_Counters { get; set; }

    public virtual DbSet<FW_Kode_Detail> FW_Kode_Details { get; set; }

    public virtual DbSet<FW_Kode_Format> FW_Kode_Formats { get; set; }

    public virtual DbSet<FW_LOG_Error> FW_LOG_Errors { get; set; }

    public virtual DbSet<FW_Notice> FW_Notices { get; set; }

    public virtual DbSet<FW_Notice_Queue> FW_Notice_Queues { get; set; }

    public virtual DbSet<FW_Notice_Template> FW_Notice_Templates { get; set; }

    public virtual DbSet<FW_Ref_Modul> FW_Ref_Moduls { get; set; }

    public virtual DbSet<FW_Ref_Role> FW_Ref_Roles { get; set; }

    public virtual DbSet<FW_Ref_Setting> FW_Ref_Settings { get; set; }

    public virtual DbSet<FW_Role_Right> FW_Role_Rights { get; set; }

    public virtual DbSet<FW_Temp_Table> FW_Temp_Tables { get; set; }

    public virtual DbSet<FW_User_Lost_Password> FW_User_Lost_Passwords { get; set; }

    public virtual DbSet<FW_User_Role> FW_User_Roles { get; set; }

    public virtual DbSet<FW_Workday> FW_Workdays { get; set; }

    public virtual DbSet<Help> Helps { get; set; }

    public virtual DbSet<Jual_Detail> Jual_Details { get; set; }

    public virtual DbSet<Jual_Header> Jual_Headers { get; set; }

    public virtual DbSet<Klasifikasi_Master> Klasifikasi_Masters { get; set; }

    public virtual DbSet<LOV> LOVs { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Menu_Permission> Menu_Permissions { get; set; }

    public virtual DbSet<Outlet_Master> Outlet_Masters { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Post_Child> Post_Children { get; set; }

    public virtual DbSet<ReportDef> ReportDefs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sales_Master> Sales_Masters { get; set; }

    public virtual DbSet<StructDivision> StructDivisions { get; set; }

    public virtual DbSet<Time> Times { get; set; }

    public virtual DbSet<User_Master> User_Masters { get; set; }

    public virtual DbSet<User_Refresh_Token> User_Refresh_Tokens { get; set; }

    public virtual DbSet<WorkflowGlobalParameter> WorkflowGlobalParameters { get; set; }

    public virtual DbSet<WorkflowInbox> WorkflowInboxes { get; set; }

    public virtual DbSet<WorkflowProcessInstance> WorkflowProcessInstances { get; set; }

    public virtual DbSet<WorkflowProcessInstancePersistence> WorkflowProcessInstancePersistences { get; set; }

    public virtual DbSet<WorkflowProcessInstanceStatus> WorkflowProcessInstanceStatuses { get; set; }

    public virtual DbSet<WorkflowProcessScheme> WorkflowProcessSchemes { get; set; }

    public virtual DbSet<WorkflowProcessTimer> WorkflowProcessTimers { get; set; }

    public virtual DbSet<WorkflowProcessTransitionHistory> WorkflowProcessTransitionHistories { get; set; }

    public virtual DbSet<WorkflowScheme> WorkflowSchemes { get; set; }

    public virtual DbSet<vHead> vHeads { get; set; }

    public virtual DbSet<vStructDivisionParent> vStructDivisionParents { get; set; }

    public virtual DbSet<vStructDivisionParentsAndThi> vStructDivisionParentsAndThis { get; set; }

    public virtual DbSet<vw_UserPermission> vw_UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Barang_Diskon>(entity =>
        {
            entity.HasKey(e => e.barangd_id);

            entity.ToTable("Barang_Diskon");

            entity.Property(e => e.barangd_disc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.barang).WithMany(p => p.Barang_Diskons)
                .HasForeignKey(d => d.barang_id)
                .HasConstraintName("FK_Barang_Diskon_Barang_Master");
        });

        modelBuilder.Entity<Barang_Master>(entity =>
        {
            entity.HasKey(e => e.barang_id);

            entity.ToTable("Barang_Master");

            entity.Property(e => e.barang_harga).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.barang_kode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("menggambarkan hubungan wilayah penjualan.\r\nContoh : city anak dari branch");
            entity.Property(e => e.barang_nama)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<Barang_Outlet>(entity =>
        {
            entity.HasKey(e => e.barango_id);

            entity.ToTable("Barang_Outlet");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.barang).WithMany(p => p.Barang_Outlets)
                .HasForeignKey(d => d.barang_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Barang_Outlet_Barang_Master");

            entity.HasOne(d => d.outlet).WithMany(p => p.Barang_Outlets)
                .HasForeignKey(d => d.outlet_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Barang_Outlet_Outlet_Master");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID).HasName("PK_Categories");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryName).HasMaxLength(64);
        });

        modelBuilder.Entity<CategoryLog>(entity =>
        {
            entity.ToTable("CategoryLog");

            entity.HasIndex(e => new { e.LogID, e.CategoryID }, "ixCategoryLog");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryLogs)
                .HasForeignKey(d => d.CategoryID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CategoryLog_Category");

            entity.HasOne(d => d.Log).WithMany(p => p.CategoryLogs)
                .HasForeignKey(d => d.LogID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CategoryLog_Log");
        });

        modelBuilder.Entity<Category_DiffDate>(entity =>
        {
            entity.HasKey(e => e.cd_id);

            entity.ToTable("Category_DiffDate");

            entity.Property(e => e.cd_text)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<DocumentTransitionHistory>(entity =>
        {
            entity.ToTable("DocumentTransitionHistory");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Command).HasMaxLength(1024);
            entity.Property(e => e.DestinationState).HasMaxLength(1024);
            entity.Property(e => e.InitialState).HasMaxLength(1024);
            entity.Property(e => e.Order).ValueGeneratedOnAdd();
            entity.Property(e => e.TransitionTime).HasColumnType("datetime");
            entity.Property(e => e.TransitionTimeForSort)
                .HasComputedColumnSql("(coalesce([TransitionTime],CONVERT([datetime],'9999-12-31',(20))))", false)
                .HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.DocumentTransitionHistories)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_DocumentTransitionHistory_Employee");
        });

        modelBuilder.Entity<Email_In>(entity =>
        {
            entity.HasKey(e => e.emin_id);

            entity.ToTable("Email_In");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.emin_body)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emin_bounce_from)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.emin_bounce_message_id)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.emin_bounce_subject).IsUnicode(false);
            entity.Property(e => e.emin_cc)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emin_date).HasColumnType("datetime");
            entity.Property(e => e.emin_from)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emin_header)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emin_message_id)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.emin_receive_date).HasColumnType("datetime");
            entity.Property(e => e.emin_reply_to)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emin_subject)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emin_to)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(256);

            entity.HasOne(d => d.StructDivision).WithMany(p => p.Employees)
                .HasForeignKey(d => d.StructDivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_StructDivision");

            entity.HasMany(d => d.Roles).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_EmployeeRole_Roles"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .HasConstraintName("FK_EmployeeRole_Employee"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "RoleId").HasName("PK_EmployeeRoles");
                        j.ToTable("EmployeeRole");
                    });
        });

        modelBuilder.Entity<FW_Attachment>(entity =>
        {
            entity.HasKey(e => e.attach_id).HasName("PK_Attachment");

            entity.ToTable("FW_Attachment");

            entity.HasIndex(e => e.stsrc, "idx_att_stsrc2");

            entity.HasIndex(e => e.stsrc, "idx_att_stsrc_pplandok");

            entity.Property(e => e.attach_file_link)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.attach_file_nama)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.attach_file_pwd)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.attach_kode)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.attach_thumb).HasColumnType("image");
            entity.Property(e => e.attach_tipe)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.help).WithMany(p => p.FW_Attachments)
                .HasForeignKey(d => d.help_id)
                .HasConstraintName("FK_FW_Attachment_Help");
        });

        modelBuilder.Entity<FW_Calendar>(entity =>
        {
            entity.HasKey(e => e.calendar_id).HasName("PK_Calendar");

            entity.ToTable("FW_Calendar");

            entity.Property(e => e.calendar_id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.calendar_date).HasColumnType("datetime");
            entity.Property(e => e.calendar_dayName)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.calendar_holiday_keterangan)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.calendar_monthName)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.calendar_type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Standard");
        });

        modelBuilder.Entity<FW_Email_Queue>(entity =>
        {
            entity.HasKey(e => e.emailq_id).HasName("PK_Email_Queue");

            entity.ToTable("FW_Email_Queue");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.emailq_attch_file1).HasColumnType("image");
            entity.Property(e => e.emailq_attch_file2).HasColumnType("image");
            entity.Property(e => e.emailq_attch_file3).HasColumnType("image");
            entity.Property(e => e.emailq_attch_name1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.emailq_attch_name2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.emailq_attch_name3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.emailq_bcc)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emailq_body)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emailq_cc)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emailq_error_text).IsUnicode(false);
            entity.Property(e => e.emailq_from)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emailq_message_id)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.emailq_process)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emailq_queue_date).HasColumnType("smalldatetime");
            entity.Property(e => e.emailq_reply_to)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emailq_scheduled_sent).HasColumnType("smalldatetime");
            entity.Property(e => e.emailq_sent_date).HasColumnType("smalldatetime");
            entity.Property(e => e.emailq_sent_try).HasDefaultValue(0);
            entity.Property(e => e.emailq_subject)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.emailq_to)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.notice).WithMany(p => p.FW_Email_Queues)
                .HasForeignKey(d => d.notice_id)
                .HasConstraintName("FK_FW_Email_Queue_FW_Notice");
        });

        modelBuilder.Entity<FW_Grid_Setting>(entity =>
        {
            entity.HasKey(e => e.gset_id).HasName("PK_Grid_Setting");

            entity.ToTable("FW_Grid_Setting");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.gset_grid)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.gset_setting).IsUnicode(false);
            entity.Property(e => e.gset_user)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<FW_Holiday>(entity =>
        {
            entity.HasKey(e => e.holiday_id).HasName("PK_Holiday");

            entity.ToTable("FW_Holiday");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.holiday_date).HasColumnType("datetime");
            entity.Property(e => e.holiday_keterangan)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<FW_Kode_Counter>(entity =>
        {
            entity.HasKey(e => e.kdcn_id).HasName("PK_New_Kode_Counter");

            entity.ToTable("FW_Kode_Counter");

            entity.Property(e => e.kdcn_counter).HasDefaultValue(1);
            entity.Property(e => e.kdcn_last)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.kdcn_last_reset).HasColumnType("datetime");
            entity.Property(e => e.kdcn_last_update).HasColumnType("datetime");
            entity.Property(e => e.kdcn_prefix)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.kof_id)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.kof).WithMany(p => p.FW_Kode_Counters)
                .HasForeignKey(d => d.kof_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_New_Kode_Counter_New_Kode_Format");
        });

        modelBuilder.Entity<FW_Kode_Detail>(entity =>
        {
            entity.HasKey(e => e.kod_id).HasName("PK_New_Kode_Detail");

            entity.ToTable("FW_Kode_Detail");

            entity.Property(e => e.kod_catatan)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.kod_char)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.kod_param_kode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.kod_tipe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.kof_id)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.kof).WithMany(p => p.FW_Kode_Details)
                .HasForeignKey(d => d.kof_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_New_Kode_Detail_New_Kode_Format");
        });

        modelBuilder.Entity<FW_Kode_Format>(entity =>
        {
            entity.HasKey(e => e.kof_id).HasName("PK_Kode_Format_New");

            entity.ToTable("FW_Kode_Format");

            entity.Property(e => e.kof_id)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.kof_catatan)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.kof_reset_time).HasColumnType("datetime");
            entity.Property(e => e.kof_reset_tp)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<FW_LOG_Error>(entity =>
        {
            entity.HasKey(e => e.err_id).HasName("PK_LOG_Error");

            entity.ToTable("FW_LOG_Error");

            entity.Property(e => e.err_date).HasColumnType("datetime");
            entity.Property(e => e.err_description).IsUnicode(false);
            entity.Property(e => e.err_ip_address)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.err_message).IsUnicode(false);
            entity.Property(e => e.err_user_id)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FW_Notice>(entity =>
        {
            entity.HasKey(e => e.notice_id).HasName("PK_Notice");

            entity.ToTable("FW_Notice");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.notice_attach_link)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.notice_attach_link2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.notice_attach_link3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.notice_batch_users)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_bcc)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_catatan)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_cc)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_content)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_rdef_param_csv)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.notice_ref_id)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_ref_id2)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_ref_id3)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_sender)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_small_content)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_title)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.notice_to)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.nott_id)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.nott).WithMany(p => p.FW_Notices)
                .HasForeignKey(d => d.nott_id)
                .HasConstraintName("FK_FW_Notice_FW_Notice_Template");
        });

        modelBuilder.Entity<FW_Notice_Queue>(entity =>
        {
            entity.HasKey(e => e.notq_id).HasName("PK_Notice_Queue");

            entity.ToTable("FW_Notice_Queue");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.notq_content).IsUnicode(false);
            entity.Property(e => e.notq_email).IsUnicode(false);
            entity.Property(e => e.notq_title).IsUnicode(false);
            entity.Property(e => e.notq_user)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.nott_date).HasColumnType("datetime");
            entity.Property(e => e.nott_id)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.nott_sent_date).HasColumnType("datetime");
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.emailq).WithMany(p => p.FW_Notice_Queues)
                .HasForeignKey(d => d.emailq_id)
                .HasConstraintName("FK_Notice_Queue_Email_Queue");

            entity.HasOne(d => d.notq_userNavigation).WithMany(p => p.FW_Notice_Queues)
                .HasForeignKey(d => d.notq_user)
                .HasConstraintName("FK_Notice_Queue_User_Master");

            entity.HasOne(d => d.nott).WithMany(p => p.FW_Notice_Queues)
                .HasForeignKey(d => d.nott_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notice_Queue_Notice_Template");
        });

        modelBuilder.Entity<FW_Notice_Template>(entity =>
        {
            entity.HasKey(e => e.nott_id).HasName("PK_Notice_Template");

            entity.ToTable("FW_Notice_Template");

            entity.Property(e => e.nott_id)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.nott_batch).IsUnicode(false);
            entity.Property(e => e.nott_bcc).IsUnicode(false);
            entity.Property(e => e.nott_catatan)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.nott_cc).IsUnicode(false);
            entity.Property(e => e.nott_content).IsUnicode(false);
            entity.Property(e => e.nott_group)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.nott_key_type)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.nott_last_test_id)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.nott_model_type)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.nott_rdef_param_csv)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.nott_ref_id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.nott_ref_id2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.nott_ref_id3)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.nott_sender).IsUnicode(false);
            entity.Property(e => e.nott_small_content)
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.nott_title)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.nott_to).IsUnicode(false);
            entity.Property(e => e.rdef_kode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.rdef_kodeNavigation).WithMany(p => p.FW_Notice_Templates)
                .HasForeignKey(d => d.rdef_kode)
                .HasConstraintName("FK_FW_Notice_Template_ReportDef");
        });

        modelBuilder.Entity<FW_Ref_Modul>(entity =>
        {
            entity.HasKey(e => e.mod_kode).HasName("PK_New_Ref_Modul");

            entity.ToTable("FW_Ref_Modul");

            entity.Property(e => e.mod_kode)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.mod_catatan)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.parent_mod_kode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<FW_Ref_Role>(entity =>
        {
            entity.HasKey(e => e.role_id).HasName("PK_New_Ref_Role");

            entity.ToTable("FW_Ref_Role", tb => tb.HasTrigger("trig_defaultRefModul"));

            entity.Property(e => e.role_id)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.role_catatan)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.role_scope)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<FW_Ref_Setting>(entity =>
        {
            entity.HasKey(e => e.set_name).HasName("PK_Ref_Setting_1");

            entity.ToTable("FW_Ref_Setting");

            entity.Property(e => e.set_name)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.set_catatan)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.set_group)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.set_type)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.set_value).IsUnicode(false);
        });

        modelBuilder.Entity<FW_Role_Right>(entity =>
        {
            entity.HasKey(e => e.right_id).HasName("PK_New_Role_Right");

            entity.ToTable("FW_Role_Right");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.mod_kode)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.role_id)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.mod_kodeNavigation).WithMany(p => p.FW_Role_Rights)
                .HasForeignKey(d => d.mod_kode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Role_Right_FW_Ref_Modul");

            entity.HasOne(d => d.role).WithMany(p => p.FW_Role_Rights)
                .HasForeignKey(d => d.role_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_Role_Right_FW_Ref_Role");
        });

        modelBuilder.Entity<FW_Temp_Table>(entity =>
        {
            entity.HasKey(e => e.temp_id).HasName("PK_Temp_Table");

            entity.ToTable("FW_Temp_Table");

            entity.Property(e => e.temp_id).ValueGeneratedNever();
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.user_id)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FW_User_Lost_Password>(entity =>
        {
            entity.HasKey(e => e.ulp_id).HasName("PK_User_Lost_Password");

            entity.ToTable("FW_User_Lost_Password");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.ulp_code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ulp_date).HasColumnType("datetime");
            entity.Property(e => e.ulp_email)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ulp_expire_date).HasColumnType("datetime");
            entity.Property(e => e.ulp_reset_date).HasColumnType("datetime");
            entity.Property(e => e.ulp_status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.user_id)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.user).WithMany(p => p.FW_User_Lost_Passwords)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Lost_Password_User_Master");
        });

        modelBuilder.Entity<FW_User_Role>(entity =>
        {
            entity.HasKey(e => e.urole_id).HasName("PK_New_User_Role");

            entity.ToTable("FW_User_Role", tb => tb.HasTrigger("trig_userRolesCsv"));

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.role_id)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.user_id)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.role).WithMany(p => p.FW_User_Roles)
                .HasForeignKey(d => d.role_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_User_Role_FW_Ref_Role");

            entity.HasOne(d => d.user).WithMany(p => p.FW_User_Roles)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FW_User_Role_User_Master");
        });

        modelBuilder.Entity<FW_Workday>(entity =>
        {
            entity.HasKey(e => e.workday_id).HasName("PK_Workday");

            entity.ToTable("FW_Workday");

            entity.Property(e => e.calendar_type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Standard");
            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.workday_dayName)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.workday_end).HasColumnType("datetime");
            entity.Property(e => e.workday_start).HasColumnType("datetime");
        });

        modelBuilder.Entity<Help>(entity =>
        {
            entity.HasKey(e => e.help_id);

            entity.ToTable("Help");

            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.help_catatan)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.help_last_update).HasColumnType("datetime");
            entity.Property(e => e.help_last_update_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.help_nama)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<Jual_Detail>(entity =>
        {
            entity.HasKey(e => e.juald_id);

            entity.ToTable("Jual_Detail");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.juald_disk).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.juald_harga).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.barang).WithMany(p => p.Jual_Details)
                .HasForeignKey(d => d.barang_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jual_Detail_Barang_Master");

            entity.HasOne(d => d.jualh).WithMany(p => p.Jual_Details)
                .HasForeignKey(d => d.jualh_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jual_Detail_Jual_Header");
        });

        modelBuilder.Entity<Jual_Header>(entity =>
        {
            entity.HasKey(e => e.jualh_id);

            entity.ToTable("Jual_Header");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.jualh_date).HasColumnType("datetime");
            entity.Property(e => e.jualh_kode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.outlet).WithMany(p => p.Jual_Headers)
                .HasForeignKey(d => d.outlet_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jual_Header_Outlet_Master");

            entity.HasOne(d => d.sales).WithMany(p => p.Jual_Headers)
                .HasForeignKey(d => d.sales_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jual_Header_Sales_Master");
        });

        modelBuilder.Entity<Klasifikasi_Master>(entity =>
        {
            entity.HasKey(e => e.klas_id);

            entity.ToTable("Klasifikasi_Master");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.klas_kode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("menggambarkan hubungan wilayah penjualan.\r\nContoh : city anak dari branch");
            entity.Property(e => e.klas_nama)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<LOV>(entity =>
        {
            entity.HasKey(e => e.lov_id);

            entity.ToTable("LOV");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.lov_kode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.lov_nama)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.lov_scope)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.AppDomainName).HasMaxLength(512);
            entity.Property(e => e.FormattedMessage).HasColumnType("ntext");
            entity.Property(e => e.MachineName).HasMaxLength(32);
            entity.Property(e => e.ProcessID).HasMaxLength(256);
            entity.Property(e => e.ProcessName).HasMaxLength(512);
            entity.Property(e => e.Severity).HasMaxLength(32);
            entity.Property(e => e.ThreadName).HasMaxLength(512);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(256);
            entity.Property(e => e.Win32ThreadId).HasMaxLength(128);
        });

        modelBuilder.Entity<Menu_Permission>(entity =>
        {
            entity.HasKey(e => e.menu_text).HasName("PK_Menu_Permission_1");

            entity.ToTable("Menu_Permission");

            entity.Property(e => e.menu_text)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.menu_permission)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Outlet_Master>(entity =>
        {
            entity.HasKey(e => e.outlet_id);

            entity.ToTable("Outlet_Master");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.outlet_kode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("menggambarkan hubungan wilayah penjualan.\r\nContoh : city anak dari branch");
            entity.Property(e => e.outlet_nama)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.post_id).HasName("PK_Budget_Post");

            entity.ToTable("Post");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.post_nama)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.post_tipe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.parent).WithMany(p => p.Inverseparent)
                .HasForeignKey(d => d.parent_id)
                .HasConstraintName("FK_Budget_Post_Budget_Post");
        });

        modelBuilder.Entity<Post_Child>(entity =>
        {
            entity.HasKey(e => e.pc_id);

            entity.HasOne(d => d.child_post).WithMany(p => p.Post_Childchild_posts)
                .HasForeignKey(d => d.child_post_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_Children_Budget_Post1");

            entity.HasOne(d => d.post).WithMany(p => p.Post_Childposts)
                .HasForeignKey(d => d.post_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_Children_Budget_Post");
        });

        modelBuilder.Entity<ReportDef>(entity =>
        {
            entity.HasKey(e => e.rdef_kode).HasName("PK_ReportDef_1");

            entity.ToTable("ReportDef");

            entity.Property(e => e.rdef_kode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.rdef_def).HasColumnType("image");
            entity.Property(e => e.rdef_nama)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<Sales_Master>(entity =>
        {
            entity.HasKey(e => e.sales_id);

            entity.ToTable("Sales_Master");

            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.modified_by)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.sales_kode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("menggambarkan hubungan wilayah penjualan.\r\nContoh : city anak dari branch");
            entity.Property(e => e.sales_nama)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();

            entity.HasOne(d => d.outlet).WithMany(p => p.Sales_Masters)
                .HasForeignKey(d => d.outlet_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sales_Master_Outlet_Master");
        });

        modelBuilder.Entity<StructDivision>(entity =>
        {
            entity.ToTable("StructDivision");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(256);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_StructDivision_StructDivision");
        });

        modelBuilder.Entity<Time>(entity =>
        {
            entity.HasKey(e => e.PK_Date);

            entity.ToTable("Time");

            entity.Property(e => e.PK_Date).HasColumnType("datetime");
            entity.Property(e => e.Date_Name).HasMaxLength(50);
            entity.Property(e => e.Day_Of_Month_Name).HasMaxLength(50);
            entity.Property(e => e.Day_Of_Quarter_Name).HasMaxLength(50);
            entity.Property(e => e.Day_Of_Week_Name).HasMaxLength(50);
            entity.Property(e => e.Day_Of_Year_Name).HasMaxLength(50);
            entity.Property(e => e.Month).HasColumnType("datetime");
            entity.Property(e => e.Month_Name).HasMaxLength(50);
            entity.Property(e => e.Month_Of_Quarter_Name).HasMaxLength(50);
            entity.Property(e => e.Month_Of_Year_Name).HasMaxLength(50);
            entity.Property(e => e.Quarter).HasColumnType("datetime");
            entity.Property(e => e.Quarter_Name).HasMaxLength(50);
            entity.Property(e => e.Quarter_Of_Year_Name).HasMaxLength(50);
            entity.Property(e => e.Week).HasColumnType("datetime");
            entity.Property(e => e.Week_Name).HasMaxLength(50);
            entity.Property(e => e.Week_Of_Year_Name).HasMaxLength(50);
            entity.Property(e => e.Year).HasColumnType("datetime");
            entity.Property(e => e.Year_Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User_Master>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PK_User_Master2");

            entity.ToTable("User_Master");

            entity.Property(e => e.user_id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastTimeCookies).HasColumnType("datetime");
            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.ip_address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.user_agent)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.user_alamat)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.user_blocked_date).HasColumnType("datetime");
            entity.Property(e => e.user_delegate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.user_divisi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.user_email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.user_kode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.user_last_login).HasColumnType("datetime");
            entity.Property(e => e.user_ldap).IsUnicode(false);
            entity.Property(e => e.user_ldap_department)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.user_ldap_description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.user_ldap_office)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.user_main_role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.user_nama)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.user_password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.user_password_lastchange).HasColumnType("datetime");
            entity.Property(e => e.user_roles_csv)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.user_status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Aktif");
            entity.Property(e => e.user_telp)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.post).WithMany(p => p.User_Masters)
                .HasForeignKey(d => d.post_id)
                .HasConstraintName("FK_User_Master_Post");
        });

        modelBuilder.Entity<User_Refresh_Token>(entity =>
        {
            entity.HasKey(e => e.refresh_token_id).HasName("PK__User_Ref__B0A1F7C7D609760D");

            entity.ToTable("User_Refresh_Token");

            entity.HasIndex(e => e.date_expires, "IX_User_Refresh_Token_Expires");

            entity.HasIndex(e => e.stsrc, "IX_User_Refresh_Token_IsActive");

            entity.HasIndex(e => e.refresh_token, "IX_User_Refresh_Token_RefreshToken");

            entity.HasIndex(e => e.user_id, "IX_User_Refresh_Token_UserId");

            entity.Property(e => e.access_token).HasMaxLength(2048);
            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_created).HasColumnType("datetime");
            entity.Property(e => e.date_expires).HasColumnType("datetime");
            entity.Property(e => e.date_modified).HasColumnType("datetime");
            entity.Property(e => e.date_updated).HasColumnType("datetime");
            entity.Property(e => e.device_info).HasMaxLength(255);
            entity.Property(e => e.ip_address).HasMaxLength(45);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.refresh_token).HasMaxLength(500);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.user_id)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.user).WithMany(p => p.User_Refresh_Tokens)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_User_Refresh_Token_User_Id");
        });

        modelBuilder.Entity<WorkflowGlobalParameter>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("WorkflowGlobalParameter");

            entity.HasIndex(e => new { e.Type, e.Name }, "IX_Type_Name_Clustered")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Type).HasMaxLength(306);
        });

        modelBuilder.Entity<WorkflowInbox>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("WorkflowInbox");

            entity.HasIndex(e => e.IdentityId, "IX_IdentityId_Clustered").IsClustered();

            entity.HasIndex(e => e.ProcessId, "IX_ProcessId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.IdentityId).HasMaxLength(256);
        });

        modelBuilder.Entity<WorkflowProcessInstance>(entity =>
        {
            entity.ToTable("WorkflowProcessInstance");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<WorkflowProcessInstancePersistence>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("WorkflowProcessInstancePersistence");

            entity.HasIndex(e => e.ProcessId, "IX_ProcessId_Clustered").IsClustered();

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<WorkflowProcessInstanceStatus>(entity =>
        {
            entity.ToTable("WorkflowProcessInstanceStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<WorkflowProcessScheme>(entity =>
        {
            entity.ToTable("WorkflowProcessScheme");

            entity.HasIndex(e => new { e.SchemeCode, e.DefiningParametersHash, e.IsObsolete }, "IX_SchemeCode_Hash_IsObsolete");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DefiningParameters).HasColumnType("ntext");
            entity.Property(e => e.DefiningParametersHash).HasMaxLength(24);
            entity.Property(e => e.RootSchemeCode).HasMaxLength(256);
            entity.Property(e => e.Scheme).HasColumnType("ntext");
            entity.Property(e => e.SchemeCode).HasMaxLength(256);
        });

        modelBuilder.Entity<WorkflowProcessTimer>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("WorkflowProcessTimer");

            entity.HasIndex(e => e.NextExecutionDateTime, "IX_NextExecutionDateTime_Clustered").IsClustered();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NextExecutionDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkflowProcessTransitionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("WorkflowProcessTransitionHistory");

            entity.HasIndex(e => e.ExecutorIdentityId, "IX_ExecutorIdentityId");

            entity.HasIndex(e => e.ProcessId, "IX_ProcessId_Clustered").IsClustered();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ActorIdentityId).HasMaxLength(256);
            entity.Property(e => e.ExecutorIdentityId).HasMaxLength(256);
            entity.Property(e => e.TransitionTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkflowScheme>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("WorkflowScheme");

            entity.Property(e => e.Code).HasMaxLength(256);
        });

        modelBuilder.Entity<vHead>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vHeads");

            entity.Property(e => e.HeadName).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<vStructDivisionParent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vStructDivisionParents");
        });

        modelBuilder.Entity<vStructDivisionParentsAndThi>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vStructDivisionParentsAndThis");
        });

        modelBuilder.Entity<vw_UserPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_UserPermission");

            entity.Property(e => e.mod_kode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.role_id)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.user_id)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
