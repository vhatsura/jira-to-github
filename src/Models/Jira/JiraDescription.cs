using System.Text.Json.Serialization;

namespace JiraToGitHubMigration.Models.Jira;

public class JiraDescriptionAttributes
{
    [JsonPropertyName("href")]
    public string? Href { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class JiraDescriptionMark
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("attrs")]
    public JiraDescriptionAttributes? Attributes { get; set; }
}

public class JiraDescriptionContent
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("content")]
    public JiraDescriptionContent[]? Content { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("marks")]
    public JiraDescriptionMark[]? Marks { get; set; }

    [JsonPropertyName("attrs")]
    public JiraDescriptionAttributes? Attributes { get; set; }
}

public class JiraDescription
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("content")]
    public required JiraDescriptionContent[] Content { get; set; }
}
