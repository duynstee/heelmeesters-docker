using HeelmeestersAPI.Features.HuisartsPortal.Patient.DTOs;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.Interfaces;

namespace HeelmeestersAPI.Features.HuisartsPortal.Patient.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repo;

    public PatientService(IPatientRepository repo)
    {
        _repo = repo;
    }

    public Task<List<PatientListItemDto>> GetMyPatientsAsync(int loggedInUserId, string? search, int take)
    {
        return _repo.GetMyPatientsAsync(loggedInUserId, search, take);
    }
}