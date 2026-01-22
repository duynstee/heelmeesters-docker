using HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs;

namespace HeelmeestersAPI.Features.HuisartsPortal.Referral.Interfaces;

public interface IReferralRepository
{
    Task<List<ReferralDto>> GetForSelectedPatientAsync(int loggedInUserId, long patientNumber);
    Task<ReferralDto> CreateAsync(int loggedInUserId, long patientNumber, CreateReferralRequestDto request);
}