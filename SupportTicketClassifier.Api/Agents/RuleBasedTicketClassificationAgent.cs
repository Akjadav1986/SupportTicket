using SupportTicketClassifier.Api.Dtos;
using SupportTicketClassifier.Api.Models;

namespace SupportTicketClassifier.Api.Agents;

public sealed class RuleBasedTicketClassificationAgent : ITicketClassificationAgent
{
    private static readonly Dictionary<string, string[]> CategoryKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Billing"] = ["invoice", "payment", "refund", "billing", "subscription", "card", "charge", "paid", "amount"],
        ["Technical"] = ["api", "server", "database", "timeout", "performance", "slow", "integration", "deployment", "login error"],
        ["Security"] = ["hack", "hacked", "breach", "vulnerability", "malware", "phishing", "unauthorized", "2fa", "mfa"],
        ["Account"] = ["password", "login", "sign in", "account", "profile", "email change", "locked", "reset"],
        ["Bug"] = ["bug", "crash", "exception", "error", "not working", "failed", "broken", "issue"],
        ["FeatureRequest"] = ["feature", "enhancement", "request", "suggestion", "improve", "add option", "new functionality"]
    };

    private static readonly string[] CriticalKeywords =
    [
        "production down", "system down", "data loss", "security breach", "hacked", "cannot access all", "outage"
    ];

    private static readonly string[] HighKeywords =
    [
        "urgent", "asap", "blocked", "payment failed", "login failed", "cannot login", "major issue", "deadline"
    ];

    public Task<ClassificationResultDto> ClassifyAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        var text = $"{ticket.Title} {ticket.Description} {ticket.ProductName}".ToLowerInvariant();

        var categoryScores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var matchedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var category in CategoryKeywords)
        {
            var score = 0;

            foreach (var keyword in category.Value)
            {
                if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    score++;
                    matchedTags.Add(keyword);
                }
            }

            categoryScores[category.Key] = score;
        }

        var bestCategory = categoryScores
            .OrderByDescending(x => x.Value)
            .FirstOrDefault();

        var categoryName = bestCategory.Value > 0 ? bestCategory.Key : "General";
        var priority = DetectPriority(text);
        var routedTeam = GetRoutedTeam(categoryName);
        var confidence = CalculateConfidence(bestCategory.Value, matchedTags.Count);

        if (matchedTags.Count == 0)
        {
            matchedTags.Add("general-support");
        }

        var result = new ClassificationResultDto
        {
            Category = categoryName,
            Priority = priority,
            RoutedTeam = routedTeam,
            Confidence = confidence,
            Tags = matchedTags.OrderBy(x => x).ToList(),
            Reason = bestCategory.Value > 0
                ? $"Ticket classified as {categoryName} because it matched {bestCategory.Value} related keyword(s)."
                : "Ticket did not strongly match known categories, so it was routed to general support."
        };

        return Task.FromResult(result);
    }

    private static string DetectPriority(string text)
    {
        if (CriticalKeywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
        {
            return "Critical";
        }

        if (HighKeywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
        {
            return "High";
        }

        if (text.Length > 500)
        {
            return "Medium";
        }

        return "Low";
    }

    private static string GetRoutedTeam(string category) => category switch
    {
        "Billing" => "Billing Team",
        "Technical" => "Engineering Team",
        "Security" => "Security Team",
        "Account" => "Account Support Team",
        "Bug" => "Engineering Team",
        "FeatureRequest" => "Product Team",
        _ => "L1 Support Team"
    };

    private static decimal CalculateConfidence(int bestCategoryScore, int tagCount)
    {
        if (bestCategoryScore <= 0)
        {
            return 0.45m;
        }

        var confidence = 0.55m + (bestCategoryScore * 0.10m) + (tagCount * 0.02m);
        return Math.Min(confidence, 0.98m);
    }
}
