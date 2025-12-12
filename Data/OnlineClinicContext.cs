using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OnlineClinic.Models;

namespace OnlineClinic.Data;

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

    public virtual DbSet<Consultation> Consultations { get; set; }

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
        => optionsBuilder.UseSqlServer("Server=DESKTOP-AFMPLMB;Database=OnlineClinic;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.AlertId).HasName("PK__alerts__4B8FB03AD9E24324");

            entity.ToTable("alerts");

            entity.Property(e => e.AlertId).HasColumnName("alert_id");
            entity.Property(e => e.AcknowledgedAt)
                .HasColumnType("datetime")
                .HasColumnName("acknowledged_at");
            entity.Property(e => e.AlertType)
                .HasMaxLength(50)
                .HasColumnName("alert_type");
            entity.Property(e => e.AssignedUserId).HasColumnName("assigned_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ReadingValue)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("reading_value");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("new")
                .HasColumnName("status");
            entity.Property(e => e.Threshold)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("threshold");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.VitalType)
                .HasMaxLength(50)
                .HasColumnName("vital_type");

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.AssignedUserId)
                .HasConstraintName("FK__alerts__assigned__571DF1D5");

            entity.HasOne(d => d.Patient).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__alerts__patient___5441852A");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__appointm__A50828FCCB3D0BD2");

            entity.ToTable("appointments");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointments_doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_appointments_patients");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__audit_lo__9E2397E05C701C51");

            entity.ToTable("audit_logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.ResourceType)
                .HasMaxLength(50)
                .HasColumnName("resource_type");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__audit_log__user___71D1E811");
        });

        modelBuilder.Entity<BackupRecord>(entity =>
        {
            entity.HasKey(e => e.BackupId).HasName("PK__backup_r__AE70C880FA313354");

            entity.ToTable("backup_records");

            entity.Property(e => e.BackupId).HasColumnName("backup_id");
            entity.Property(e => e.BackupDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("backup_date");
            entity.Property(e => e.BackupFilePath)
                .HasMaxLength(255)
                .HasColumnName("backup_file_path");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Restored)
                .HasDefaultValue(false)
                .HasColumnName("restored");
            entity.Property(e => e.RestoredAt)
                .HasColumnType("datetime")
                .HasColumnName("restored_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BackupRecords)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__backup_re__creat__00200768");
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(e => e.ConsultationId).HasName("PK__Consulta__5D014A98A7FB6A84");

            entity.Property(e => e.ChiefComplaint).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Diagnosis).HasMaxLength(2000);
            entity.Property(e => e.Medications).HasMaxLength(2000);
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Open");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consultations_Appointments");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consultations_Doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consultations_Patients");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PK__devices__3B085D8BA5DB3F9E");

            entity.ToTable("devices");

            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceType)
                .HasMaxLength(50)
                .HasColumnName("device_type");
            entity.Property(e => e.LastSeen)
                .HasColumnType("datetime")
                .HasColumnName("last_seen");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Patient).WithMany(p => p.Devices)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__devices__patient__49C3F6B7");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__doctors__F39935645485CA23");

            entity.ToTable("doctors");

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Speciality)
                .HasMaxLength(50)
                .HasColumnName("speciality");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__doctors__user_id__2BFE89A6");
        });

        modelBuilder.Entity<HelpFaq>(entity =>
        {
            entity.HasKey(e => e.FaqId).HasName("PK__help_faq__66734BAF8BA68108");

            entity.ToTable("help_faqs");

            entity.Property(e => e.FaqId).HasColumnName("faq_id");
            entity.Property(e => e.Answer).HasColumnName("answer");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Question).HasColumnName("question");
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__medical___096AA2E95F5B5459");

            entity.ToTable("medical_history");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Diagnosis)
                .HasMaxLength(100)
                .HasColumnName("diagnosis");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalHistories)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__medical_h__patie__440B1D61");
        });

        modelBuilder.Entity<MedicalNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__medical___CEDD0FA42E0C5CB0");

            entity.ToTable("medical_notes");

            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.NoteType)
                .HasMaxLength(50)
                .HasColumnName("note_type");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Author).WithMany(p => p.MedicalNotes)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__medical_n__autho__60A75C0F");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalNotes)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__medical_n__patie__5FB337D6");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F83608D2A");

            entity.ToTable("notifications");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.AlertId).HasColumnName("alert_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveredAt)
                .HasColumnType("datetime")
                .HasColumnName("delivered_at");
            entity.Property(e => e.DeliveryMethod)
                .HasMaxLength(20)
                .HasColumnName("delivery_method");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Alert).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AlertId)
                .HasConstraintName("FK__notificat__alert__5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__notificat__user___5AEE82B9");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__patients__4D5CE4763272571B");

            entity.ToTable("patients");

            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.AdmissionDate).HasColumnName("admission_date");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.AssignedDoctor).HasColumnName("assigned_doctor");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(255)
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EmergencyContact)
                .HasMaxLength(255)
                .HasColumnName("emergency_contact");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(10)
                .HasColumnName("room_number");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AssignedDoctorNavigation).WithMany(p => p.PatientAssignedDoctorNavigations)
                .HasForeignKey(d => d.AssignedDoctor)
                .HasConstraintName("FK__patients__assign__3E52440B");

            entity.HasOne(d => d.User).WithMany(p => p.PatientUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__patients__user_i__3D5E1FD2");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__permissi__E5331AFA96B708E3");

            entity.ToTable("permissions");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(50)
                .HasColumnName("permission_name");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__reports__779B7C58467D6673");

            entity.ToTable("reports");

            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateRangeEnd).HasColumnName("date_range_end");
            entity.Property(e => e.DateRangeStart).HasColumnName("date_range_start");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.GeneratedBy).HasColumnName("generated_by");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Printable)
                .HasDefaultValue(false)
                .HasColumnName("printable");
            entity.Property(e => e.ReportType)
                .HasMaxLength(50)
                .HasColumnName("report_type");
            entity.Property(e => e.SignedBy).HasColumnName("signed_by");

            entity.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.ReportGeneratedByNavigations)
                .HasForeignKey(d => d.GeneratedBy)
                .HasConstraintName("FK__reports__generat__66603565");

            entity.HasOne(d => d.Patient).WithMany(p => p.Reports)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__reports__patient__656C112C");

            entity.HasOne(d => d.SignedByNavigation).WithMany(p => p.ReportSignedByNavigations)
                .HasForeignKey(d => d.SignedBy)
                .HasConstraintName("FK__reports__signed___68487DD7");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role_per__3213E83FFE3DB2B9");

            entity.ToTable("role_permission");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK__role_perm__permi__7B5B524B");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__role_perm__role___7A672E12");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__sessions__69B13FDC9B278DF2");

            entity.ToTable("sessions");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("login_time");
            entity.Property(e => e.LogoutTime)
                .HasColumnType("datetime")
                .HasColumnName("logout_time");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__sessions__user_i__6C190EBB");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.SupportTicketId).HasName("PK__support___34FDECE5D9E35819");

            entity.ToTable("support_tickets");

            entity.Property(e => e.SupportTicketId).HasColumnName("support_ticket_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("open")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__support_t__user___05D8E0BE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F1EDDE587");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164064D5E98").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("active")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__user_rol__760965CCBF4CE54D");

            entity.ToTable("user_roles");

            entity.HasIndex(e => e.RoleName, "UQ__user_rol__783254B192F48EF0").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<VitalSign>(entity =>
        {
            entity.HasKey(e => e.VitalId).HasName("PK__vital_si__4DF3C4716CC1E516");

            entity.ToTable("vital_signs");

            entity.Property(e => e.VitalId).HasColumnName("vital_id");
            entity.Property(e => e.BloodPressureDiastolic).HasColumnName("blood_pressure_diastolic");
            entity.Property(e => e.BloodPressureSystolic).HasColumnName("blood_pressure_systolic");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.HeartRate).HasColumnName("heart_rate");
            entity.Property(e => e.OxygenSaturation).HasColumnName("oxygen_saturation");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Source)
                .HasMaxLength(20)
                .HasColumnName("source");
            entity.Property(e => e.Temperature)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("temperature");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Device).WithMany(p => p.VitalSigns)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK__vital_sig__devic__5070F446");

            entity.HasOne(d => d.Patient).WithMany(p => p.VitalSigns)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__vital_sig__patie__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
