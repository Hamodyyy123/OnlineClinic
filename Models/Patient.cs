using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public string? ContactInfo { get; set; }

    public string? EmergencyContact { get; set; }

    public DateOnly? AdmissionDate { get; set; }

    public string? RoomNumber { get; set; }

    public int? AssignedDoctor { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User? AssignedDoctorNavigation { get; set; }

    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual ICollection<MedicalNote> MedicalNotes { get; set; } = new List<MedicalNote>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User? User { get; set; }

    public virtual ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
}
