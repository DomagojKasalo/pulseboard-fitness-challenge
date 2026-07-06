using FitnessChallenge.Api.Domain;
using FitnessChallenge.Api.Domain.Scoring;

namespace FitnessChallenge.Tests;

public class ScoringServiceTests
{
    private readonly IScoringService _scoring = ScoringStrategyFactory.CreateService();

    [Theory]
    [InlineData(Sport.Running, 1.0, 100)]
    [InlineData(Sport.Running, 42.195, 4219)]
    [InlineData(Sport.Walking, 1.0, 50)]
    [InlineData(Sport.Walking, 1.55, 77)]
    [InlineData(Sport.Cycling, 1.0, 25)]
    [InlineData(Sport.Cycling, 10.0, 250)]
    [InlineData(Sport.Running, 0.0, 0)]
    [InlineData(Sport.Walking, 0.019, 0)]
    public void Distance_FloorsFinalPoints(Sport sport, double distanceKm, int expected)
    {
        var points = _scoring.CalculatePoints(sport, new ActivityMeasurement(DistanceKm: (decimal)distanceKm));
        Assert.Equal(expected, points);
    }

    [Theory]
    [InlineData(Sport.Swimming, "1:55", 15)]
    [InlineData(Sport.Swimming, "2:00", 30)]
    [InlineData(Sport.Swimming, "0:59", 0)]
    [InlineData(Sport.Gym, "1:55", 5)]
    [InlineData(Sport.Gym, "10:30", 50)]
    [InlineData(Sport.Gym, "0:30", 0)]
    public void Duration_FloorsToWholeMinutesFirst(Sport sport, string duration, int expected)
    {
        var points = _scoring.CalculatePoints(sport, new ActivityMeasurement(Duration: duration));
        Assert.Equal(expected, points);
    }

    [Theory]
    [InlineData(399, 3)]
    [InlineData(100, 1)]
    [InlineData(99, 0)]
    [InlineData(250, 2)]
    [InlineData(0, 0)]
    [InlineData(10000, 100)]
    public void Steps_FloorToHundredBlocks(int steps, int expected)
    {
        var points = _scoring.CalculatePoints(Sport.DailySteps, new ActivityMeasurement(Steps: steps));
        Assert.Equal(expected, points);
    }

    [Fact]
    public void Throws_WhenRequiredMetricMissing()
    {
        Assert.Throws<InvalidOperationException>(
            () => _scoring.CalculatePoints(Sport.Running, new ActivityMeasurement(Steps: 500)));
    }
}
