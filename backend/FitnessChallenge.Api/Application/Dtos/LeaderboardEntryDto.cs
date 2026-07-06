namespace FitnessChallenge.Api.Application.Dtos;

public sealed record LeaderboardEntryDto(
    int Rank,
    Guid UserId,
    string FirstName,
    string LastName,
    int TotalPoints,
    int? PreviousRank,
    int RankDelta,
    string Trend);
