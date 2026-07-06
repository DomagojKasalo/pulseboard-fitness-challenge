namespace FitnessChallenge.Api.Application.Dtos;

public sealed record RegisterUserRequest(string? FirstName, string? LastName);

public sealed record RegisterUserResponse(Guid Id, string FirstName, string LastName);
