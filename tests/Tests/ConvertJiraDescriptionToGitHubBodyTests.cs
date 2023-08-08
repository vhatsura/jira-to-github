using JiraToGitHubMigration;
using JiraToGitHubMigration.Models.Jira;
using Snapshooter.Xunit;

namespace Tests;

public class ConvertJiraDescriptionToGitHubBodyTests
{
    private static JiraTask CreateJiraTask(JiraDescription? description)
    {
        return new JiraTask
        {
            Id = "123",
            Key = "OA-123",
            Fields = new JiraFields
            {
                Summary = "The task title",
                Status = new JiraStatus { Name = "Backlog" },
                IssueType = new JiraIssueType { Name = "Task" },
                Description = description
            }
        };
    }

    [Fact]
    public void EmptyText()
    {
        // Arrange + Act
        var content =
            JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
                CreateJiraTask(null), "https://example.atlassian.net");

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void SimpleText()
    {
        // Arrange
        var description = new JiraDescription
        {
            Version = 1,
            Type = "doc",
            Content = new[]
            {
                new JiraDescriptionContent
                {
                    Type = "paragraph",
                    Content = new[]
                    {
                        new JiraDescriptionContent { Type = "text", Text = "The task description" }
                    }
                }
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description), "https://example.atlassian.net");

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void BulletList()
    {
        // Arrange
        var description = new JiraDescription
        {
            Version = 1,
            Type = "doc",
            Content = new[]
            {
                new JiraDescriptionContent
                {
                    Type = "bulletList",
                    Content = new[]
                    {
                        new JiraDescriptionContent
                        {
                            Type = "listItem",
                            Content = new[]
                            {
                                new JiraDescriptionContent
                                {
                                    Type = "paragraph",
                                    Content = new[]
                                    {
                                        new JiraDescriptionContent
                                        {
                                            Type = "text",
                                            Text = "The task description"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description), "https://example.atlassian.net");

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void InlineCard()
    {
        // Arrange
        var description = new JiraDescription
        {
            Version = 1,
            Type = "doc",
            Content = new[]
            {
                new JiraDescriptionContent { Type = "text", Text = "Affected:" },
                new JiraDescriptionContent { Type = "hardBreak" },
                new JiraDescriptionContent { Type = "text", Text = "Relates to changes: " },
                new JiraDescriptionContent
                {
                    Type = "inlineCard",
                    Attributes = new JiraDescriptionAttributes { Url = "https://example.com" }
                },
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description), "https://example.atlassian.net");

        // Assert
        content.MatchSnapshot();
    }
}
