using Microsoft.EntityFrameworkCore;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Interfaces;
using HeelmeestersAPI.Infrastructure;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Models;

namespace HeelmeestersAPI.Features.HuisartsPortal.Referral.Repositories;

public class ReferralRepository : IReferralRepository
{
    private readonly AppDbContext _context;

    public ReferralRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReferralDto>> GetForSelectedPatientAsync(int loggedInUserId, long patientNumber)
    {
        var gpId = await _context.GeneralPractitioners
            .AsNoTracking()
            .Where(gp => gp.UserId == loggedInUserId)
            .Select(gp => gp.Id)
            .FirstOrDefaultAsync();

        if (gpId == 0)
            throw new UnauthorizedAccessException("Ingelogde gebruiker is geen huisarts.");

        var isLinked = await _context.GeneralPractitionerHasPatients
            .AsNoTracking()
            .AnyAsync(x => x.GeneralPractitionerId == gpId && x.PatientNumber == patientNumber);

        if (!isLinked)
            throw new UnauthorizedAccessException("Deze patiënt hoort niet bij jouw huisarts-account.");

        // ✅ alleen actieve referrals tonen (is_used = 0)
        var query =
            from r in _context.Referrals.AsNoTracking()
            join t in _context.Treatments.AsNoTracking() on r.CareCode equals t.CareCode
            where r.GeneralPractitionerId == gpId
               && r.PatientNumber == patientNumber
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

    public async Task<ReferralDto> CreateAsync(int loggedInUserId, long patientNumber, CreateReferralRequestDto request)
    {
        if (patientNumber <= 0)
            throw new ArgumentException("Ongeldig patientNumber.");

        if (string.IsNullOrWhiteSpace(request.CareCode))
            throw new ArgumentException("CareCode is verplicht.");

        var careCode = request.CareCode.Trim();

        // 1) userId -> gpId
        var gpId = await _context.GeneralPractitioners
            .AsNoTracking()
            .Where(gp => gp.UserId == loggedInUserId)
            .Select(gp => gp.Id)
            .FirstOrDefaultAsync();

        if (gpId == 0)
            throw new UnauthorizedAccessException("Ingelogde gebruiker is geen huisarts.");

        // 2) check patient hoort bij gp
        var isLinked = await _context.GeneralPractitionerHasPatients
            .AsNoTracking()
            .AnyAsync(x => x.GeneralPractitionerId == gpId && x.PatientNumber == patientNumber);

        if (!isLinked)
            throw new UnauthorizedAccessException("Deze patiënt hoort niet bij jouw huisarts-account.");

        // 3) treatment exists + pak description
        var treatment = await _context.Treatments
            .AsNoTracking()
            .Where(t => t.CareCode == careCode)
            .Select(t => new { t.CareCode, t.Description })
            .FirstOrDefaultAsync();

        if (treatment == null)
            throw new ArgumentException("Onbekende careCode.");

        // ✅ REGEL: meerdere referrals voor dezelfde careCode+patient mogen,
        // maar NIET meerdere ACTIEVE tegelijk.
        var activeExists = await _context.Referrals
            .AsNoTracking()
            .AnyAsync(r =>
                r.GeneralPractitionerId == gpId &&
                r.PatientNumber == patientNumber &&
                r.CareCode == careCode &&
                r.IsUsed == false);

        if (activeExists)
            throw new InvalidOperationException("Er bestaat al een actieve doorverwijzing voor deze behandeling en patiënt.");

        // 5) handmatige id (MAX+1)
        var maxId = await _context.Referrals.MaxAsync(r => (int?)r.Id) ?? 0;
        var newId = maxId + 1;

        var entity = new Models.Referral
        {
            Id = newId,
            GeneralPractitionerId = gpId,
            PatientNumber = patientNumber,
            CareCode = careCode,
            IsUsed = false,
            UsedOn = null
        };

        _context.Referrals.Add(entity);
        await _context.SaveChangesAsync();

        return new ReferralDto
        {
            Id = entity.Id,
            PatientNumber = entity.PatientNumber,
            CareCode = entity.CareCode,
            TreatmentDescription = treatment.Description,
            IsUsed = entity.IsUsed,
            UsedOn = entity.UsedOn
        };
    }
}