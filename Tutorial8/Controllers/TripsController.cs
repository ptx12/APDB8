using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly ITripsService _s;
    public TripsController(ITripsService s)
    {
        _s = s;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _s.GetTrips();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _s.GetTrip(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}