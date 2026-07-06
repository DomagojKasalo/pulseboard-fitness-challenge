namespace FitnessChallenge.Api.Application;

public sealed class DuplicateUserException(string firstName, string lastName)
    : Exception($"A user named '{firstName} {lastName}' already exists.");

public sealed class UserNotFoundException(Guid userId)
    : Exception($"User '{userId}' was not found.");
