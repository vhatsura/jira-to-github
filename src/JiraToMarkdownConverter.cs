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
                case { Type: "mediaSingle" }:
                    throw new NotImplementedException();
                default:
                    Debugger.Break();

                    throw new NotImplementedException();
            }
        }
    }

    public static string ConvertJiraDescriptionToGitHubBody(JiraTask task, string jiraBaseAddress)
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
            if (stringBuilder[^2] != '\n')
            {
                stringBuilder.AppendLine();
            }

            if (stringBuilder[^2] != '\n')
            {
                stringBuilder.AppendLine();
            }
        }


        stringBuilder.AppendLine(
            $"> Originally reported issue in Jira [{task.Key}]({jiraBaseAddress}/browse/{task.Key})");

        return stringBuilder.ToString();
    }
}
