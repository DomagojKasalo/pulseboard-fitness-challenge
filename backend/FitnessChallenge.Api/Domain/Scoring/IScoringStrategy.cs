namespace FitnessChallenge.Api.Domain.Scoring;

public interface IScoringStrategy
{
    Sport Sport { get; }

    int CalculatePoints(ActivityMeasurement measurement);
}
