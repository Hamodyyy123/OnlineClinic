using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class BackupRecord
{
    public int BackupId { get; set; }

    public DateTime? BackupDate { get; set; }

    public string? BackupFilePath { get; set; }

    public bool? Restored { get; set; }

    public DateTime? RestoredAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
