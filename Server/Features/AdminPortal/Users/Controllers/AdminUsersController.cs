using HeelmeestersAPI.Features.AdminPortal.Users.Interfaces;
using HeelmeestersAPI.Features.AdminPortal.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeelmeestersAPI.Features.AdminPortal.Users.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminUsersController(IAdminUserService service)
    {
        _service = service;
    }

    [HttpPost("patient")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientAccountDto dto)
    {
        try
        {
            await _service.CreatePatientAccountAsync(dto);
            return Ok(new { message = "Patient account aangemaakt" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("general-practitioner")]
    public async Task<IActionResult> CreateGeneralPractitioner([FromBody] CreateGeneralPractitionerAccountDto dto)
    {
        try
        {
            await _service.CreateGeneralPractitionerAccountAsync(dto);
            return Ok(new { message = "Huisarts account aangemaakt" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("hospital-staff")]
    public async Task<IActionResult> CreateHospitalStaff([FromBody] CreateHospitalStaffAccountDto dto)
    {
        try
        {
            await _service.CreateHospitalStaffAccountAsync(dto);
            return Ok(new { message = "Specialist account aangemaakt" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _service.GetUsersAsync();
        return Ok(result);
    }

    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients()
    {
        var result = await _service.GetPatientsAsync();
        return Ok(result);
    }

    [HttpGet("general-practitioners")]
    public async Task<IActionResult> GetGeneralPractitioners()
    {
        var result = await _service.GetGeneralPractitionersAsync();
        return Ok(result);
    }

    [HttpGet("hospital-staff")]
    public async Task<IActionResult> GetHospitalStaff()
    {
        var result = await _service.GetHospitalStaffAsync();
        return Ok(result);
    }
}