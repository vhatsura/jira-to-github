using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using JiraToGitHubMigration.Models;

namespace JiraToGitHubMigration;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public static async Task<TData> ParseGraphQlResponse<TData>(this HttpResponseMessage response)
        where TData : class
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Debugger.Break();

            throw new InvalidOperationException(error);
        }

        var payload = await response.Content.ReadFromJsonAsync<GitHubGraphQlResponse<TData>>(
            _options);

        if (payload?.Errors != null || payload?.Data == null)
        {
            Debugger.Break();

            throw new InvalidOperationException();
        }

        return payload.Data;
    }
}
