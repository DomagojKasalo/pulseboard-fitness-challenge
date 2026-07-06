using System.Globalization;

namespace FitnessChallenge.Api.Domain;

public readonly record struct Duration
{
    public int TotalSeconds { get; }

    public int WholeMinutes => TotalSeconds / 60;

    private Duration(int totalSeconds) => TotalSeconds = totalSeconds;

    public static bool TryParse(string? input, out Duration duration)
    {
        duration = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var parts = input.Split(':');
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var minutes) ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
            return false;

        if (seconds is < 0 or > 59)
            return false;

        duration = new Duration(minutes * 60 + seconds);
        return true;
    }

    public static Duration Parse(string input) =>
        TryParse(input, out var duration)
            ? duration
            : throw new FormatException($"Invalid duration '{input}'. Expected format 'minutes:seconds'.");
}
