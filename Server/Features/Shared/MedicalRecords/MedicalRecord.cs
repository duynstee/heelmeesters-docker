namespace HeelmeestersAPI.Features.Shared.MedicalRecords
{
    public class MedicalRecord
    {
        public int LineNumber { get; set; }
        public long PatientNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; } = "";
        public byte[]? File { get; set; }
    }
}