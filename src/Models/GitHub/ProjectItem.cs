namespace JiraToGitHubMigration.Models.GitHub;

public abstract record ProjectItem(string Id, int DatabaseId);

public record DraftIssueProjectItem(string Id, int DatabaseId) : ProjectItem(Id, DatabaseId);

public record IssueProjectItem(string Id, int DatabaseId) : ProjectItem(Id, DatabaseId);
