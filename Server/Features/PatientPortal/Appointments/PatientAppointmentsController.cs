using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.Shared.Appointments;

namespace HeelmeestersAPI.Features.PatientPortal.Appointments
{
    [ApiController]
    [Route("api/patient/appointments")]
    [Authorize(Roles = "Patient")]
    public class PatientAppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public PatientAppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var userId))
                return Unauthorized("Ongeldige user id in token");
            
            long patientNumber;
            try
            {
                patientNumber = await _service.GetPatientNumberForUserAsync(userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            var appointments = await _service.GetAppointmentsForPatientAsync(patientNumber);

            var dtos = appointments.Select(a => new PatientAppointmentDto
            {
                Id = a.Id,
                CareCode = a.CareCode,
                RoomCode = a.RoomCode,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost("from-referral")]
        public async Task<IActionResult> CreateFromReferral([FromBody] CreateAppointmentFromReferralRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var userId))
                return Unauthorized("Ongeldige user id in token");

            try
            {
                var created = await _service.CreatePatientAppointmentFromReferralAsync(
                    userId,
                    request.ReferralId,
                    request.EmployeeNumber,
                    request.RoomCode,
                    request.StartTime,
                    request.EndTime
                );

                // teruggeven in dezelfde vorm als je GET
                var dto = new PatientAppointmentDto
                {
                    Id = created.Id,
                    CareCode = created.CareCode, // komt uit referral
                    RoomCode = created.RoomCode,
                    StartTime = created.StartTime,
                    EndTime = created.EndTime
                };

                return Ok(dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // bv doctor not available
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var list = await _service.GetDoctorOptionsAsync();
            return Ok(list);
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var list = await _service.GetRoomOptionsAsync();
            return Ok(list);
        }
        
        [HttpGet("doctors/{employeeNumber:long}/schedule/week")]
        public async Task<IActionResult> GetDoctorWeekSchedule(
            [FromRoute] long employeeNumber,
            [FromQuery] DateTime weekStart)
        {
            // weekStart is local date; we pakken ma 00:00 t/m ma+7
            var start = weekStart.Date;
            var end = start.AddDays(7);

            var list = await _service.GetAppointmentsForDoctorInRangeAsync(employeeNumber, start, end);

            // return minimal DTO (start/end is genoeg)
            var dtos = list.Select(a => new
            {
                a.Id,
                a.StartTime,
                a.EndTime
            });

            return Ok(dtos);
        }
    }
}