namespace JiraToGitHubMigration.Models.GitHub;

public record AddProjectV2ItemByIdPayload(string? ClientMutationId, IssueProjectItem Item) : MutationPayload(
    ClientMutationId);

public record AddProjectV2ItemByIdResponse(AddProjectV2ItemByIdPayload AddProjectV2ItemById);
