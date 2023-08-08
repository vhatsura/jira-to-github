namespace JiraToGitHubMigration.Models.GitHub;

public record ProjectItem(string Id, int DatabaseId);

public record IssueProjectItem(string Id, int DatabaseId, string IssueId) : ProjectItem(Id, DatabaseId);
