using FitnessChallenge.Api.Domain;
using FitnessChallenge.Api.Domain.Entities;
using FitnessChallenge.Api.Domain.Scoring;
using Microsoft.EntityFrameworkCore;

namespace FitnessChallenge.Api.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        FitnessDbContext db, IScoringService scoring, TimeProvider clock, CancellationToken ct = default)
    {
        if (await db.Users.AnyAsync(ct))
            return;

        var now = clock.GetUtcNow().UtcDateTime;
        var rng = new Random(42);

        var people = new (string First, string Last, Sport Favorite)[]
        {
            ("Ana", "Kovac", Sport.Running),
            ("Marko", "Maric", Sport.Cycling),
            ("Ivana", "Novak", Sport.Swimming),
            ("Petar", "Peric", Sport.DailySteps),
            ("Luka", "Horvat", Sport.Gym),
            ("Sara", "Babic", Sport.Walking),
        };

        foreach (var person in people)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = person.First,
                LastName = person.Last,
                CreatedAt = now.AddDays(-25),
            };
            db.Users.Add(user);

            var activityDays = rng.Next(10, 20);
            for (var i = 0; i < activityDays; i++)
            {
                var sport = rng.Next(0, 10) < 6 ? person.Favorite : RandomSport(rng);
                var occurredAt = now
                    .AddDays(-rng.Next(0, 21))
                    .AddHours(rng.Next(6, 21))
                    .AddMinutes(rng.Next(0, 60));

                var measurement = BuildMeasurement(sport, rng);
                var points = scoring.CalculatePoints(sport, measurement);

                db.Activities.Add(new Activity
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Sport = sport,
                    OccurredAt = occurredAt,
                    DistanceKm = measurement.DistanceKm,
                    Duration = measurement.Duration,
                    Steps = measurement.Steps,
                    Points = points,
                    CreatedAt = occurredAt,
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static Sport RandomSport(Random rng)
    {
        var all = Enum.GetValues<Sport>();
        return all[rng.Next(all.Length)];
    }

    private static ActivityMeasurement BuildMeasurement(Sport sport, Random rng) => SportInfo.MetricFor(sport) switch
    {
        MetricType.Distance => new ActivityMeasurement(
            DistanceKm: Math.Round((decimal)(rng.NextDouble() * DistanceRange(sport) + 1), 2)),
        MetricType.Duration => new ActivityMeasurement(
            Duration: $"{rng.Next(15, 75)}:{rng.Next(0, 60):D2}"),
        MetricType.Count => new ActivityMeasurement(
            Steps: rng.Next(2000, 16000)),
        _ => throw new InvalidOperationException($"Unhandled metric for {sport}."),
    };

    private static double DistanceRange(Sport sport) => sport switch
    {
        Sport.Cycling => 35,
        Sport.Running => 12,
        _ => 7,
    };
}
