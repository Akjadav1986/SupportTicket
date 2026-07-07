using System.ComponentModel.DataAnnotations;

namespace SupportTicketClassifier.Api.Dtos;

public sealed class CreateTicketRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string CustomerEmail { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProductName { get; set; }
}
