using HeelmeestersAPI.Features.Shared.Appointments;
using System;

namespace HeelmeestersAPI.Features.ZiekenhuisPortal.Appointments
{
    public class DoctorAppointmentDto
    {
        public int Id { get; set; }
        public long PatientNumber { get; set; }

        public string CareCode { get; set; } = "";
        public string RoomCode { get; set; } = "";

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}