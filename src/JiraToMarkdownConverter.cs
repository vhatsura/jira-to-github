using System.Diagnostics;
using System.Text;
using JiraToGitHubMigration.Models.Jira;

namespace JiraToGitHubMigration;

public static class JiraToMarkdownConverter
{
    private static void BuildMarkdown(StringBuilder stringBuilder, JiraDescriptionAttributes attributes)
    {
        if (attributes.Title == null && attributes.Href == null && attributes.Url != null)
        {
            stringBuilder.Append(attributes.Url);
        }
        else
        {
            Debugger.Break();

            throw new NotImplementedException();
        }
    }

    private static void BuildMarkdown(StringBuilder stringBuilder, JiraDescriptionContent[] items)
    {
        foreach (var content in items)
        {
            switch (content)
            {
                case { Type: "paragraph", Content: not null }:
                    BuildMarkdown(stringBuilder, content.Content);
                    stringBuilder.AppendLine();

                    break;
                case { Type: "orderedList" }:
                    throw new NotImplementedException();
                case { Type: "bulletList", Content: not null }:
                    BuildMarkdown(stringBuilder, content.Content);
                    stringBuilder.AppendLine();

                    break;
                case { Type: "listItem", Content: not null }:
                    stringBuilder.Append("* ");
                    BuildMarkdown(stringBuilder, content.Content);

                    break;
                case { Type: "text", Text: not null }:
                    stringBuilder.Append(content.Text);

                    break;
                case { Type: "hardBreak" }:
                    stringBuilder.AppendLine();

                    break;
                case { Type: "inlineCard", Attributes: not null }:
                    BuildMarkdown(stringBuilder, content.Attributes);

                    break;
                case { Type: "rule" }:
                    stringBuilder.AppendLine("---");

                    break;
                case { Type: "heading", Attributes.Level: not null, Content: not null }:
                    stringBuilder.Append('#', content.Attributes.Level.Value);
                    stringBuilder.Append(' ');

                    BuildMarkdown(stringBuilder, content.Content);
                    stringBuilder.AppendLine();

                    break;
                case { Type: "mediaGroup" }:
                case { Type: "mediaSingle" }:
                    throw new NotImplementedException();
                case { Type: "codeBlock", Content: not null }:

                    stringBuilder.AppendLine($"```{content.Attributes?.Language ?? string.Empty}");
                    BuildMarkdown(stringBuilder, content.Content);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("```");

                    break;

                case { Type: "table" }:
                    throw new NotImplementedException();
                case { Type: "emoji", Attributes.Text: not null }:
                case { Type: "mention", Attributes.Text: not null }:
                    stringBuilder.Append(content.Attributes.Text);

                    break;
                case { Type: "emoji" }:
                    throw new NotImplementedException();
                case { Type: "mention" }:
                    throw new NotImplementedException();
                case { Type: "panel" }:
                    throw new NotImplementedException();
                case { Type: "blockquote" }:
                    throw new NotImplementedException();
                case { Type: "blockCard" }:
                    throw new NotImplementedException();
                default:
                    Debugger.Break();

                    throw new NotImplementedException();
            }
        }
    }

    public static string ConvertJiraDescriptionToGitHubBody(JiraTask task)
    {
        var stringBuilder = new StringBuilder();

        if (task.Fields.Description != null)
        {
            if (task.Fields.Description.Type != "doc")
            {
                Debugger.Break();

                throw new NotImplementedException();
            }

            BuildMarkdown(stringBuilder, task.Fields.Description.Content);
        }

        if (stringBuilder.Length > 2)
        {
            if (stringBuilder[^2] == '\n' && stringBuilder[^1] == '\n')
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
        }

        return stringBuilder.ToString();
    }
}
