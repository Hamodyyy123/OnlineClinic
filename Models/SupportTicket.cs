using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class SupportTicket
{
    public int SupportTicketId { get; set; }

    public int? UserId { get; set; }

    public string? Message { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}
