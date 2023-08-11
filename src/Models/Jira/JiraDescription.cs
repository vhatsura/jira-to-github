using System.Diagnostics;
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

    public int? Level { get; set; }

    public string? Language { get; set; }

    public string? Id { get; set; }

    public string? Text { get; set; }

    public string? UserType { get; set; }

    public string? AccessLevel { get; set; }

    public string? ShortName { get; set; }
}

public class JiraDescriptionMark
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("attrs")]
    public JiraDescriptionAttributes? Attributes { get; set; }
}

[DebuggerDisplay("{Type}")]
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

    public static JiraDescriptionContent TextContent(string text)
    {
        return new JiraDescriptionContent { Type = "text", Text = text };
    }

    public static JiraDescriptionContent Paragraph(string text)
    {
        return new JiraDescriptionContent { Type = "paragraph", Content = new[] { TextContent(text) } };
    }

    public static JiraDescriptionContent Paragraph(params JiraDescriptionContent[] content)
    {
        return new JiraDescriptionContent { Type = "paragraph", Content = content };
    }

    public static JiraDescriptionContent Heading(string text, int level)
    {
        return new JiraDescriptionContent
        {
            Type = "heading",
            Attributes = new JiraDescriptionAttributes { Level = level },
            Content = new[] { new JiraDescriptionContent { Type = "text", Text = text } }
        };
    }

    public static JiraDescriptionContent ListItem(params JiraDescriptionContent[] content)
    {
        return new JiraDescriptionContent { Type = "listItem", Content = content };
    }

    public static JiraDescriptionContent BulletList(params JiraDescriptionContent[] content)
    {
        return new JiraDescriptionContent { Type = "bulletList", Content = content };
    }
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
