namespace JiraToGitHubMigration.Models.Jira;

public class JiraStatus
{
    public required string Name { get; set; }

    public override string ToString() => Name;
}
