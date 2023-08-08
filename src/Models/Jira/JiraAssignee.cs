namespace JiraToGitHubMigration.Models.Jira;

public class JiraAssignee
{
    public string? EmailAddress { get; set; }

    public required string DisplayName { get; set; }
}
