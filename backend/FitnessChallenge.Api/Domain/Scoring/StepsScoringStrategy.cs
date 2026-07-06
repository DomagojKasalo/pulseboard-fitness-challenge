namespace FitnessChallenge.Api.Domain.Scoring;

public sealed class StepsScoringStrategy : IScoringStrategy
{
    private readonly int _stepsPerPoint;

    public StepsScoringStrategy(int stepsPerPoint)
    {
        _stepsPerPoint = stepsPerPoint;
    }

    public Sport Sport => Sport.DailySteps;

    public int CalculatePoints(ActivityMeasurement measurement)
    {
        var steps = measurement.Steps
            ?? throw new InvalidOperationException("Daily steps scoring requires a step count.");

        return steps / _stepsPerPoint;
    }
}
