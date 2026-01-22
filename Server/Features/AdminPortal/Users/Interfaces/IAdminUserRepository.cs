using HeelmeestersAPI.Features.AdminPortal.Users.DTOs;
using HeelmeestersAPI.Features.AdminPortal.Users.Models;

namespace HeelmeestersAPI.Features.AdminPortal.Users.Interfaces;

public interface IAdminUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<int> GetNextUserIdAsync();

    Task<bool> PatientNumberExistsAsync(long patientNumber);
    Task<bool> EmployeeNumberExistsAsync(long employeeNumber);
    Task<bool> GeneralPractitionerIdExistsAsync(int id);

    Task CreatePatientAccountAsync(CreatePatientAccountDto dto, int newUserId);
    Task CreateGeneralPractitionerAccountAsync(CreateGeneralPractitionerAccountDto dto, int newUserId);
    Task CreateHospitalStaffAccountAsync(CreateHospitalStaffAccountDto dto, int newUserId);
    Task<List<UserListItemDto>> GetUsersAsync();
    Task<List<PatientListItemDto>> GetPatientsAsync();
    Task<List<GeneralPractitionerListItemDto>> GetGeneralPractitionersAsync();
    Task<List<HospitalStaffListItemDto>> GetHospitalStaffAsync();
}