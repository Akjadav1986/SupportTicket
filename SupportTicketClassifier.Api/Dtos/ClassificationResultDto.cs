namespace SupportTicketClassifier.Api.Dtos;

public sealed class ClassificationResultDto
{
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string RoutedTeam { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
}
