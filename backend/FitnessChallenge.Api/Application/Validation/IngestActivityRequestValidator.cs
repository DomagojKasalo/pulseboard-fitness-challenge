using FitnessChallenge.Api.Application.Dtos;
using FitnessChallenge.Api.Domain;
using FluentValidation;

namespace FitnessChallenge.Api.Application.Validation;

public sealed class IngestActivityRequestValidator : AbstractValidator<IngestActivityRequest>
{
    public IngestActivityRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("userId is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("userId must be a valid identifier.");

        RuleFor(x => x.Datetime)
            .NotEmpty().WithMessage("datetime is required.")
            .Must(dt => IsoDateTime.TryParse(dt, out _)).WithMessage("datetime must be a valid ISO 8601 timestamp.");

        RuleFor(x => x).Custom(ValidateSportAndMetric);
    }

    private static void ValidateSportAndMetric(IngestActivityRequest req, ValidationContext<IngestActivityRequest> ctx)
    {
        if (!SportResolver.TryResolve(req.Sport, req.Steps.HasValue, out var sport))
        {
            ctx.AddFailure("sport",
                "Provide a valid 'sport' (running, walking, cycling, gym, swimming), or 'steps' for daily steps.");
            return;
        }

        switch (SportInfo.MetricFor(sport))
        {
            case MetricType.Distance:
                RequirePositiveDistance(req, sport, ctx);
                RejectField(req.Duration is not null, "duration", sport, ctx);
                RejectField(req.Steps is not null, "steps", sport, ctx);
                break;

            case MetricType.Duration:
                RequireValidDuration(req, sport, ctx);
                RejectField(req.Distance is not null, "distance", sport, ctx);
                RejectField(req.Steps is not null, "steps", sport, ctx);
                break;

            case MetricType.Count:
                RequirePositiveSteps(req, ctx);
                RejectField(req.Distance is not null, "distance", sport, ctx);
                RejectField(req.Duration is not null, "duration", sport, ctx);
                break;
        }
    }

    private static void RequirePositiveDistance(IngestActivityRequest req, Sport sport, ValidationContext<IngestActivityRequest> ctx)
    {
        if (req.Distance is null or <= 0)
            ctx.AddFailure("distance", $"{sport} requires a positive 'distance' in kilometers.");
    }

    private static void RequireValidDuration(IngestActivityRequest req, Sport sport, ValidationContext<IngestActivityRequest> ctx)
    {
        if (req.Duration is null || !Duration.TryParse(req.Duration, out var d) || d.TotalSeconds <= 0)
            ctx.AddFailure("duration", $"{sport} requires a positive 'duration' in 'minutes:seconds' format.");
    }

    private static void RequirePositiveSteps(IngestActivityRequest req, ValidationContext<IngestActivityRequest> ctx)
    {
        if (req.Steps is null or <= 0)
            ctx.AddFailure("steps", "Daily steps requires a positive 'steps' count.");
    }

    private static void RejectField(bool present, string field, Sport sport, ValidationContext<IngestActivityRequest> ctx)
    {
        if (present)
            ctx.AddFailure(field, $"'{field}' is not valid for {sport}.");
    }
}
