using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

public partial class OnlineClinicContext : DbContext
{
    public OnlineClinicContext()
    {
    }

    public OnlineClinicContext(DbContextOptions<OnlineClinicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BackupRecord> BackupRecords { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<HelpFaq> HelpFaqs { get; set; }

    public virtual DbSet<MedicalHistory> MedicalHistories { get; set; }

    public virtual DbSet<MedicalNote> MedicalNotes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VitalSign> VitalSigns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-AFMPLMB;Database=OnlineClinic;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.AlertId).HasName("PK__alerts__4B8FB03AD9E24324");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("new");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.Alerts).HasConstraintName("FK__alerts__assigned__571DF1D5");

            entity.HasOne(d => d.Patient).WithMany(p => p.Alerts).HasConstraintName("FK__alerts__patient___5441852A");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__appointm__A50828FCCB3D0BD2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointments_doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointments_patients");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__audit_lo__9E2397E05C701C51");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs).HasConstraintName("FK__audit_log__user___71D1E811");
        });

        modelBuilder.Entity<BackupRecord>(entity =>
        {
            entity.HasKey(e => e.BackupId).HasName("PK__backup_r__AE70C880FA313354");

            entity.Property(e => e.BackupDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Restored).HasDefaultValue(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BackupRecords).HasConstraintName("FK__backup_re__creat__00200768");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PK__devices__3B085D8BA5DB3F9E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("active");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Patient).WithMany(p => p.Devices).HasConstraintName("FK__devices__patient__49C3F6B7");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__doctors__F39935645485CA23");

            entity.HasOne(d => d.User).WithMany(p => p.Doctors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__doctors__user_id__2BFE89A6");
        });

        modelBuilder.Entity<HelpFaq>(entity =>
        {
            entity.HasKey(e => e.FaqId).HasName("PK__help_faq__66734BAF8BA68108");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__medical___096AA2E95F5B5459");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalHistories).HasConstraintName("FK__medical_h__patie__440B1D61");
        });

        modelBuilder.Entity<MedicalNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__medical___CEDD0FA42E0C5CB0");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Author).WithMany(p => p.MedicalNotes).HasConstraintName("FK__medical_n__autho__60A75C0F");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalNotes).HasConstraintName("FK__medical_n__patie__5FB337D6");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F83608D2A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Alert).WithMany(p => p.Notifications).HasConstraintName("FK__notificat__alert__5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasConstraintName("FK__notificat__user___5AEE82B9");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__patients__4D5CE4763272571B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("active");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AssignedDoctorNavigation).WithMany(p => p.PatientAssignedDoctorNavigations).HasConstraintName("FK__patients__assign__3E52440B");

            entity.HasOne(d => d.User).WithMany(p => p.PatientUsers).HasConstraintName("FK__patients__user_i__3D5E1FD2");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__permissi__E5331AFA96B708E3");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__reports__779B7C58467D6673");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Printable).HasDefaultValue(false);

            entity.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.ReportGeneratedByNavigations).HasConstraintName("FK__reports__generat__66603565");

            entity.HasOne(d => d.Patient).WithMany(p => p.Reports).HasConstraintName("FK__reports__patient__656C112C");

            entity.HasOne(d => d.SignedByNavigation).WithMany(p => p.ReportSignedByNavigations).HasConstraintName("FK__reports__signed___68487DD7");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role_per__3213E83FFE3DB2B9");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions).HasConstraintName("FK__role_perm__permi__7B5B524B");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions).HasConstraintName("FK__role_perm__role___7A672E12");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__sessions__69B13FDC9B278DF2");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LoginTime).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions).HasConstraintName("FK__sessions__user_i__6C190EBB");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.SupportTicketId).HasName("PK__support___34FDECE5D9E35819");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("open");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTickets).HasConstraintName("FK__support_t__user___05D8E0BE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F1EDDE587");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("active");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__user_rol__760965CCBF4CE54D");
        });

        modelBuilder.Entity<VitalSign>(entity =>
        {
            entity.HasKey(e => e.VitalId).HasName("PK__vital_si__4DF3C4716CC1E516");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Device).WithMany(p => p.VitalSigns).HasConstraintName("FK__vital_sig__devic__5070F446");

            entity.HasOne(d => d.Patient).WithMany(p => p.VitalSigns).HasConstraintName("FK__vital_sig__patie__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
