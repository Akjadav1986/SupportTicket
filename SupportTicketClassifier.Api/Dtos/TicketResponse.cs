namespace SupportTicketClassifier.Api.Dtos;

public sealed class TicketResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public ClassificationResultDto? Classification { get; set; }
}
