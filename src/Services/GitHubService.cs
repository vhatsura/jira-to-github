using System.Net.Http.Json;
using JiraToGitHubMigration.Models;
using JiraToGitHubMigration.Models.GitHub;

namespace JiraToGitHubMigration.Services;

public class GitHubService
{
    private readonly HttpClient _client;

    private const string UpdateProjectV2ItemFieldValueMutation = @"
mutation($input: UpdateProjectV2ItemFieldValueInput!) {
  updateProjectV2ItemFieldValue(input: $input){
    clientMutationId
  }
}";

    public GitHubService(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> RepositoryId(string repositoryName, string owner)
    {
        var response = await _client.PostAsJsonAsync(
            "/graphql", new
            {
                query = @"
query($name:String!, $owner:String!) {
  repository(name: $name, owner: $owner) {
    id
  }
}",
                variables = new { name = repositoryName, owner }
            });

        var result = await response.ParseGraphQlResponse<RepositoryResponse>();

        return result.Repository.Id ?? throw new InvalidOperationException();
    }

    public async Task<Issue> CreateIssue(
        string body, string repositoryId, string title, string[]? assigneeIds)
    {
        var response = await _client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = @"
mutation ($input: CreateIssueInput!) {
  createIssue(input: $input) {
    clientMutationId
    issue {
      id
    }
  }
}",
                variables = new { input = new { repositoryId, title, body, assigneeIds } }
            });

        var result = await response.ParseGraphQlResponse<CreateIssueResponse>();

        return result.CreateIssue.Issue;
    }

    public async Task AssignIssue(string issueId, string assigneeId)
    {
        var assignIssueResponse = await _client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = @"
mutation ($input: AddAssigneesToAssignableInput!) {
  addAssigneesToAssignable(input: $input) {
    clientMutationId
  }
}",
                variables = new { input = new { assignableId = issueId, assigneeIds = new[] { assigneeId } } }
            });

        _ = await assignIssueResponse.ParseGraphQlResponse<AddAssigneesToAssignableResponse>();
    }

    public async Task SetIssueEpic(string projectId, string itemId, string value)
    {
        var setEpicResponse = await _client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = UpdateProjectV2ItemFieldValueMutation,
                variables = new
                {
                    input = new
                    {
                        fieldId = Constants.GitHubEpicFieldId,
                        itemId,
                        projectId,
                        value = new { text = value }
                    }
                }
            });

        _ = await setEpicResponse.ParseGraphQlResponse<UpdateProjectV2ItemFieldValueResponse>();
    }

    public async Task SetIssueStatus(string projectId, string itemId, string value)
    {
        var setStatusResponse = await _client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = UpdateProjectV2ItemFieldValueMutation,
                variables = new
                {
                    input = new
                    {
                        fieldId = Constants.GitHubStatusFieldId,
                        itemId,
                        projectId,
                        value = new { singleSelectOptionId = value }
                    }
                }
            });

        _ = await setStatusResponse.ParseGraphQlResponse<UpdateProjectV2ItemFieldValueResponse>();
    }

    public async Task<IssueProjectItem> AddIssueToProject(string projectId, string issueId)
    {
        var response = await _client.PostAsJsonAsync(
            "/graphql", new
            {
                query = @"
mutation ($input: AddProjectV2ItemByIdInput!) {
  addProjectV2ItemById(input: $input) {
    item {
      id
      databaseId
    }
  }
}",
                variables = new { input = new { projectId, contentId = issueId } }
            });

        var data = await response.ParseGraphQlResponse<AddProjectV2ItemByIdResponse>();

        return new IssueProjectItem(
            data.AddProjectV2ItemById.Item.Id, data.AddProjectV2ItemById.Item.DatabaseId, issueId);
    }

    public async Task CloseIssue(string issueId)
    {
        var response = await _client.PostAsJsonAsync(
            "/graphql", new
            {
                query = @"
mutation ($input: CloseIssueInput!) {
  closeIssue(input: $input) {
    clientMutationId
  }
}",
                variables = new { input = new { issueId, stateReason = "COMPLETED" } }
            });

        _ = await response.ParseGraphQlResponse<CloseIssueResponse>();
    }

    public async Task<ProjectItem> AddDraftIssue(
        string projectId, string title, string body, string[]? assigneeIds)
    {
        object variables = assigneeIds != null
            ? new { input = new { title, projectId, body, assigneeIds } }
            : new { input = new { title, projectId, body } };

        var addDraftIssueHttpResponse = await _client.PostAsJsonAsync(
            "/graphql", new
            {
                query = @"
mutation ($input: AddProjectV2DraftIssueInput!) {
  addProjectV2DraftIssue(input: $input) {
    projectItem {
      id
      databaseId
    }
  }
}",
                variables
            });

        var addDraftIssueResponse =
            await addDraftIssueHttpResponse.ParseGraphQlResponse<AddProjectV2DraftIssueResponse>();

        return addDraftIssueResponse.AddProjectV2DraftIssue.ProjectItem;
    }
}
