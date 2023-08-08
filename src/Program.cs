using System.Net.Http.Headers;
using System.Text;
using JiraToGitHubMigration.Options;
using JiraToGitHubMigration.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<JiraService>();
builder.Services.AddSingleton<GitHubService>();
builder.Services.AddSingleton<MigrationService>();

builder.Services.AddHttpClient<GitHubService>(
    client =>
    {
        var gitHubOptions = builder.Configuration.GetSection("GitHub").Get<GitHubOptions>();

        if (gitHubOptions == null) throw new InvalidOperationException();

        client.BaseAddress = new Uri(gitHubOptions.BaseAddress);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", gitHubOptions.Token);

        client.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue(new ProductHeaderValue("JiraToGitHubMigration")));

        client.DefaultRequestHeaders.Add("X-Github-Next-Global-ID", "1");
    });

builder.Services.AddHttpClient<JiraService>(
    client =>
    {
        var jiraOptions = builder.Configuration.GetSection("Jira").Get<JiraOptions>();

        if (jiraOptions == null) throw new InvalidOperationException();

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{jiraOptions.Email}:{jiraOptions.Token}"));

        client.BaseAddress = new Uri(jiraOptions.BaseAddress);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    });

var host = builder.Build();
var service = host.Services.GetRequiredService<MigrationService>();
await service.Run();
