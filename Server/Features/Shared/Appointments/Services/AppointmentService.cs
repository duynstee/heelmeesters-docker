using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs;
using HeelmeestersAPI.Features.Shared.Appointments;
using HeelmeestersAPI.Features.Shared.Appointments.Dtos;
using HeelmeestersAPI.Features.Shared.Appointments.Interfaces;

namespace HeelmeestersAPI.Features.Shared.Appointments.Services
{
    public class AppointmentService : IAppointmentService, ITreatmentProvider
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<Appointment> CreateDoctorAppointmentAsync(
            long employeeNumber,
            long patientNumber,
            string careCode,
            string roomCode,
            DateTime startTime,
            DateTime endTime)
        {
            if (endTime <= startTime)
                throw new ArgumentException("EndTime must be after StartTime");

            var startTod = startTime.TimeOfDay;
            var endTod = endTime.TimeOfDay;

            var open = new TimeSpan(9, 0, 0);
            var close = new TimeSpan(17, 0, 0);

            if (startTod < open || endTod > close)
                throw new ArgumentException("Afspraken kunnen alleen tussen 09:00 en 17:00 worden gepland.");

            if (startTime.Date != endTime.Date)
                throw new ArgumentException("Afspraak moet op dezelfde dag beginnen en eindigen.");
            
            // Check beschikbaarheid dokter (overlap)
            bool available = await _repo.IsDoctorAvailable(employeeNumber, startTime, endTime);
            if (!available)
                throw new Exception("Doctor is not available");

            // Maak appointment (let op: status bestaat niet meer)
            var appointment = new Appointment
            {
                EmployeeNumber = employeeNumber,
                PatientNumber = patientNumber,
                CareCode = careCode,
                RoomCode = roomCode,
                StartTime = startTime,
                EndTime = endTime
            };

            await _repo.AddAsync(appointment);

            // repo hoort Id te vullen (na insert), als je dat implementeert
            return appointment;
        }

        public async Task CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Afspraak niet gevonden");

            if (appointment.StartTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Afspraak kan niet meer geannuleerd worden");

            await _repo.DeleteAsync(appointmentId);
        }

        public Task<List<Appointment>> GetAppointmentsForPatientAsync(long patientNumber)
        {
            
            
            return _repo.GetAllForPatientAsync(patientNumber);
        }

        public async Task<List<Appointment>> GetAppointmentsForDoctorInRangeAsync(long employeeNumber, DateTime start, DateTime end)
        {
            return await _repo.GetForDoctorInRangeAsync(employeeNumber, start, end);
        }

        public Task<List<Treatment>> GetTreatmentsAsync()
        {
            return _repo.GetTreatmentsAsync();
        }
        
        public Task<List<DoctorOptionDto>> GetDoctorOptionsAsync()
        {
            return _repo.GetDoctorOptionsAsync();
        }

        public Task<List<PatientOptionDto>> GetPatientOptionsAsync()
        {
            return _repo.GetPatientOptionsAsync();
        }

        public Task<List<RoomOptionDto>> GetRoomOptionsAsync()
        {
            return _repo.GetRoomOptionsAsync();
        }
        
        public async Task<long> GetPatientNumberForUserAsync(int userId)
        {
            var pn = await _repo.GetPatientNumberForUserAsync(userId);
            if (pn == null)
                throw new UnauthorizedAccessException("Ingelogde gebruiker is geen patiënt.");
            return pn.Value;
        }

        public async Task<Appointment> CreatePatientAppointmentFromReferralAsync(
            int userId,
            int referralId,
            long employeeNumber,
            string roomCode,
            DateTime startTime,
            DateTime endTime)
        {
            var patientNumber = await GetPatientNumberForUserAsync(userId);

            // 1) referral check + careCode ophalen
            var (careCode, isUsed) = await _repo.GetReferralForPatientAsync(referralId, patientNumber);

            if (isUsed)
                throw new InvalidOperationException("Deze doorverwijzing is al gebruikt.");

            // 2) maak afspraak met careCode uit referral
            var appointment = await CreateDoctorAppointmentAsync(
                employeeNumber,
                patientNumber,
                careCode,
                roomCode,
                startTime,
                endTime
            );

            // 3) mark referral used
            await _repo.MarkReferralUsedAsync(referralId, DateTime.UtcNow);

            return appointment;
        }
        
        public async Task<List<ReferralDto>> GetActiveReferralsForLoggedInPatientAsync(int userId)
        {
            var patientNumber = await GetPatientNumberForUserAsync(userId);
            return await _repo.GetActiveReferralsForPatientAsync(patientNumber);
        }
    }
}
