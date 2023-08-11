using System.Diagnostics;
using System.Text;
using JiraToGitHubMigration.Models.Jira;

namespace JiraToGitHubMigration;

public static class JiraToMarkdownConverter
{
    private record Context(int ListLevel, bool WrapWithEmptyLine);

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

    private static Context BuildMarkdown(StringBuilder stringBuilder, JiraDescriptionContent content, Context context)
    {
        switch (content)
        {
            case { Type: "paragraph", Content: not null }:
                BuildMarkdown(stringBuilder, content.Content, context);
                stringBuilder.AppendLine();

                return context with { WrapWithEmptyLine = true };
            case { Type: "orderedList" }:
                throw new NotImplementedException();
            case { Type: "bulletList", Content: not null }:
                if (context is { WrapWithEmptyLine: true, ListLevel: 0 })
                {
                    stringBuilder.AppendLine();
                }

                BuildMarkdown(stringBuilder, content.Content, context with { ListLevel = context.ListLevel + 1 });

                return context with { WrapWithEmptyLine = true };
            case { Type: "listItem", Content: not null }:
                stringBuilder.Append("* ".PadLeft(context.ListLevel * 2, ' '));
                BuildMarkdown(stringBuilder, content.Content, context);

                return context;
            case { Type: "text", Text: not null }:
                stringBuilder.Append(content.Text);

                return context;
            case { Type: "hardBreak" }:
                stringBuilder.AppendLine();

                return context;
            case { Type: "inlineCard", Attributes: not null }:
                BuildMarkdown(stringBuilder, content.Attributes);

                return context;
            case { Type: "rule" }:
                stringBuilder.AppendLine("---");

                return context;
            case { Type: "heading", Attributes.Level: not null, Content: not null }:

                if (context is { WrapWithEmptyLine: true, ListLevel: 0 })
                {
                    stringBuilder.AppendLine();
                }

                stringBuilder.Append('#', content.Attributes.Level.Value);
                stringBuilder.Append(' ');

                BuildMarkdown(stringBuilder, content.Content, context);
                stringBuilder.AppendLine();

                return context with { WrapWithEmptyLine = true };
            case { Type: "mediaGroup" }:
            case { Type: "mediaSingle" }:

                stringBuilder.AppendLine(
                    "> Here should be an attachment, but Jira API [doesn't provide access to their Media API](https://community.developer.atlassian.com/t/fetching-media-from-atlassian-document-format/31587) to download it.");

                stringBuilder.AppendLine("> Please, see original Jira issue for details.");
                stringBuilder.AppendLine();

                return context;

            case { Type: "codeBlock", Content: not null }:

                stringBuilder.AppendLine($"```{content.Attributes?.Language ?? string.Empty}");
                BuildMarkdown(stringBuilder, content.Content, context);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("```");

                return context;

            case { Type: "table" }:
                throw new NotImplementedException();
            case { Type: "emoji", Attributes.Text: not null }:
            case { Type: "mention", Attributes.Text: not null }:
                stringBuilder.Append(content.Attributes.Text);

                return context;
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

    private static Context BuildMarkdown(StringBuilder stringBuilder, JiraDescriptionContent[] items, Context context)
    {
        var contextToUse = context;

        foreach (var content in items)
        {
            contextToUse = BuildMarkdown(stringBuilder, content, contextToUse);
        }

        return contextToUse;
    }

    private static void BuildMarkdown(StringBuilder stringBuilder, JiraDescription description)
    {
        if (description.Type != "doc")
        {
            Debugger.Break();

            throw new NotImplementedException();
        }

        var context = BuildMarkdown(stringBuilder, description.Content, new Context(0, false));
    }

    public static string ConvertJiraDescriptionToGitHubBody(JiraTask task)
    {
        var stringBuilder = new StringBuilder();

        if (task.Fields.Description != null)
        {
            BuildMarkdown(stringBuilder, task.Fields.Description);
        }

        return stringBuilder.ToString();
    }
}
