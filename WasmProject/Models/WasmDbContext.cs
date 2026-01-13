using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace WasmProject.Models;

public partial class WasmDbContext : DbContext
{
    public WasmDbContext()
    {
    }

    public WasmDbContext(DbContextOptions<WasmDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LostItem> LostItems { get; set; }

    public virtual DbSet<PendingSync> PendingSyncs { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<SyncLog> SyncLogs { get; set; }

    public virtual DbSet<SystemAdmin> SystemAdmins { get; set; }

    public virtual DbSet<TrustedStore> TrustedStores { get; set; }

    public virtual DbSet<UserDb> UserDbs { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost;Database=WasmDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LostItem>(entity =>
        {
            entity.HasKey(e => e.LostItemId).HasName("PK__LostItem__3B3BF43281CC2224");

            entity.Property(e => e.LostItemId).HasColumnName("LostItemID");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.LostDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");

            entity.HasOne(d => d.Property).WithMany(p => p.LostItems)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__LostItems__Prope__59FA5E80");
        });

        modelBuilder.Entity<PendingSync>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PendingS__3214EC07613A28C0");

            entity.ToTable("PendingSync");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SenderId).HasMaxLength(50);
            entity.Property(e => e.SourceType).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserPhone).HasMaxLength(20);
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.PropertyId).HasName("PK__Properti__70C9A755B5658AC4");

            entity.HasIndex(e => e.SerialNumber, "UQ__Properti__048A0008D133D1D0").IsUnique();

            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExternalReferenceId).HasMaxLength(255);
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.SourceIdentifier).HasMaxLength(100);
            entity.Property(e => e.SourceType).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Safe");
            entity.Property(e => e.SyncTimestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VerifiedStoreName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Properties)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Propertie__UserI");
        });

        modelBuilder.Entity<SyncLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SyncLogs__3214EC0732244D7B");

            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.LogTimestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<SystemAdmin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__SystemAd__719FE4E8079FE4EF");

            entity.HasIndex(e => e.AdminUser, "UQ__SystemAd__8851A4AB6CA17CDF").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__SystemAd__A9D10534BBA1A1FB").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.AdminName).HasMaxLength(100);
            entity.Property(e => e.AdminUser).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<TrustedStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TrustedS__3214EC076EB43848");

            entity.HasIndex(e => e.SenderId, "UQ__TrustedS__BB49918A83CFBDA2").IsUnique();

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SenderId).HasMaxLength(50);
        });

        modelBuilder.Entity<UserDb>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PK__UserDB__1788CCACDEA010C7");

            entity.ToTable("UserDB");

            entity.HasIndex(e => e.UserName, "UQ__UserDB__C9F284560A75B6D9").IsUnique();

            entity.HasIndex(e => e.NationalID, "UQ__UserDB__E9AA321A23090FDD").IsUnique();

            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.NationalID)
                .HasMaxLength(10)
                .HasColumnName("NationalID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
