namespace FitnessChallenge.Api.Application.Dtos;

public sealed record DashboardDto(
    Guid UserId,
    string FirstName,
    string LastName,
    int TotalPoints,
    int? Rank,
    int ActivityCount,
    int CurrentStreak,
    int LongestStreak,
    IReadOnlyList<ActivityHistoryItem> History,
    IReadOnlyList<DailyVolumePoint> VolumeOverTime,
    IReadOnlyList<SportBreakdownSlice> SportBreakdown);

public sealed record DailyVolumePoint(DateOnly Date, int Points, int ActivityCount);

public sealed record SportBreakdownSlice(string Sport, int Points, int ActivityCount);

public sealed record UserSummaryDto(Guid Id, string FirstName, string LastName);
