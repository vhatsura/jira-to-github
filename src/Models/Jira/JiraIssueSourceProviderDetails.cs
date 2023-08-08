using System.Text.Json.Serialization;

namespace JiraToGitHubMigration.Models.Jira;

public class JiraSourceProviderInstance
{
    public required string Name { get; set; }

    public required string BaseUrl { get; set; }

    public required string Type { get; set; }

    public required string Id { get; set; }

    public required string TypeName { get; set; }

    public bool SingleInstance { get; set; }
}

public class PullRequestDetail
{
    public required string Id { get; set; }

    public required string Status { get; set; }

    public required string Url { get; set; }
}

public class JiraIssueSourceProviderDetailsItem
{
    public required PullRequestDetail[] PullRequests { get; set; }

    public string[]? Repositories { get; set; }

    [JsonPropertyName("_instance")]
    public required JiraSourceProviderInstance Instance { get; set; }
}

public class JiraIssueSourceProviderDetails
{
    public string[]? Errors { get; set; }

    public required JiraIssueSourceProviderDetailsItem[] Detail { get; set; }
}
