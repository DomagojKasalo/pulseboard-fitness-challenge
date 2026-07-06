namespace FitnessChallenge.Api.Domain.Entities;

public class Activity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Sport Sport { get; set; }
    public DateTime OccurredAt { get; set; }

    public decimal? DistanceKm { get; set; }
    public string? Duration { get; set; }
    public int? Steps { get; set; }

    public int Points { get; set; }

    public DateTime CreatedAt { get; set; }
}
