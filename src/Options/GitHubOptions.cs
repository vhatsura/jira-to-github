namespace JiraToGitHubMigration.Options;

public class GitHubOptions
{
    public required string BaseAddress { get; init; }

    public required string Token { get; init; }
}
