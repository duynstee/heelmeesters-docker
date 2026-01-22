using HeelmeestersAPI.Features.HuisartsPortal.Patient.DTOs;

namespace HeelmeestersAPI.Features.HuisartsPortal.Patient.Interfaces;

public interface IPatientService
{
    Task<List<PatientListItemDto>> GetMyPatientsAsync(int loggedInUserId, string? search, int take);
}