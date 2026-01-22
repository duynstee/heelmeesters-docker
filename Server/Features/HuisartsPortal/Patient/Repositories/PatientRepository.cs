using Microsoft.EntityFrameworkCore;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.DTOs;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.Interfaces;
using HeelmeestersAPI.Infrastructure;

namespace HeelmeestersAPI.Features.HuisartsPortal.Patient.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientListItemDto>> GetMyPatientsAsync(int loggedInUserId, string? search, int take)
    {
        take = Math.Clamp(take, 1, 200);
        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        // 1) userId -> gpId (data-toegang, dus repo)
        var gpId = await _context.GeneralPractitioners
            .AsNoTracking()
            .Where(gp => gp.UserId == loggedInUserId)
            .Select(gp => gp.Id)
            .FirstOrDefaultAsync();

        if (gpId == 0)
            return new List<PatientListItemDto>(); // of throw; ik laat service/controller dit afhandelen

        // 2) gpId -> patients via koppel tabel
        var query =
            from link in _context.GeneralPractitionerHasPatients.AsNoTracking()
            join p in _context.Patients.AsNoTracking() on link.PatientNumber equals p.PatientNumber
            where link.GeneralPractitionerId == gpId
            select p;

        if (search != null)
        {
            if (long.TryParse(search, out var pn))
            {
                query = query.Where(p => p.PatientNumber == pn);
            }
            else
            {
                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    (p.Prefix != null && p.Prefix.Contains(search)) ||
                    p.LastName.Contains(search));
            }
        }

        return await query
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Take(take)
            .Select(p => new PatientListItemDto
            {
                PatientNumber = p.PatientNumber,
                FirstName = p.FirstName,
                Prefix = p.Prefix,
                LastName = p.LastName
            })
            .ToListAsync();
    }
}
