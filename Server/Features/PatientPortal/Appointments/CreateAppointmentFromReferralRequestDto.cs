namespace HeelmeestersAPI.Features.PatientPortal.Appointments;

public class CreateAppointmentFromReferralRequestDto
{
    public int ReferralId { get; set; }
    public long EmployeeNumber { get; set; }
    public string RoomCode { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}