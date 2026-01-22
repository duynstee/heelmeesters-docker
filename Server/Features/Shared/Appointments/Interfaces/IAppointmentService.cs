using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.Shared.Appointments.Dtos;

namespace HeelmeestersAPI.Features.Shared.Appointments
{
    public interface IAppointmentService
    {
        // PatientNumber is bigint in DB -> long
        Task<List<Appointment>> GetAppointmentsForPatientAsync(long patientNumber);

        // Doctor is hospital_staff.employee_number (bigint) -> long
        Task<List<Appointment>> GetAppointmentsForDoctorInRangeAsync(long employeeNumber, DateTime start, DateTime end);

        // Create: DB heeft care_code + room_code + start/end
        Task<Appointment> CreateDoctorAppointmentAsync(
            long employeeNumber,
            long patientNumber,
            string careCode,
            string roomCode,
            DateTime startTime,
            DateTime endTime
        );

        Task CancelAppointmentAsync(int appointmentId);

        // AppointmentTypes -> Treatments
        Task<List<Treatment>> GetTreatmentsAsync();
        
        Task<List<DoctorOptionDto>> GetDoctorOptionsAsync();
        Task<List<PatientOptionDto>> GetPatientOptionsAsync();
        Task<List<RoomOptionDto>> GetRoomOptionsAsync();
        
        Task<long> GetPatientNumberForUserAsync(int userId);

        Task<Appointment> CreatePatientAppointmentFromReferralAsync(
            int userId,
            int referralId,
            long employeeNumber,
            string roomCode,
            DateTime startTime,
            DateTime endTime
        );
        Task<List<HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs.ReferralDto>>
            GetActiveReferralsForLoggedInPatientAsync(int userId);
    }
}