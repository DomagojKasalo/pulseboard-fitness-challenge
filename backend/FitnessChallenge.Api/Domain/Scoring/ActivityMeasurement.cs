namespace FitnessChallenge.Api.Domain.Scoring;

public sealed record ActivityMeasurement(
    decimal? DistanceKm = null,
    string? Duration = null,
    int? Steps = null);
