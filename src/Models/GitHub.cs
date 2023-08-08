namespace JiraToGitHubMigration.Models;

public record Repository(string Id);

public record RepositoryResponse(Repository Repository);

public record AddAssigneesToAssignableResponse(MutationPayload AddAssigneesToAssignable);

public record UpdateProjectV2ItemFieldValueResponse(MutationPayload UpdateProjectV2ItemFieldValue);

public record MutationPayload(string? ClientMutationId);

public record GitHubGraphQlError(string Message);

public class GitHubGraphQlResponse<TData> where TData : class
{
    public GitHubGraphQlError[]? Errors { get; set; }

    public TData? Data { get; set; }
}
