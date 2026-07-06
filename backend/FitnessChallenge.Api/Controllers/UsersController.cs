using FitnessChallenge.Api.Application.Dtos;
using FitnessChallenge.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessChallenge.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    private readonly IDashboardService _dashboard;

    public UsersController(IUserService users, IDashboardService dashboard)
    {
        _users = users;
        _dashboard = dashboard;
    }

    [HttpPost]
    public async Task<ActionResult<RegisterUserResponse>> Register(RegisterUserRequest request, CancellationToken ct)
    {
        var result = await _users.RegisterAsync(request, ct);
        return CreatedAtAction(nameof(GetDashboard), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserSummaryDto>>> GetUsers(CancellationToken ct) =>
        Ok(await _users.GetUsersAsync(ct));

    [HttpGet("{id:guid}/dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(Guid id, CancellationToken ct) =>
        Ok(await _dashboard.GetDashboardAsync(id, ct));
}
