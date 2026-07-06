namespace FitnessChallenge.Api.Domain.Scoring;

public interface IScoringService
{
    int CalculatePoints(Sport sport, ActivityMeasurement measurement);
}
