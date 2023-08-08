namespace JiraToGitHubMigration.Options;

public class JiraOptions
{
    public required string BaseAddress { get; init; }

    public required string Email { get; init; }

    public required string Token { get; init; }
}
