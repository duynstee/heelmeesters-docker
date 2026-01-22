using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
        public async Task<IActionResult> GetAll()
        {
            var records = await _repo.GetAllAsync();

            var dtos = records.Select(r => new DoctorMedicalRecordDto
            {
                Id = r.Id,
                PatientId = r.PatientId,
                PatientName = $"Patient {r.PatientId}", // demo
                Notes = r.Notes,
                Diagnoses = r.Diagnoses,
                Medications = r.Medications,
                CreatedAt = r.CreatedAt,
                Title = r.Title,
                DocumentFileName = string.IsNullOrWhiteSpace(r.Title) ? "document.pdf" : r.Title + ".pdf",
                // Provide a data URL so frontend can open/download immediately without a second API call
                DocumentDownloadHref = (r.Document != null && r.Document.Length > 0)
                    ? ("data:application/pdf;base64," + Convert.ToBase64String(r.Document))
                    : string.Empty
            }).ToList();

            return Ok(dtos);
        }

        // Keep the PDF endpoint as an alternative, but frontend can use the data URL instead of calling this
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetPdf(int id)
        {
            var record = await _repo.GetByIdAsync(id);
            if (record == null) return NotFound();

            if (record.Document == null || record.Document.Length == 0)
            {
                return NoContent();
            }

            var fileName = string.IsNullOrWhiteSpace(record.Title) ? "document.pdf" : record.Title + ".pdf";
            return File(record.Document, "application/pdf", fileName);
        }
    }
}