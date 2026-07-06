namespace FitnessChallenge.Api.Domain.Scoring;

public sealed class ScoringService : IScoringService
{
    private readonly IReadOnlyDictionary<Sport, IScoringStrategy> _strategies;

    public ScoringService(IEnumerable<IScoringStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Sport);
    }

    public int CalculatePoints(Sport sport, ActivityMeasurement measurement)
    {
        if (!_strategies.TryGetValue(sport, out var strategy))
            throw new InvalidOperationException($"No scoring strategy registered for sport '{sport}'.");

        return strategy.CalculatePoints(measurement);
    }
}
