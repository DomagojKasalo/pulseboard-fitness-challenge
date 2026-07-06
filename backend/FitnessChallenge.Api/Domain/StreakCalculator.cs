namespace FitnessChallenge.Api.Domain;

public static class StreakCalculator
{
    public static (int Current, int Longest) Compute(IEnumerable<DateOnly> activeDays, DateOnly today)
    {
        var days = activeDays.Distinct().OrderBy(d => d).ToList();
        if (days.Count == 0)
            return (0, 0);

        var longest = 1;
        var run = 1;
        for (var i = 1; i < days.Count; i++)
        {
            run = days[i].DayNumber - days[i - 1].DayNumber == 1 ? run + 1 : 1;
            longest = Math.Max(longest, run);
        }

        var set = days.ToHashSet();
        DateOnly anchor;
        if (set.Contains(today))
            anchor = today;
        else if (set.Contains(today.AddDays(-1)))
            anchor = today.AddDays(-1); // grace: today isn't over yet
        else
            return (0, longest);

        var current = 0;
        for (var d = anchor; set.Contains(d); d = d.AddDays(-1))
            current++;

        return (current, longest);
    }
}
