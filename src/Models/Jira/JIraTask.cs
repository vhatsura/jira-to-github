using System.Diagnostics;

namespace JiraToGitHubMigration.Models.Jira;

[DebuggerDisplay("{Key} - {Fields.Summary}")]
public class JiraTask
{
    public required string Id { get; set; }

    public required string Key { get; set; }

    public required JiraFields Fields { get; set; }
}
