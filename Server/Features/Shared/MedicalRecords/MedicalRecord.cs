namespace HeelmeestersAPI.Features.Shared.MedicalRecords
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        
        public byte[] Document { get; set; } = Array.Empty<byte>();

        public int DoctorId { get; set; } 

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Diagnoses { get; set; } = "";
        public string Medications { get; set; } = "";
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}