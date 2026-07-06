namespace FitnessChallenge.Api.Domain.Scoring;

public sealed class DurationScoringStrategy : IScoringStrategy
{
    private readonly int _pointsPerMinute;

    public DurationScoringStrategy(Sport sport, int pointsPerMinute)
    {
        Sport = sport;
        _pointsPerMinute = pointsPerMinute;
    }

    public Sport Sport { get; }

    public int CalculatePoints(ActivityMeasurement measurement)
    {
        var raw = measurement.Duration
            ?? throw new InvalidOperationException($"{Sport} scoring requires a duration value.");

        return Duration.Parse(raw).WholeMinutes * _pointsPerMinute;
    }
}
