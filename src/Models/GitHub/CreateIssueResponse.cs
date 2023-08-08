namespace JiraToGitHubMigration.Models.GitHub;

public record Issue(string Id);

public record CreateIssue(string? ClientMutationId, Issue Issue) : MutationPayload(ClientMutationId);

public record CreateIssueResponse(CreateIssue CreateIssue);
