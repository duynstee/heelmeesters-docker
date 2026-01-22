using HeelmeestersAPI.Features.Shared.Appointments;
using System;

namespace HeelmeestersAPI.Features.PatientPortal.Appointments
{
    public class PatientAppointmentDto
    {
        public int Id { get; set; }
        public string CareCode { get; set; } = "";
        public string RoomCode { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}