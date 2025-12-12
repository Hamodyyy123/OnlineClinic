using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<BackupRecord> BackupRecords { get; set; } = new List<BackupRecord>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<MedicalNote> MedicalNotes { get; set; } = new List<MedicalNote>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Patient> PatientAssignedDoctorNavigations { get; set; } = new List<Patient>();

    public virtual ICollection<Patient> PatientUsers { get; set; } = new List<Patient>();

    public virtual ICollection<Report> ReportGeneratedByNavigations { get; set; } = new List<Report>();

    public virtual ICollection<Report> ReportSignedByNavigations { get; set; } = new List<Report>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
