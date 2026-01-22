using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HeelmeestersAPI.Features.Shared.Appointments;

namespace HeelmeestersAPI.Features.PatientPortal.Referrals;

[ApiController]
[Route("api/patient/referrals")]
[Authorize(Roles = "Patient")]
public class PatientReferralsController : ControllerBase
{
    private readonly IAppointmentService _appointments;

    public PatientReferralsController(IAppointmentService appointments)
    {
        _appointments = appointments;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var userId))
            return Unauthorized("Ongeldige user id in token.");

        try
        {
            var list = await _appointments.GetActiveReferralsForLoggedInPatientAsync(userId);
            return Ok(list);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}