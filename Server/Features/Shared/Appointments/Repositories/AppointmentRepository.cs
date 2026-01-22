using HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs;
using HeelmeestersAPI.Features.Shared.Appointments.Dtos;
using HeelmeestersAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HeelmeestersAPI.Features.Shared.Appointments.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetForDoctorInRangeAsync(long employeeNumber, DateTime start, DateTime end)
        {
            // eerst join table filteren, dan appointments ophalen
            var appointmentIds = await _context.AppointmentHospitalStaff
                .Where(j => j.EmployeeNumber == employeeNumber)
                .Select(j => j.AppointmentId)
                .ToListAsync();

            var result = await _context.Appointments
                .Where(a => appointmentIds.Contains(a.Id) && a.StartTime >= start && a.StartTime < end)
                .OrderBy(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync();

            // EmployeeNumber vullen (handig voor service/controller)
            foreach (var a in result)
                a.EmployeeNumber = employeeNumber;

            return result;
        }

        public async Task<List<Appointment>> GetAllForPatientAsync(long patientNumber)
        {
            return await _context.Appointments
                .Where(a => a.PatientNumber == patientNumber)
                .OrderBy(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Appointment appointment)
        {
            // Zelf id bepalen (geen identity)
            appointment.Id = await GetNextAppointmentIdAsync();

            // 1) Insert in appointments
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // 2) Koppeling dokter/staff in join table
            _context.AppointmentHospitalStaff.Add(new AppointmentHospitalStaff
            {
                AppointmentId = appointment.Id,
                EmployeeNumber = appointment.EmployeeNumber
            });

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsDoctorAvailable(long employeeNumber, DateTime start, DateTime end)
        {
            // overlap check: alle afspraken van deze employee in join table,
            // en dan overlap met start/end
            var overlapping = await (
                from j in _context.AppointmentHospitalStaff
                join a in _context.Appointments on j.AppointmentId equals a.Id
                where j.EmployeeNumber == employeeNumber
                      && a.StartTime < end
                      && a.EndTime > start
                select a.Id
            ).AnyAsync();

            return !overlapping;
        }

        public async Task<List<Treatment>> GetTreatmentsAsync()
        {
            return await _context.Treatments
                .OrderBy(t => t.CareCode)
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task DeleteAsync(int appointmentId)
        {
            // 1) Eerst join table records verwijderen (FK naar appointments)
            var links = await _context.AppointmentHospitalStaff
                .Where(j => j.AppointmentId == appointmentId)
                .ToListAsync();

            if (links.Count > 0)
                _context.AppointmentHospitalStaff.RemoveRange(links);

            // 2) Daarna appointment zelf verwijderen
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment != null)
                _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();
        }
        
        public async Task<List<DoctorOptionDto>> GetDoctorOptionsAsync()
        {
            var rows = await _context.HospitalStaff
                .AsNoTracking()
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .Select(d => new
                {
                    d.EmployeeNumber,
                    d.FirstName,
                    d.Prefix,
                    d.LastName,
                    d.Specialization
                })
                .ToListAsync();

            return rows.Select(d => new DoctorOptionDto
            {
                EmployeeNumber = d.EmployeeNumber,
                Name = $"{d.FirstName} {(string.IsNullOrWhiteSpace(d.Prefix) ? "" : d.Prefix + " ")}{d.LastName}".Trim(),
                Specialization = d.Specialization
            }).ToList();
        }

        public async Task<List<PatientOptionDto>> GetPatientOptionsAsync()
        {
            var rows = await _context.Patients
                .AsNoTracking()
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new
                {
                    p.PatientNumber,
                    p.FirstName,
                    p.Prefix,
                    p.LastName
                })
                .ToListAsync();

            return rows.Select(p => new PatientOptionDto
            {
                PatientNumber = p.PatientNumber,
                Name = $"{p.FirstName} {(string.IsNullOrWhiteSpace(p.Prefix) ? "" : p.Prefix + " ")}{p.LastName}".Trim()
            }).ToList();
        }

        public async Task<List<RoomOptionDto>> GetRoomOptionsAsync()
        {
            return await _context.Rooms
                .AsNoTracking()
                .OrderBy(r => r.RoomCode)
                .Select(r => new RoomOptionDto
                {
                    RoomCode = r.RoomCode
                })
                .ToListAsync();
        }
        
        private async Task<int> GetNextAppointmentIdAsync()
        {
            var maxId = await _context.Appointments.MaxAsync(a => (int?)a.Id) ?? 0;
            return maxId + 1;
        }
        
        public async Task<long?> GetPatientNumberForUserAsync(int userId)
        {
            return await _context.Patients
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => (long?)p.PatientNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<(string CareCode, bool IsUsed)> GetReferralForPatientAsync(int referralId, long patientNumber)
        {
            // referral moet bij patiënt horen
            var row = await _context.Referrals
                .AsNoTracking()
                .Where(r => r.Id == referralId && r.PatientNumber == patientNumber)
                .Select(r => new { r.CareCode, r.IsUsed })
                .FirstOrDefaultAsync();

            if (row == null)
                throw new KeyNotFoundException("Doorverwijzing niet gevonden voor deze patiënt.");

            return (row.CareCode, row.IsUsed);
        }

        public async Task MarkReferralUsedAsync(int referralId, DateTime usedOnUtc)
        {
            var referral = await _context.Referrals.FirstOrDefaultAsync(r => r.Id == referralId);
            if (referral == null)
                throw new KeyNotFoundException("Doorverwijzing niet gevonden.");

            referral.IsUsed = true;
            referral.UsedOn = usedOnUtc;

            await _context.SaveChangesAsync();
        }

        public async Task<List<ReferralDto>> GetActiveReferralsForPatientAsync(long patientNumber)
        {
            var query =
                from r in _context.Referrals.AsNoTracking()
                join t in _context.Treatments.AsNoTracking() on r.CareCode equals t.CareCode
                where r.PatientNumber == patientNumber
                      && r.IsUsed == false
                orderby r.Id descending
                select new ReferralDto
                {
                    Id = r.Id,
                    PatientNumber = r.PatientNumber,
                    CareCode = r.CareCode,
                    TreatmentDescription = t.Description,
                    IsUsed = r.IsUsed,
                    UsedOn = r.UsedOn
                };

            return await query.ToListAsync();
        }
    }
}