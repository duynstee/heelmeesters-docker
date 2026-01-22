namespace HeelmeestersAPI.Features.ZiekenhuisPortal.Appointments;

public class CreateDoctorAppointmentDto
{
    public long PatientNumber { get; set; }
    public string CareCode { get; set; } = "";
    public string RoomCode { get; set; } = "";
    public DateTime StartTime { get; set; }
    public long EmployeeNumber { get; set; }
}