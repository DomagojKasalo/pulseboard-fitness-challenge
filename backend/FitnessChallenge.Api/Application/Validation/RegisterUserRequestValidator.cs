using FitnessChallenge.Api.Application.Dtos;
using FluentValidation;

namespace FitnessChallenge.Api.Application.Validation;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("firstName is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("lastName is required.")
            .MaximumLength(100);
    }
}
