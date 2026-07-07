using System.ComponentModel.DataAnnotations;

namespace SupportTicketClassifier.Api.Models;

public sealed class Ticket
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string CustomerEmail { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProductName { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.New;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public TicketClassification? Classification { get; set; }
}
