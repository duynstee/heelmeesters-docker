using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HeelmeestersAPI.Features.Shared.Appointments.Interfaces;

namespace HeelmeestersAPI.Features.HuisartsPortal.Treatments.Controllers;

[ApiController]
[Route("api/huisarts/treatments")]
[Authorize(Roles = "GeneralPractitioner")]
public class GpTreatmentsController : ControllerBase
{
    private readonly ITreatmentProvider _treatments;

    public GpTreatmentsController(ITreatmentProvider treatments)
    {
        _treatments = treatments;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _treatments.GetTreatmentsAsync();

        // Alleen wat de frontend nodig heeft
        var result = list.Select(t => new
        {
            careCode = t.CareCode,
            description = t.Description
        });

        return Ok(result);
    }
}