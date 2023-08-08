namespace JiraToGitHubMigration.Models.GitHub;

public record CloseIssuePayload(string? ClientMutationId, IssueProjectItem Item) : MutationPayload(
    ClientMutationId);

public record CloseIssueResponse(CloseIssuePayload CloseIssue);
