using System.Net;
using System.Net.Http.Json;
using FitnessChallenge.Api.Application.Dtos;

namespace FitnessChallenge.Tests;

public class ApiIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Guid> RegisterAsync(string first, string last)
    {
        var response = await _client.PostAsJsonAsync("/api/users", new { firstName = first, lastName = last });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task Register_ReturnsCreatedWithId()
    {
        var response = await _client.PostAsJsonAsync("/api/users", new { firstName = "Ivo", lastName = "Ivic" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        Assert.NotEqual(Guid.Empty, body!.Id);
    }

    [Fact]
    public async Task Register_DuplicateNameCaseInsensitive_Returns409()
    {
        await RegisterAsync("Duplicate", "Person");
        var response = await _client.PostAsJsonAsync("/api/users", new { firstName = "DUPLICATE", lastName = "person" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_MissingName_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users", new { firstName = "", lastName = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Ingest_ValidRunning_ReturnsCreatedWithFlooredPoints()
    {
        var userId = await RegisterAsync("Runner", "One");

        var response = await _client.PostAsJsonAsync("/api/activities", new
        {
            userId = userId.ToString(),
            datetime = "2026-06-30T10:30:00Z",
            sport = "running",
            distance = 42.195m,
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<IngestActivityResponse>();
        Assert.Equal(4219, body!.Points);
    }

    [Fact]
    public async Task Ingest_StepsWithoutSport_TreatedAsDailySteps()
    {
        var userId = await RegisterAsync("Stepper", "One");

        var response = await _client.PostAsJsonAsync("/api/activities", new
        {
            userId = userId.ToString(),
            datetime = "2026-06-30T10:30:00Z",
            steps = 399,
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<IngestActivityResponse>();
        Assert.Equal("DailySteps", body!.Sport);
        Assert.Equal(3, body.Points);
    }

    [Fact]
    public async Task Ingest_SwimmingWithDistance_Returns400()
    {
        var userId = await RegisterAsync("Swimmer", "Wrong");

        var response = await _client.PostAsJsonAsync("/api/activities", new
        {
            userId = userId.ToString(),
            datetime = "2026-06-30T10:30:00Z",
            sport = "swimming",
            distance = 42.195m,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Ingest_UnknownUser_Returns404()
    {
        var response = await _client.PostAsJsonAsync("/api/activities", new
        {
            userId = Guid.NewGuid().ToString(),
            datetime = "2026-06-30T10:30:00Z",
            sport = "running",
            distance = 5m,
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Leaderboard_ReflectsIngestedPoints()
    {
        var userId = await RegisterAsync("Leader", "Board");
        await _client.PostAsJsonAsync("/api/activities", new
        {
            userId = userId.ToString(),
            datetime = "2026-06-30T10:30:00Z",
            sport = "running",
            distance = 10m,
        });

        var leaderboard = await _client.GetFromJsonAsync<List<LeaderboardEntryDto>>("/api/leaderboard");
        var entry = leaderboard!.Single(e => e.UserId == userId);

        Assert.Equal(1000, entry.TotalPoints);
    }
}
