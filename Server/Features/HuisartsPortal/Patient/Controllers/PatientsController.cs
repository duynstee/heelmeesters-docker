using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.Interfaces;

namespace HeelmeestersAPI.Features.HuisartsPortal.Patient.Controllers;

[ApiController]
[Route("api/huisarts/patienten")]
[Authorize(Roles = "GeneralPractitioner")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientsController(IPatientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyPatients([FromQuery] string? search = null, [FromQuery] int take = 50)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var userId))
            return Unauthorized("Ongeldige user id in token.");

        var list = await _service.GetMyPatientsAsync(userId, search, take);
        return Ok(list);
    }
}