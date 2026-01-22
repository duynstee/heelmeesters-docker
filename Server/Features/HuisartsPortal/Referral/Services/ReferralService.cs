using HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Interfaces;

namespace HeelmeestersAPI.Features.HuisartsPortal.Referral.Services;

public class ReferralService : IReferralService
{
    private readonly IReferralRepository _repo;

    public ReferralService(IReferralRepository repo)
    {
        _repo = repo;
    }

    public Task<List<ReferralDto>> GetForSelectedPatientAsync(int loggedInUserId, long patientNumber)
        => _repo.GetForSelectedPatientAsync(loggedInUserId, patientNumber);

    public Task<ReferralDto> CreateAsync(int loggedInUserId, long patientNumber, CreateReferralRequestDto request)
        => _repo.CreateAsync(loggedInUserId, patientNumber, request);
}
