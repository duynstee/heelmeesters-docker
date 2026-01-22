using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.Shared.Appointments.Dtos;

namespace HeelmeestersAPI.Features.Shared.Appointments
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetForDoctorInRangeAsync(long employeeNumber, DateTime start, DateTime end);

        Task<List<Appointment>> GetAllForPatientAsync(long patientNumber);

        Task<Appointment?> GetByIdAsync(int id);

        Task AddAsync(Appointment appointment);

        // Voor cancel gaan we straks óf delete doen óf "soft cancel" (maar DB heeft geen status).
        // Voor nu laten we UpdateAsync staan; later beslissen we wat cancel doet.
        Task UpdateAsync(Appointment appointment);

        Task<bool> IsDoctorAvailable(long employeeNumber, DateTime start, DateTime end);

        // Treatments ophalen uit DB
        Task<List<Treatment>> GetTreatmentsAsync();
        Task DeleteAsync(int appointmentId);
        
        Task<List<DoctorOptionDto>> GetDoctorOptionsAsync();
        Task<List<PatientOptionDto>> GetPatientOptionsAsync();
        Task<List<RoomOptionDto>> GetRoomOptionsAsync();
        
        Task<long?> GetPatientNumberForUserAsync(int userId);

        // referral lookup/update
        Task<(string CareCode, bool IsUsed)> GetReferralForPatientAsync(int referralId, long patientNumber);
        Task MarkReferralUsedAsync(int referralId, DateTime usedOnUtc);
        Task<List<HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs.ReferralDto>>
            GetActiveReferralsForPatientAsync(long patientNumber);
    }
}