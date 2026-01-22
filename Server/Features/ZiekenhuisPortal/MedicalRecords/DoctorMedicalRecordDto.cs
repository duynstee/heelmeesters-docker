using System;

namespace HeelmeestersAPI.Features.ZiekenhuisPortal.MedicalRecords
{
    public class DoctorMedicalRecordDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Diagnoses { get; set; } = "";
        public string Medications { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        // New fields to support PDF delivery for the frontend
        public string Title { get; set; } = "";
        public string DocumentFileName { get; set; } = "";
        // Provide a downloadable href that contains the PDF as a data URL (no second API call required)
        public string DocumentDownloadHref { get; set; } = ""; // e.g. data:application/pdf;base64,...
    }
}