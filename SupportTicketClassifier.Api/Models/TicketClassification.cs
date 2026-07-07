using System.ComponentModel.DataAnnotations;

namespace SupportTicketClassifier.Api.Models;

public sealed class TicketClassification
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    [Required]
    [MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Priority { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string RoutedTeam { get; set; } = string.Empty;

    public decimal Confidence { get; set; }

    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;

    public string TagsJson { get; set; } = "[]";

    public DateTime ClassifiedAtUtc { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
}
