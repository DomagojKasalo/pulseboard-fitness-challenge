using FitnessChallenge.Api.Application.Dtos;
using FitnessChallenge.Api.Domain.Entities;
using FitnessChallenge.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessChallenge.Api.Application.Services;

public interface IUserService
{
    Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct);
    Task<IReadOnlyList<UserSummaryDto>> GetUsersAsync(CancellationToken ct);
}

public sealed class UserService : IUserService
{
    private readonly FitnessDbContext _db;
    private readonly TimeProvider _clock;

    public UserService(FitnessDbContext db, TimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct)
    {
        var firstName = request.FirstName!.Trim();
        var lastName = request.LastName!.Trim();

        if (await _db.Users.AnyAsync(u => u.FirstName == firstName && u.LastName == lastName, ct))
            throw new DuplicateUserException(firstName, lastName);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = _clock.GetUtcNow().UtcDateTime,
        };

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            throw new DuplicateUserException(firstName, lastName);
        }

        return new RegisterUserResponse(user.Id, user.FirstName, user.LastName);
    }

    public async Task<IReadOnlyList<UserSummaryDto>> GetUsersAsync(CancellationToken ct) =>
        await _db.Users
            .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
            .Select(u => new UserSummaryDto(u.Id, u.FirstName, u.LastName))
            .ToListAsync(ct);
}
