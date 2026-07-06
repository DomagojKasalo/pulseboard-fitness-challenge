using FitnessChallenge.Api.Domain;

namespace FitnessChallenge.Tests;

public class StreakCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 7, 6);

    private static DateOnly D(int daysAgo) => Today.AddDays(-daysAgo);

    [Fact]
    public void NoActivity_ReturnsZero()
    {
        var (current, longest) = StreakCalculator.Compute([], Today);
        Assert.Equal(0, current);
        Assert.Equal(0, longest);
    }

    [Fact]
    public void SingleDayToday_IsStreakOfOne()
    {
        var (current, longest) = StreakCalculator.Compute([Today], Today);
        Assert.Equal(1, current);
        Assert.Equal(1, longest);
    }

    [Fact]
    public void ThreeConsecutiveEndingToday()
    {
        var (current, longest) = StreakCalculator.Compute([D(2), D(1), D(0)], Today);
        Assert.Equal(3, current);
        Assert.Equal(3, longest);
    }

    [Fact]
    public void GapResetsCurrentButKeepsLongest()
    {
        var (current, longest) = StreakCalculator.Compute([D(5), D(4), D(0)], Today);
        Assert.Equal(1, current);
        Assert.Equal(2, longest);
    }

    [Fact]
    public void StreakEndingYesterday_StillCurrent_GracePeriod()
    {
        var (current, longest) = StreakCalculator.Compute([D(2), D(1)], Today);
        Assert.Equal(2, current);
        Assert.Equal(2, longest);
    }

    [Fact]
    public void MostRecentOlderThanYesterday_CurrentIsZero()
    {
        var (current, longest) = StreakCalculator.Compute([D(4), D(3)], Today);
        Assert.Equal(0, current);
        Assert.Equal(2, longest);
    }

    [Fact]
    public void DuplicateDaysAreIgnored()
    {
        var (current, longest) = StreakCalculator.Compute([Today, Today, D(1)], Today);
        Assert.Equal(2, current);
        Assert.Equal(2, longest);
    }
}
