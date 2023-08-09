using System.Diagnostics;
using System.Text.RegularExpressions;
using JiraToGitHubMigration.Models.GitHub;
using JiraToGitHubMigration.Models.Jira;
using Microsoft.Extensions.Logging;

namespace JiraToGitHubMigration.Services;

public class MigrationService
{
    private readonly JiraService _jiraService;
    private readonly GitHubService _githubService;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(JiraService jiraService, GitHubService githubService, ILogger<MigrationService> logger)
    {
        _jiraService = jiraService;
        _githubService = githubService;
        _logger = logger;
    }

    public async Task Run()
    {
        await foreach (var jiraTasks in _jiraService.GetTasks(Constants.JiraGql))
        {
            foreach (var jiraTask in jiraTasks)
            {
                try
                {
                    var issueUrl = await CreateGitHubIssue(jiraTask);

                    if (issueUrl != null)
                    {
                        await UpdateJiraTask(jiraTask, issueUrl);
                    }
                }
                catch (NotImplementedException)
                {
                    // ignore
                }
            }
        }
    }

    private static string[]? AssigneeIds(JiraTask task)
    {
        if (task.Fields.Assignee?.EmailAddress != null)
        {
            if (!Constants.JiraAssigneeToGitHubAssigneeMap.TryGetValue(
                    task.Fields.Assignee.EmailAddress, out var assigneeId))
            {
                Debugger.Break();

                throw new InvalidOperationException();
            }


            return assigneeId != null ? new[] { assigneeId } : null;
        }

        return null;
    }

    private bool HasMergedReferencedPullRequests(JiraTask task)
    {
        if (task.Fields.SourceProviderData == null || task.Fields.SourceProviderData == "{}")
            return false;

        if (task.Fields.SourceProviderData.StartsWith("{build="))
        {
            return false;
        }

        if (task.Fields.SourceProviderData.StartsWith("{branch="))
        {
            return false;
        }

        if (task.Fields.SourceProviderData.Contains("dataType=pullrequest, state=MERGED"))
        {
            return true;
        }

        if (task.Fields.SourceProviderData.Contains("dataType=pullrequest, state=OPEN"))
        {
            return true;
        }

        if (task.Fields.SourceProviderData.Contains("dataType=pullrequest, state=DECLINED"))
        {
            return false;
        }

        Debugger.Break();

        throw new NotImplementedException();
    }

    private async Task<IssueProjectItem> CreateIssueAndAddToProject(
        JiraTask task, string title, string body, string[]? assigneeIds)
    {
        var pullRequestLinks = await _jiraService.PullRequestLinks(task.Id, "GitHub");

        HashSet<(string Owner, string Repository, string PRNumber)> set = new();

        foreach (var pullRequest in pullRequestLinks.Where(p => p.Status == "MERGED"))
        {
            var pattern = @"https://github\.com/(?<owner>.+?)/(?<repository>.+?)/pull/(?<prNumber>\d+)";

            var regex = new Regex(pattern);
            Match match = regex.Match(pullRequest.Url);

            set.Add(
                (match.Groups["owner"].Value, match.Groups["repository"].Value,
                    match.Groups["prNumber"].Value));
        }

        if (set.Count != 1)
        {
            _logger.LogError($"There are a few pull requests for '{task.Key}' task");

            throw new NotImplementedException();
        }

        var repositoryId = await _githubService.RepositoryId(set.First().Repository, set.First().Owner);

        var createdIssue = await _githubService.CreateIssue(body, repositoryId, title, assigneeIds);

        return await _githubService.AddIssueToProject(Constants.GitHubProjectId, createdIssue.Id);
        // link to PR - not available in API
    }

    private string? ExtractIterationId(JiraTask task)
    {
        if (task.Fields.Sprint is not null)
        {
            if (task.Fields.Sprint.Length != 1)
            {
                _logger.LogError($"Multiple Sprints are not supported for now. Task '{task.Key}'");

                // Debugger.Break();

                throw new NotImplementedException();
            }

            if (!Constants.JiraSprintToGitHubIterationMap.TryGetValue(task.Fields.Sprint[0].Name, out var iterationId))
            {
                Debugger.Break();

                throw new InvalidOperationException();
            }

            return iterationId;
        }

        return null;
    }

    private string Body(JiraTask task)
    {
        var content = JiraToMarkdownConverter.ConvertJiraDescriptionToGitHubBody(task);

        return content + "\n\n" +
               $"> Originally reported issue in Jira [{task.Key}]({_jiraService.JiraBaseAddress}/browse/{task.Key})";
    }

    private async Task<string?> CreateGitHubIssue(JiraTask task)
    {
        if (task.Fields.IssueType.Name != "Task" && task.Fields.IssueType.Name != "Bug")
        {
            _logger.LogError($"'{task.Fields.IssueType.Name}' issues are not supported. Task '{task.Key}'");

            return null;
        }

        if (task.Fields.Parent != null && task.Fields.Parent.Fields.IssueType.Name != "Epic")
        {
            _logger.LogError($"Only Epic issues as parent supported for now. Task '{task.Key}'");

            return null;
        }

        if (task.Fields.IssueLinks is { Length: > 0 })
        {
            _logger.LogError($"Issue links are not supported for now. Task '{task.Key}'");

            return null;
        }

        if (task.Fields.Subtasks is { Length: > 0 })
        {
            _logger.LogError($"Subtasks are not supported for now. Task '{task.Key}'");

            return null;
        }

        if (!Constants.JiraToGitHubStatusMap.TryGetValue(task.Fields.Status.Name, out var statusFieldValueId))
        {
            Debugger.Break();

            throw new InvalidOperationException();
        }

        var title = task.Fields.Summary;
        var body = Body(task);
        var assigneeIds = AssigneeIds(task);
        var iterationId = ExtractIterationId(task);
        var storyPoints = task.Fields.Estimations;

        ProjectItem projectItem = HasMergedReferencedPullRequests(task)
            ? await CreateIssueAndAddToProject(task, title, body, assigneeIds)
            : await _githubService.AddDraftIssue(Constants.GitHubProjectId, title, body, assigneeIds);

        await _githubService.SetIssueStatus(Constants.GitHubProjectId, projectItem.Id, statusFieldValueId);

        if (task.Fields.Status.Name == "Done" && projectItem is IssueProjectItem issueProjectItem)
        {
            await _githubService.CloseIssue(issueProjectItem.IssueId);
        }

        if (iterationId != null)
        {
            await _githubService.SetIteration(Constants.GitHubProjectId, projectItem.Id, iterationId);
        }

        if (storyPoints != null)
        {
            await _githubService.SetStoryPoints(Constants.GitHubProjectId, projectItem.Id, storyPoints.Value);
        }

        if (task.Fields.Parent != null)
        {
            if (task.Fields.Parent.Fields.IssueType.Name == "Epic")
            {
                await _githubService.SetIssueEpic(
                    Constants.GitHubProjectId, projectItem.Id, task.Fields.Parent.Fields.Summary);
            }
            else
            {
                Debugger.Break();

                throw new NotImplementedException();
            }
        }

        return
            $"https://github.com/orgs/{Constants.GitHubOrganizationOwner}/projects/{Constants.GitHubProjectIndex}/views/{Constants.GitHubProjectViewIndex}?pane=issue&itemId={projectItem.DatabaseId}";
    }

    private async Task UpdateJiraTask(JiraTask task, string githubIssueUrl)
    {
        await _jiraService.AddLabel(task.Key, Constants.JiraMigratedLabel);

        await _jiraService.AddComment(
            task.Key,
            new JiraDescription
            {
                Type = "doc",
                Version = 1,
                Content = new[]
                {
                    new JiraDescriptionContent
                    {
                        Type = "paragraph",
                        Content = new[]
                        {
                            new JiraDescriptionContent
                            {
                                Type = "text", Text = "Migrated to GitHub issue "
                            },
                            new JiraDescriptionContent
                            {
                                Type = "text",
                                Text = $"`{githubIssueUrl}`",
                                Marks = new[]
                                {
                                    new JiraDescriptionMark
                                    {
                                        Type = "link",
                                        Attributes = new JiraDescriptionAttributes
                                        {
                                            Href = githubIssueUrl, Title = "GitHub"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
    }
}
