using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.Shared.Auth.Interfaces;
using HeelmeestersAPI.Features.Shared.MedicalRecords;

namespace HeelmeestersAPI.Features.PatientPortal.MedicalRecords
{
    [ApiController]
    [Route("api/patient/medicalrecords")]
    [Authorize(Roles = "Patient")]
    public class PatientMedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordRepository _repo;
        private readonly IPatientIdentityService _patientIdentity;

        public PatientMedicalRecordController(IMedicalRecordRepository repo, IPatientIdentityService patientIdentity)
        {
            _repo = repo;
            _patientIdentity = patientIdentity;
        }

        [HttpGet]
        public async Task<IActionResult> GetForLoggedInPatient(CancellationToken cancellationToken)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var userId))
            {
                return Unauthorized("Ongeldige user id in token.");
            }

            long patientNumber;
            try
            {
                patientNumber = await _patientIdentity.GetPatientNumberForUserAsync(userId, cancellationToken);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            var records = await _repo.GetByPatientNumberAsync(patientNumber);

            return Ok(MedicalRecordResponseMapper.MapMany(records));
        }
    }
}