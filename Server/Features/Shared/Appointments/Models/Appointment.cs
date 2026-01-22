namespace HeelmeestersAPI.Features.Shared.Appointments
{
    public class Appointment
    {
        public int Id { get; set; }

        // DB
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public long PatientNumber { get; set; }     // patient_number (bigint)
        public string CareCode { get; set; } = "";  // care_code (varchar(5))
        public string RoomCode { get; set; } = "";  // room_code (varchar(10))

        // Niet in appointments tabel, maar nodig voor "doctor schedule"
        public long EmployeeNumber { get; set; }    // via appointment_has_hospital_staff
    }
}