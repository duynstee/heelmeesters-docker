using System.Security.Claims;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Interfaces;
using HeelmeestersAPI.Features.Shared.Appointments.Interfaces;

namespace HeelmeestersAPI.Features.HuisartsPortal.Referral.Controllers;

[ApiController]
[Route("api/huisarts/patienten/{patientNumber:long}/referrals")]
[Authorize(Roles = "GeneralPractitioner")]
public class ReferralsController : ControllerBase
{
    private readonly IReferralService _service;

    public ReferralsController(IReferralService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetForPatient([FromRoute] long patientNumber)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var userId))
            return Unauthorized("Ongeldige user id in token.");

        try
        {
            var list = await _service.GetForSelectedPatientAsync(userId, patientNumber);
            return Ok(list);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromRoute] long patientNumber,
        [FromBody] CreateReferralRequestDto request)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var userId))
            return Unauthorized("Ongeldige user id in token.");

        try
        {
            var created = await _service.CreateAsync(userId, patientNumber, request);
            return Ok(created);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}