namespace SachkovTech.Core.Dtos;

public class UserIssueDto
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public Guid IssueId { get; init; }
    public Guid ModuleId { get; init; }

    public string IssueTitle { get; init; } = string.Empty;
    public string IssueDescription { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;

    public DateTime StartDateOfExecution { get; init; }

    public DateTime EndDateOfExecution { get; init; }

    public int Attempts { get; init; }

    public string PullRequestUrl { get; init; } = string.Empty;
}