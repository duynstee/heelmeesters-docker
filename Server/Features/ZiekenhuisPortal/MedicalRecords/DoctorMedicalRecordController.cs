using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.Shared.MedicalRecords;

namespace HeelmeestersAPI.Features.ZiekenhuisPortal.MedicalRecords
{
    [ApiController]
    [Route("api/hospital/medicalrecords")]
    //[Authorize(Roles = "Doctor")]
    public class DoctorMedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordRepository _repo;

        public DoctorMedicalRecordController(IMedicalRecordRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var records = await _repo.GetAllAsync();

            return Ok(MedicalRecordResponseMapper.MapMany(records));
        }

        [HttpGet("patient/{patientNumber:long}")]
        public async Task<IActionResult> GetByPatient(long patientNumber, CancellationToken cancellationToken)
        {
            var records = await _repo.GetByPatientNumberAsync(patientNumber);
            return Ok(MedicalRecordResponseMapper.MapMany(records));
        }

        [HttpGet("{lineNumber:int}")]
        public async Task<IActionResult> GetByLineNumber(int lineNumber, CancellationToken cancellationToken)
        {
            var record = await _repo.GetByLineNumberAsync(lineNumber, cancellationToken);
            if (record == null)
            {
                return NotFound();
            }

            return Ok(MedicalRecordResponseMapper.Map(record));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest("Description is required.");
            }

            var record = new MedicalRecord
            {
                PatientNumber = dto.PatientNumber,
                Description = dto.Description,
                File = dto.File
            };

            await _repo.AddAsync(record, cancellationToken);

            var createdDto = MedicalRecordResponseMapper.Map(record);
            return CreatedAtAction(nameof(GetByLineNumber), new { lineNumber = record.LineNumber }, createdDto);
        }
    }
}