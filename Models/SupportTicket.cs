using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("support_tickets")]
public partial class SupportTicket
{
    [Key]
    [Column("support_ticket_id")]
    public int SupportTicketId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("message")]
    public string? Message { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("SupportTickets")]
    public virtual User? User { get; set; }
}
