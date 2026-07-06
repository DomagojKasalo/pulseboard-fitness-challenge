using FitnessChallenge.Api.Application.Dtos;
using FitnessChallenge.Api.Application.Validation;
using FitnessChallenge.Api.Domain.Entities;
using FitnessChallenge.Api.Domain.Scoring;
using FitnessChallenge.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessChallenge.Api.Application.Services;

public interface IActivityService
{
    Task<IngestActivityResponse> IngestAsync(IngestActivityRequest request, CancellationToken ct);
}

public sealed class ActivityService : IActivityService
{
    private readonly FitnessDbContext _db;
    private readonly IScoringService _scoring;
    private readonly TimeProvider _clock;

    public ActivityService(FitnessDbContext db, IScoringService scoring, TimeProvider clock)
    {
        _db = db;
        _scoring = scoring;
        _clock = clock;
    }

    public async Task<IngestActivityResponse> IngestAsync(IngestActivityRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(request.UserId!);
        IsoDateTime.TryParse(request.Datetime, out var occurredAt);
        SportResolver.TryResolve(request.Sport, request.Steps.HasValue, out var sport);

        if (!await _db.Users.AnyAsync(u => u.Id == userId, ct))
            throw new UserNotFoundException(userId);

        var measurement = new ActivityMeasurement(request.Distance, request.Duration, request.Steps);
        var points = _scoring.CalculatePoints(sport, measurement);

        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Sport = sport,
            OccurredAt = occurredAt,
            DistanceKm = request.Distance,
            Duration = request.Duration,
            Steps = request.Steps,
            Points = points,
            CreatedAt = _clock.GetUtcNow().UtcDateTime,
        };

        _db.Activities.Add(activity);
        await _db.SaveChangesAsync(ct);

        return new IngestActivityResponse(activity.Id, userId, sport.ToString(), occurredAt, points);
    }
}
