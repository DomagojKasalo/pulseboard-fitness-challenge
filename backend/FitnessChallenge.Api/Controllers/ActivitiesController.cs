using FitnessChallenge.Api.Application.Dtos;
using FitnessChallenge.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessChallenge.Api.Controllers;

[ApiController]
[Route("api/activities")]
public class ActivitiesController : ControllerBase
{
    private readonly IActivityService _activities;

    public ActivitiesController(IActivityService activities)
    {
        _activities = activities;
    }

    [HttpPost]
    public async Task<ActionResult<IngestActivityResponse>> Ingest(IngestActivityRequest request, CancellationToken ct)
    {
        var result = await _activities.IngestAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
