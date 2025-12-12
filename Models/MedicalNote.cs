using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class MedicalNote
{
    public int NoteId { get; set; }

    public int? PatientId { get; set; }

    public int? AuthorId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? NoteType { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? Author { get; set; }

    public virtual Patient? Patient { get; set; }
}
