using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("users")]
[Index("Email", Name = "UQ__users__AB6E6164064D5E98", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("role")]
    [StringLength(20)]
    public string Role { get; set; } = null!;

    [Column("status")]
    [StringLength(10)]
    public string? Status { get; set; }

    [Column("last_login", TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("AssignedUser")]
    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<BackupRecord> BackupRecords { get; set; } = new List<BackupRecord>();

    [InverseProperty("User")]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    [InverseProperty("Author")]
    public virtual ICollection<MedicalNote> MedicalNotes { get; set; } = new List<MedicalNote>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("AssignedDoctorNavigation")]
    public virtual ICollection<Patient> PatientAssignedDoctorNavigations { get; set; } = new List<Patient>();

    [InverseProperty("User")]
    public virtual ICollection<Patient> PatientUsers { get; set; } = new List<Patient>();

    [InverseProperty("GeneratedByNavigation")]
    public virtual ICollection<Report> ReportGeneratedByNavigations { get; set; } = new List<Report>();

    [InverseProperty("SignedByNavigation")]
    public virtual ICollection<Report> ReportSignedByNavigations { get; set; } = new List<Report>();

    [InverseProperty("User")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("User")]
    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
