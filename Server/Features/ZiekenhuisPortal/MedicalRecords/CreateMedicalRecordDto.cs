namespace HeelmeestersAPI.Features.ZiekenhuisPortal.MedicalRecords
{
    public class CreateMedicalRecordDto
    {
        public long PatientNumber { get; set; }
        public string Description { get; set; } = "";
        public byte[]? File { get; set; }
    }
}