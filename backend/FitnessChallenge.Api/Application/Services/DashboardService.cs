using FitnessChallenge.Api.Application.Dtos;
using FitnessChallenge.Api.Domain;
using FitnessChallenge.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessChallenge.Api.Application.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(Guid userId, CancellationToken ct);
}

public sealed class DashboardService : IDashboardService
{
    private readonly FitnessDbContext _db;
    private readonly ILeaderboardService _leaderboard;
    private readonly TimeProvider _clock;

    public DashboardService(FitnessDbContext db, ILeaderboardService leaderboard, TimeProvider clock)
    {
        _db = db;
        _leaderboard = leaderboard;
        _clock = clock;
    }

    public async Task<DashboardDto> GetDashboardAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new UserNotFoundException(userId);

        var activities = await _db.Activities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync(ct);

        var history = activities
            .Select(a => new ActivityHistoryItem(
                a.Id, a.Sport.ToString(), a.OccurredAt, a.Points, a.DistanceKm, a.Duration, a.Steps))
            .ToList();

        var volumeOverTime = activities
            .GroupBy(a => DateOnly.FromDateTime(a.OccurredAt))
            .OrderBy(g => g.Key)
            .Select(g => new DailyVolumePoint(g.Key, g.Sum(a => a.Points), g.Count()))
            .ToList();

        var sportBreakdown = activities
            .GroupBy(a => a.Sport)
            .Select(g => new SportBreakdownSlice(g.Key.ToString(), g.Sum(a => a.Points), g.Count()))
            .OrderByDescending(s => s.Points)
            .ToList();

        var leaderboard = await _leaderboard.GetLeaderboardAsync(ct);
        var rank = leaderboard.FirstOrDefault(e => e.UserId == userId)?.Rank;

        var today = DateOnly.FromDateTime(_clock.GetUtcNow().UtcDateTime);
        var activeDays = activities.Select(a => DateOnly.FromDateTime(a.OccurredAt));
        var (currentStreak, longestStreak) = StreakCalculator.Compute(activeDays, today);

        return new DashboardDto(
            user.Id, user.FirstName, user.LastName,
            activities.Sum(a => a.Points), rank, activities.Count,
            currentStreak, longestStreak,
            history, volumeOverTime, sportBreakdown);
    }
}
