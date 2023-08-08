using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JiraToGitHubMigration.Models.Jira;

namespace JiraToGitHubMigration.Services;

public class JiraService
{
    private readonly HttpClient _client;

    public string JiraBaseAddress => _client.BaseAddress?.ToString() ?? throw new InvalidOperationException();

    public JiraService(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<PullRequestDetail>> PullRequestLinks(string issueId, string applicationType)
    {
        var sourceProviderDetails = await _client.GetFromJsonAsync<JiraIssueSourceProviderDetails>(
                                        $"/rest/dev-status/latest/issue/detail?issueId={issueId}&applicationType={applicationType}&dataType=branch") ??
                                    throw new InvalidOperationException();

        if (sourceProviderDetails.Errors is { Length: > 0 } || sourceProviderDetails.Detail.Length == 0)
        {
            Debugger.Break();

            throw new InvalidOperationException();
        }

        return sourceProviderDetails.Detail.SelectMany(x => x.PullRequests).ToList();
    }

    public async Task AddLabel(string taskKey, string label)
    {
        var updateLabelResponse = await _client.PutAsJsonAsync(
            $"/rest/api/3/issue/{taskKey}", new { update = new { labels = new[] { new { add = label } } } });

        if (!updateLabelResponse.IsSuccessStatusCode)
        {
            var error = await updateLabelResponse.Content.ReadAsStringAsync();
            Debugger.Break();

            throw new InvalidOperationException(error);
        }
    }

    public async Task AddComment(string taskKey, JiraDescription content)
    {
        var leaveCommentResponse = await _client.PostAsJsonAsync(
            $"/rest/api/3/issue/{taskKey}/comment",
            new { body = content },
            new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        if (!leaveCommentResponse.IsSuccessStatusCode)
        {
            var error = await leaveCommentResponse.Content.ReadAsStringAsync();
            Debugger.Break();

            throw new InvalidOperationException(error);
        }
    }

    public async IAsyncEnumerable<JiraTask[]> GetTasks(string jql)
    {
        var startAt = 0;
        var maxResults = 50;

        bool isLastPage;

        do
        {
            var response = await _client.GetFromJsonAsync<JiraSearchResponse>(
                $"/rest/api/3/search?jql={jql}&startAt={startAt}&maxResults={maxResults}");

            if (response == null)
            {
                Debugger.Break();

                throw new InvalidOperationException();
            }

            yield return response.Issues;

            isLastPage = response.StartAt + response.MaxResults >= response.Total;
        } while (!isLastPage);
    }
}
