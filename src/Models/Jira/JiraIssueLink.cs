namespace JiraToGitHubMigration.Models.Jira;

public class JiraIssueLinkType
{
    public required string Inward { get; set; }

    public required string Outward { get; set; }
}

public class JiraIssueLink
{
    public required string Id { get; set; }

    public required JiraIssueLinkType Type { get; set; }

    public JiraTask? InwardIssue { get; set; }

    public JiraTask? OutwardIssue { get; set; }

    public override string ToString()
    {
        if (InwardIssue != null) return $"{Type.Inward} {InwardIssue}";
        if (OutwardIssue != null) return $"{Type.Outward} {OutwardIssue}";

        return $"{Type.Inward} / {Type.Outward}";
    }
}
