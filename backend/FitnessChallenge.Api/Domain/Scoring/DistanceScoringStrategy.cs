namespace FitnessChallenge.Api.Domain.Scoring;

public sealed class DistanceScoringStrategy : IScoringStrategy
{
    private readonly decimal _pointsPerKm;

    public DistanceScoringStrategy(Sport sport, decimal pointsPerKm)
    {
        Sport = sport;
        _pointsPerKm = pointsPerKm;
    }

    public Sport Sport { get; }

    public int CalculatePoints(ActivityMeasurement measurement)
    {
        var km = measurement.DistanceKm
            ?? throw new InvalidOperationException($"{Sport} scoring requires a distance value.");

        return (int)Math.Floor(km * _pointsPerKm);
    }
}
