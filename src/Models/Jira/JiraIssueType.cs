namespace JiraToGitHubMigration.Models.Jira;

public class JiraIssueType
{
    public required string Name { get; set; }

    public bool Subtask { get; set; }
}
