using SupportTicketClassifier.Api.Dtos;
using SupportTicketClassifier.Api.Models;

namespace SupportTicketClassifier.Api.Agents;

public interface ITicketClassificationAgent
{
    Task<ClassificationResultDto> ClassifyAsync(Ticket ticket, CancellationToken cancellationToken = default);
}
