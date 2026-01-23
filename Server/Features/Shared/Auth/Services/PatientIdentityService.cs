using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.Shared.Auth.Interfaces;
using HeelmeestersAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HeelmeestersAPI.Features.Shared.Auth.Services
{
    public class PatientIdentityService : IPatientIdentityService
    {
        private readonly AppDbContext _context;

        public PatientIdentityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> GetPatientNumberForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var patientNumber = await _context.Patients
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => (long?)p.PatientNumber)
                .FirstOrDefaultAsync(cancellationToken);

            if (!patientNumber.HasValue)
            {
                throw new UnauthorizedAccessException("Ingelogde gebruiker is geen patiënt.");
            }

            return patientNumber.Value;
        }
    }
}
