using System.Globalization;

namespace FitnessChallenge.Api.Application.Validation;

public static class IsoDateTime
{
    public static bool TryParse(string? input, out DateTime utc)
    {
        utc = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (!DateTimeOffset.TryParse(input, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var parsed))
            return false;

        utc = parsed.UtcDateTime;
        return true;
    }
}
