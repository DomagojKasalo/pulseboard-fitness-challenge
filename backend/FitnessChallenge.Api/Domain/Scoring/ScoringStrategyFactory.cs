namespace FitnessChallenge.Api.Domain.Scoring;

public static class ScoringStrategyFactory
{
    public const int StepsPerPoint = 100;

    public static IReadOnlyList<IScoringStrategy> CreateDefault() =>
    [
        new DistanceScoringStrategy(Sport.Running, pointsPerKm: 100m),
        new DistanceScoringStrategy(Sport.Walking, pointsPerKm: 50m),
        new DistanceScoringStrategy(Sport.Cycling, pointsPerKm: 25m),
        new DurationScoringStrategy(Sport.Swimming, pointsPerMinute: 15),
        new DurationScoringStrategy(Sport.Gym, pointsPerMinute: 5),
        new StepsScoringStrategy(StepsPerPoint),
    ];

    public static IScoringService CreateService() => new ScoringService(CreateDefault());
}
