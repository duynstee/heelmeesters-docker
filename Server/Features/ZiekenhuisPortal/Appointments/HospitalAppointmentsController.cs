using HeelmeestersAPI.Features.Shared.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeelmeestersAPI.Features.ZiekenhuisPortal.Appointments
{
    [ApiController]
    [Route("api/hospital/appointments")]
    [Authorize(Roles = "Doctor")]
    public class HospitalAppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public HospitalAppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet("schedule/week")]
        public async Task<IActionResult> GetWeekSchedule([FromQuery] DateTime weekStart, long employeeNumber)
        {
            try
            {
                DateTime weekEnd = weekStart.AddDays(7);


                var appointments = await _service.GetAppointmentsForDoctorInRangeAsync(employeeNumber, weekStart, weekEnd);

                var dtos = appointments.Select(a => new DoctorAppointmentDto
                {
                    Id = a.Id,
                    PatientNumber = a.PatientNumber,
                    CareCode = a.CareCode,
                    RoomCode = a.RoomCode,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("appointment")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateDoctorAppointmentDto dto)
        {
            try
            {
                long employeeNumber = dto.EmployeeNumber;

                // Eindtijd bepalen:
                // - als client EndTime meegeeft, gebruiken we die
                // - anders pakken we default 30 minuten (tot we duration in DB hebben)
                var endTime = dto.StartTime.AddMinutes(30);

                var result = await _service.CreateDoctorAppointmentAsync(
                    employeeNumber,
                    dto.PatientNumber,
                    dto.CareCode,
                    dto.RoomCode,
                    dto.StartTime,
                    endTime
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("appointment/cancel/{id}")]
        public async Task<IActionResult> CancelAppointment([FromRoute] int id)
        {
            try
            {
                await _service.CancelAppointmentAsync(id);

                return Ok(new
                {
                    message = "Hospital appointment has been cancelled",
                    appointmentId = id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // AppointmentType -> Treatments in nieuwe DB
        [HttpGet("treatments")]
        public async Task<IActionResult> GetTreatments()
        {
            try
            {
                var result = await _service.GetTreatmentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var result = await _service.GetDoctorOptionsAsync();
            return Ok(result);
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            var result = await _service.GetPatientOptionsAsync();
            return Ok(result);
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var result = await _service.GetRoomOptionsAsync();
            return Ok(result);
        }
    }
}