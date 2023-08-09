using FluentAssertions;
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
                IssueType = new JiraIssueType { Name = "Backlog", },
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
                CreateJiraTask(null));

        // Assert
        content.Should().BeEmpty();
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
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void Heading()
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
                    Type = "heading",
                    Attributes = new JiraDescriptionAttributes { Level = 3 },
                    Content = new[]
                    {
                        new JiraDescriptionContent { Type = "text", Text = "The task description" }
                    }
                }
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void SimpleBulletList()
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
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void ComplexBulletList()
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
                                            Type = "text", Text = "Option 1"
                                        }
                                    }
                                }
                            }
                        },
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
                                            Type = "text", Text = "Option 2"
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
            CreateJiraTask(description));

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
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void SimpleCodeBlock()
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
                    Type = "codeBlock",
                    Content = new[] { new JiraDescriptionContent { Type = "text", Text = "some code" } }
                },
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void CodeBlockWithLanguage()
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
                    Type = "codeBlock",
                    Attributes = new JiraDescriptionAttributes { Language = "text" },
                    Content = new[] { new JiraDescriptionContent { Type = "text", Text = "some code" } }
                },
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void Mention()
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
                    Type = "mention", Attributes = new JiraDescriptionAttributes { Text = "@John Doe" }
                },
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }

    [Fact]
    public void Emojii()
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
                    Type = "emoji", Attributes = new JiraDescriptionAttributes { Text = "😀" }
                },
            }
        };

        // Act
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(
            CreateJiraTask(description));

        // Assert
        content.MatchSnapshot();
    }
}
