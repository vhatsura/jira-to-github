namespace JiraToGitHubMigration.Models.Jira;

class JiraSearchResponse
{
    public int StartAt { get; set; }

    public int MaxResults { get; set; }

    public int Total { get; set; }

    public required JiraTask[] Issues { get; set; }
}
