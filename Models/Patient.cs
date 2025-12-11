using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("patients")]
public partial class Patient
{
    [Key]
    [Column("patient_id")]
    public int PatientId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("age")]
    public int? Age { get; set; }

    [Column("gender")]
    [StringLength(10)]
    public string? Gender { get; set; }

    [Column("contact_info")]
    [StringLength(255)]
    public string? ContactInfo { get; set; }

    [Column("emergency_contact")]
    [StringLength(255)]
    public string? EmergencyContact { get; set; }

    [Column("admission_date")]
    public DateOnly? AdmissionDate { get; set; }

    [Column("room_number")]
    [StringLength(10)]
    public string? RoomNumber { get; set; }

    [Column("assigned_doctor")]
    public int? AssignedDoctor { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [ForeignKey("AssignedDoctor")]
    [InverseProperty("PatientAssignedDoctorNavigations")]
    public virtual User? AssignedDoctorNavigation { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    [InverseProperty("Patient")]
    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    [InverseProperty("Patient")]
    public virtual ICollection<MedicalNote> MedicalNotes { get; set; } = new List<MedicalNote>();

    [InverseProperty("Patient")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("UserId")]
    [InverseProperty("PatientUsers")]
    public virtual User? User { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
}
