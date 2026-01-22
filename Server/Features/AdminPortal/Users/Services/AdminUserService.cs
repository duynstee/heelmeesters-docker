using HeelmeestersAPI.Features.AdminPortal.Users.DTOs;
using HeelmeestersAPI.Features.AdminPortal.Users.Interfaces;
using HeelmeestersAPI.Features.AdminPortal.Users.Models;

namespace HeelmeestersAPI.Features.AdminPortal.Users.Services;

public class AdminUserService : IAdminUserService
{
    private readonly IAdminUserRepository _repo;

    public AdminUserService(IAdminUserRepository repo)
    {
        _repo = repo;
    }

    public async Task CreatePatientAccountAsync(CreatePatientAccountDto dto)
    {
        Normalize(dto);

        if (await _repo.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email bestaat al.");

        if (await _repo.PatientNumberExistsAsync(dto.PatientNumber))
            throw new InvalidOperationException("PatientNumber bestaat al.");

        var newUserId = await _repo.GetNextUserIdAsync();
        await _repo.CreatePatientAccountAsync(dto, newUserId);
    }

    public async Task CreateGeneralPractitionerAccountAsync(CreateGeneralPractitionerAccountDto dto)
    {
        Normalize(dto);

        if (await _repo.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email bestaat al.");

        if (await _repo.GeneralPractitionerIdExistsAsync(dto.Id))
            throw new InvalidOperationException("Huisarts id bestaat al.");

        var newUserId = await _repo.GetNextUserIdAsync();
        await _repo.CreateGeneralPractitionerAccountAsync(dto, newUserId);
    }

    public async Task CreateHospitalStaffAccountAsync(CreateHospitalStaffAccountDto dto)
    {
        Normalize(dto);

        if (await _repo.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email bestaat al.");

        if (await _repo.EmployeeNumberExistsAsync(dto.EmployeeNumber))
            throw new InvalidOperationException("EmployeeNumber bestaat al.");

        if (string.IsNullOrWhiteSpace(dto.Specialization))
            throw new InvalidOperationException("Specialization is verplicht.");

        var newUserId = await _repo.GetNextUserIdAsync();
        await _repo.CreateHospitalStaffAccountAsync(dto, newUserId);
    }

    public Task<List<UserListItemDto>> GetUsersAsync()
        => _repo.GetUsersAsync();

    public Task<List<PatientListItemDto>> GetPatientsAsync()
        => _repo.GetPatientsAsync();

    public Task<List<GeneralPractitionerListItemDto>> GetGeneralPractitionersAsync()
        => _repo.GetGeneralPractitionersAsync();

    public Task<List<HospitalStaffListItemDto>> GetHospitalStaffAsync()
        => _repo.GetHospitalStaffAsync();
    
    private static void Normalize(CreatePatientAccountDto dto)
    {
        dto.Email = dto.Email.Trim().ToLowerInvariant();
        dto.Password = dto.Password.Trim();
        dto.FirstName = dto.FirstName.Trim();
        dto.Prefix = string.IsNullOrWhiteSpace(dto.Prefix) ? null : dto.Prefix.Trim();
        dto.LastName = dto.LastName.Trim();
    }

    private static void Normalize(CreateGeneralPractitionerAccountDto dto)
    {
        dto.Email = dto.Email.Trim().ToLowerInvariant();
        dto.Password = dto.Password.Trim();
        dto.FirstName = dto.FirstName.Trim();
        dto.Prefix = string.IsNullOrWhiteSpace(dto.Prefix) ? null : dto.Prefix.Trim();
        dto.LastName = dto.LastName.Trim();
    }

    private static void Normalize(CreateHospitalStaffAccountDto dto)
    {
        dto.Email = dto.Email.Trim().ToLowerInvariant();
        dto.Password = dto.Password.Trim();
        dto.FirstName = dto.FirstName.Trim();
        dto.Prefix = string.IsNullOrWhiteSpace(dto.Prefix) ? null : dto.Prefix.Trim();
        dto.LastName = dto.LastName.Trim();
        dto.Specialization = dto.Specialization.Trim();
    }
    
    
}