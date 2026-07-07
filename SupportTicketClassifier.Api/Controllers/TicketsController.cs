using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketClassifier.Api.Agents;
using SupportTicketClassifier.Api.Data;
using SupportTicketClassifier.Api.Dtos;
using SupportTicketClassifier.Api.Models;

namespace SupportTicketClassifier.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TicketsController(
    AppDbContext dbContext,
    ITicketClassificationAgent classificationAgent) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tickets = await dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Classification)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(tickets.Select(MapToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var ticket = await dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Classification)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (ticket is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(ticket));
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> Create(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            ProductName = request.ProductName?.Trim(),
            Status = TicketStatus.New,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        var classificationResult = await classificationAgent.ClassifyAsync(ticket, cancellationToken);

        ticket.Status = TicketStatus.Classified;
        ticket.Classification = new TicketClassification
        {
            TicketId = ticket.Id,
            Category = classificationResult.Category,
            Priority = classificationResult.Priority,
            RoutedTeam = classificationResult.RoutedTeam,
            Confidence = classificationResult.Confidence,
            Reason = classificationResult.Reason,
            TagsJson = JsonSerializer.Serialize(classificationResult.Tags),
            ClassifiedAtUtc = DateTime.UtcNow
        };

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(ticket);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, response);
    }

    [HttpPost("{id:int}/classify")]
    public async Task<ActionResult<TicketResponse>> Reclassify(int id, CancellationToken cancellationToken)
    {
        var ticket = await dbContext.Tickets
            .Include(x => x.Classification)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (ticket is null)
        {
            return NotFound();
        }

        var classificationResult = await classificationAgent.ClassifyAsync(ticket, cancellationToken);

        if (ticket.Classification is null)
        {
            ticket.Classification = new TicketClassification { TicketId = ticket.Id };
        }

        ticket.Status = TicketStatus.Classified;
        ticket.Classification.Category = classificationResult.Category;
        ticket.Classification.Priority = classificationResult.Priority;
        ticket.Classification.RoutedTeam = classificationResult.RoutedTeam;
        ticket.Classification.Confidence = classificationResult.Confidence;
        ticket.Classification.Reason = classificationResult.Reason;
        ticket.Classification.TagsJson = JsonSerializer.Serialize(classificationResult.Tags);
        ticket.Classification.ClassifiedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(MapToResponse(ticket));
    }

    private static TicketResponse MapToResponse(Ticket ticket)
    {
        return new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            CustomerEmail = ticket.CustomerEmail,
            ProductName = ticket.ProductName,
            Status = ticket.Status.ToString(),
            CreatedAtUtc = ticket.CreatedAtUtc,
            Classification = ticket.Classification is null
                ? null
                : new ClassificationResultDto
                {
                    Category = ticket.Classification.Category,
                    Priority = ticket.Classification.Priority,
                    RoutedTeam = ticket.Classification.RoutedTeam,
                    Confidence = ticket.Classification.Confidence,
                    Reason = ticket.Classification.Reason,
                    Tags = DeserializeTags(ticket.Classification.TagsJson)
                }
        };
    }

    private static List<string> DeserializeTags(string? tagsJson)
    {
        if (string.IsNullOrWhiteSpace(tagsJson))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(tagsJson) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
