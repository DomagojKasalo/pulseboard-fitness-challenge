namespace FitnessChallenge.Api.Domain;

public enum Sport
{
    Running,
    Walking,
    Cycling,
    Gym,
    Swimming,
    DailySteps
}

public enum MetricType
{
    Distance,
    Duration,
    Count
}

public static class SportInfo
{
    private static readonly IReadOnlyDictionary<Sport, MetricType> Metrics = new Dictionary<Sport, MetricType>
    {
        [Sport.Running] = MetricType.Distance,
        [Sport.Walking] = MetricType.Distance,
        [Sport.Cycling] = MetricType.Distance,
        [Sport.Gym] = MetricType.Duration,
        [Sport.Swimming] = MetricType.Duration,
        [Sport.DailySteps] = MetricType.Count,
    };

    public static MetricType MetricFor(Sport sport) => Metrics[sport];
}
