using FitnessChallenge.Api.Application.Dtos;
using FitnessChallenge.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessChallenge.Api.Application.Services;

public interface ILeaderboardService
{
    Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(CancellationToken ct);
}

public sealed class LeaderboardService : ILeaderboardService
{
    private const int TrendWindowDays = 7;

    private readonly FitnessDbContext _db;
    private readonly TimeProvider _clock;

    public LeaderboardService(FitnessDbContext db, TimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(CancellationToken ct)
    {
        var cutoff = _clock.GetUtcNow().UtcDateTime.AddDays(-TrendWindowDays);

        var users = await _db.Users
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToListAsync(ct);

        var currentTotals = await SumPointsByUserAsync(all: true, cutoff, ct);
        var previousTotals = await SumPointsByUserAsync(all: false, cutoff, ct);

        var currentRanks = RankByPoints(users.Select(u => (u.Id, currentTotals.GetValueOrDefault(u.Id))));
        var previousRanks = RankByPoints(users.Select(u => (u.Id, previousTotals.GetValueOrDefault(u.Id))));

        return users
            .Select(u =>
            {
                var points = currentTotals.GetValueOrDefault(u.Id);
                var rank = currentRanks[u.Id];
                var hadHistory = previousTotals.ContainsKey(u.Id);
                var previousRank = hadHistory ? previousRanks[u.Id] : (int?)null;
                var delta = previousRank is null ? 0 : previousRank.Value - rank;
                var trend = !hadHistory && points > 0 ? "new"
                    : delta > 0 ? "up"
                    : delta < 0 ? "down"
                    : "same";

                return new LeaderboardEntryDto(rank, u.Id, u.FirstName, u.LastName, points, previousRank, delta, trend);
            })
            .OrderBy(e => e.Rank)
            .ThenBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToList();
    }

    private async Task<Dictionary<Guid, int>> SumPointsByUserAsync(bool all, DateTime cutoff, CancellationToken ct)
    {
        var query = _db.Activities.AsQueryable();
        if (!all)
            query = query.Where(a => a.OccurredAt <= cutoff);

        var totals = await query
            .GroupBy(a => a.UserId)
            .Select(g => new { UserId = g.Key, Points = g.Sum(a => a.Points) })
            .ToListAsync(ct);

        return totals.ToDictionary(t => t.UserId, t => t.Points);
    }

    private static Dictionary<Guid, int> RankByPoints(IEnumerable<(Guid UserId, int Points)> totals)
    {
        var ranks = new Dictionary<Guid, int>();
        var seen = 0;
        var rank = 0;
        var lastPoints = int.MinValue;

        foreach (var entry in totals.OrderByDescending(t => t.Points))
        {
            seen++;
            if (entry.Points != lastPoints)
            {
                rank = seen;
                lastPoints = entry.Points;
            }

            ranks[entry.UserId] = rank;
        }

        return ranks;
    }
}
