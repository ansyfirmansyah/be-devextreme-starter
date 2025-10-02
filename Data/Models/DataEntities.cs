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

    public virtual DbSet<City_Master> City_Masters { get; set; }

    public virtual DbSet<Contact_Master> Contact_Masters { get; set; }

    public virtual DbSet<Contact_Status_Master> Contact_Status_Masters { get; set; }

    public virtual DbSet<Country_Master> Country_Masters { get; set; }

    public virtual DbSet<FW_Ref_Modul> FW_Ref_Moduls { get; set; }

    public virtual DbSet<FW_Ref_Role> FW_Ref_Roles { get; set; }

    public virtual DbSet<FW_Ref_Setting> FW_Ref_Settings { get; set; }

    public virtual DbSet<FW_Role_Right> FW_Role_Rights { get; set; }

    public virtual DbSet<FW_Temp_Table> FW_Temp_Tables { get; set; }

    public virtual DbSet<FW_User_Role> FW_User_Roles { get; set; }

    public virtual DbSet<Jual_Detail> Jual_Details { get; set; }

    public virtual DbSet<Jual_Header> Jual_Headers { get; set; }

    public virtual DbSet<Klasifikasi_Master> Klasifikasi_Masters { get; set; }

    public virtual DbSet<Lead_Source_Master> Lead_Source_Masters { get; set; }

    public virtual DbSet<Outlet_Master> Outlet_Masters { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sales_Master> Sales_Masters { get; set; }

    public virtual DbSet<User_Master> User_Masters { get; set; }

    public virtual DbSet<User_Refresh_Token> User_Refresh_Tokens { get; set; }

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

        modelBuilder.Entity<City_Master>(entity =>
        {
            entity.HasKey(e => e.city_id).HasName("PK__City_Mas__031491A8B1447E8E");

            entity.ToTable("City_Master");

            entity.Property(e => e.city_name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.country).WithMany(p => p.City_Masters)
                .HasForeignKey(d => d.country_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Country");
        });

        modelBuilder.Entity<Contact_Master>(entity =>
        {
            entity.HasKey(e => e.contact_id).HasName("PK__Contact___024E7A86394A5283");

            entity.ToTable("Contact_Master");

            entity.HasIndex(e => e.email, "UQ__Contact___AB6E61647E7D41C1").IsUnique();

            entity.Property(e => e.address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.company)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.created_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.date_added).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.estimated_value).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.full_name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.job_title)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.modified_by)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.notes).IsUnicode(false);
            entity.Property(e => e.phone_number)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.stsrc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A");

            entity.HasOne(d => d.city).WithMany(p => p.Contact_Masters)
                .HasForeignKey(d => d.city_id)
                .HasConstraintName("FK_Contact_City");

            entity.HasOne(d => d.contact_status).WithMany(p => p.Contact_Masters)
                .HasForeignKey(d => d.contact_status_id)
                .HasConstraintName("FK_Contact_Status");

            entity.HasOne(d => d.lead_source).WithMany(p => p.Contact_Masters)
                .HasForeignKey(d => d.lead_source_id)
                .HasConstraintName("FK_Contact_LeadSource");
        });

        modelBuilder.Entity<Contact_Status_Master>(entity =>
        {
            entity.HasKey(e => e.contact_status_id).HasName("PK__Contact___D6C66E3F585A6EB7");

            entity.ToTable("Contact_Status_Master");

            entity.Property(e => e.contact_status_name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Country_Master>(entity =>
        {
            entity.HasKey(e => e.country_id).HasName("PK__Country___7E8CD055B0305840");

            entity.ToTable("Country_Master");

            entity.Property(e => e.country_name)
                .HasMaxLength(100)
                .IsUnicode(false);
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

            entity.ToTable("FW_Ref_Role");

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

        modelBuilder.Entity<FW_User_Role>(entity =>
        {
            entity.HasKey(e => e.urole_id).HasName("PK_New_User_Role");

            entity.ToTable("FW_User_Role");

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

        modelBuilder.Entity<Lead_Source_Master>(entity =>
        {
            entity.HasKey(e => e.lead_source_id).HasName("PK__Lead_Sou__02D65A87CDCDB0C2");

            entity.ToTable("Lead_Source_Master");

            entity.Property(e => e.lead_source_name)
                .HasMaxLength(100)
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
        });

        modelBuilder.Entity<User_Refresh_Token>(entity =>
        {
            entity.HasKey(e => e.refresh_token_id).HasName("PK__User_Ref__B0A1F7C7D06C746D");

            entity.ToTable("User_Refresh_Token");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
