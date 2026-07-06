using FitnessChallenge.Api.Domain;

namespace FitnessChallenge.Tests;

public class DurationTests
{
    [Theory]
    [InlineData("0:00", 0)]
    [InlineData("0:59", 0)]
    [InlineData("1:00", 1)]
    [InlineData("1:55", 1)]
    [InlineData("2:00", 2)]
    [InlineData("90:30", 90)]
    public void WholeMinutes_FloorsSecondsAway(string input, int expectedMinutes)
    {
        Assert.True(Duration.TryParse(input, out var duration));
        Assert.Equal(expectedMinutes, duration.WholeMinutes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("5")]
    [InlineData("5:")]
    [InlineData("5:99")]
    [InlineData("5:-1")]
    [InlineData("-1:30")]
    [InlineData("5:30:00")]
    [InlineData(" 5:30")]
    [InlineData("5.5:30")]
    public void TryParse_RejectsInvalidFormats(string? input)
    {
        Assert.False(Duration.TryParse(input, out _));
    }

    [Fact]
    public void Parse_ThrowsOnInvalidInput()
    {
        Assert.Throws<FormatException>(() => Duration.Parse("not-a-duration"));
    }
}
