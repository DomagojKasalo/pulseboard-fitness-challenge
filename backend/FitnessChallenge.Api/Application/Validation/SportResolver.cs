using FitnessChallenge.Api.Domain;

namespace FitnessChallenge.Api.Application.Validation;

public static class SportResolver
{
    private static readonly IReadOnlyDictionary<string, Sport> ApiSports =
        new Dictionary<string, Sport>(StringComparer.OrdinalIgnoreCase)
        {
            ["running"] = Sport.Running,
            ["walking"] = Sport.Walking,
            ["cycling"] = Sport.Cycling,
            ["gym"] = Sport.Gym,
            ["swimming"] = Sport.Swimming,
        };

    public static bool TryResolve(string? sport, bool hasSteps, out Sport resolved)
    {
        if (!string.IsNullOrWhiteSpace(sport))
            return ApiSports.TryGetValue(sport.Trim(), out resolved);

        if (hasSteps)
        {
            resolved = Sport.DailySteps;
            return true;
        }

        resolved = default;
        return false;
    }
}
