using System.Text.Json.Serialization;

namespace JiraToGitHubMigration.Models.Jira;

public class JiraFields
{
    public JiraTask? Parent { get; set; }

    public required JiraStatus Status { get; set; }

    public required JiraIssueType IssueType { get; set; }

    public required string Summary { get; set; }

    public JiraDescription? Description { get; set; }

    public JiraAssignee? Assignee { get; set; }

    [JsonPropertyName("customfield_10000")]
    public string? SourceProviderData { get; set; }

    [JsonPropertyName("customfield_10020")]
    public JiraSprintDetails[]? Sprint { get; set; }

    public JiraIssueLink[]? IssueLinks { get; set; }

    public JiraTask[]? Subtasks { get; set; }

    [JsonPropertyName("customfield_10026")]
    public float? Estimations { get; set; }
}
