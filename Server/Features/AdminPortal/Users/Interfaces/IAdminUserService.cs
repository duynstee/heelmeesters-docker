using HeelmeestersAPI.Features.AdminPortal.Users.DTOs;
using HeelmeestersAPI.Features.AdminPortal.Users.Models;

namespace HeelmeestersAPI.Features.AdminPortal.Users.Interfaces;

public interface IAdminUserService
{
    Task CreatePatientAccountAsync(CreatePatientAccountDto dto);
    Task CreateGeneralPractitionerAccountAsync(CreateGeneralPractitionerAccountDto dto);
    Task CreateHospitalStaffAccountAsync(CreateHospitalStaffAccountDto dto);
    Task<List<UserListItemDto>> GetUsersAsync();
    Task<List<PatientListItemDto>> GetPatientsAsync();
    Task<List<GeneralPractitionerListItemDto>> GetGeneralPractitionersAsync();
    Task<List<HospitalStaffListItemDto>> GetHospitalStaffAsync();
}