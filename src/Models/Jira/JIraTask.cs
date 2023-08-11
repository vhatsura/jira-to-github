namespace JiraToGitHubMigration.Models.Jira;

public class JiraTask
{
    public required string Id { get; set; }

    public required string Key { get; set; }

    public required JiraFields Fields { get; set; }

    public override string ToString()
    {
        return $"[{Key}] - \"{Fields.Summary}\" - {Fields.Status}";
    }
}
