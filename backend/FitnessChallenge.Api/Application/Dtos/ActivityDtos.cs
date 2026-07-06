namespace FitnessChallenge.Api.Application.Dtos;

public sealed record IngestActivityResponse(
    Guid Id,
    Guid UserId,
    string Sport,
    DateTime OccurredAt,
    int Points);

public sealed record ActivityHistoryItem(
    Guid Id,
    string Sport,
    DateTime OccurredAt,
    int Points,
    decimal? DistanceKm,
    string? Duration,
    int? Steps);
