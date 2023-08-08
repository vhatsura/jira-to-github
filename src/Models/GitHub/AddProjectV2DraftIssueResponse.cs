namespace JiraToGitHubMigration.Models.GitHub;

public record AddProjectV2DraftIssueResponse(AddProjectV2DraftIssuePayload AddProjectV2DraftIssue);

public record AddProjectV2DraftIssuePayload(string? ClientMutationId, ProjectItem ProjectItem) :
    MutationPayload(ClientMutationId);
