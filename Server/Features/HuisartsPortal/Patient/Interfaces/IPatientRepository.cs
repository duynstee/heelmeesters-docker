using HeelmeestersAPI.Features.HuisartsPortal.Patient.DTOs;

namespace HeelmeestersAPI.Features.HuisartsPortal.Patient.Interfaces;

public interface IPatientRepository
{
    Task<List<PatientListItemDto>> GetMyPatientsAsync(int loggedInUserId, string? search, int take);
}