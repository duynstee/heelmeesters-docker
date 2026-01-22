using HeelmeestersAPI.Features.AdminPortal.Users.DTOs;
using HeelmeestersAPI.Features.AdminPortal.Users.Interfaces;
using HeelmeestersAPI.Features.AdminPortal.Users.Models;
using HeelmeestersAPI.Features.Shared.Auth.Models;
using HeelmeestersAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HeelmeestersAPI.Features.AdminPortal.Users.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AppDbContext _db;

    public AdminUserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return _db.Users.AnyAsync(u => u.MailAdress == email);
    }

    public async Task<int> GetNextUserIdAsync()
    {
        var max = await _db.Users.MaxAsync(u => (int?)u.Id) ?? 0;
        return max + 1;
    }

    public Task<bool> PatientNumberExistsAsync(long patientNumber)
        => _db.Patients.AnyAsync(p => p.PatientNumber == patientNumber);

    public Task<bool> EmployeeNumberExistsAsync(long employeeNumber)
        => _db.HospitalStaff.AnyAsync(h => h.EmployeeNumber == employeeNumber);

    public Task<bool> GeneralPractitionerIdExistsAsync(int id)
        => _db.GeneralPractitioners.AnyAsync(g => g.Id == id);

    public async Task CreatePatientAccountAsync(CreatePatientAccountDto dto, int newUserId)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        _db.Users.Add(new User
        {
            Id = newUserId,
            MailAdress = dto.Email,
            Password = dto.Password
        });
        await _db.SaveChangesAsync();

        _db.Patients.Add(new Patient
        {
            PatientNumber = dto.PatientNumber,
            FirstName = dto.FirstName,
            Prefix = dto.Prefix,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            UserId = newUserId
        });
        await _db.SaveChangesAsync();

        await tx.CommitAsync();
    }

    public async Task CreateGeneralPractitionerAccountAsync(CreateGeneralPractitionerAccountDto dto, int newUserId)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        _db.Users.Add(new User
        {
            Id = newUserId,
            MailAdress = dto.Email,
            Password = dto.Password
        });
        await _db.SaveChangesAsync();

        _db.GeneralPractitioners.Add(new GeneralPractitioner
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            Prefix = dto.Prefix,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            UserId = newUserId
        });
        await _db.SaveChangesAsync();

        await tx.CommitAsync();
    }

    public async Task CreateHospitalStaffAccountAsync(CreateHospitalStaffAccountDto dto, int newUserId)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        _db.Users.Add(new User
        {
            Id = newUserId,
            MailAdress = dto.Email,
            Password = dto.Password
        });
        await _db.SaveChangesAsync();

        _db.HospitalStaff.Add(new HospitalStaff
        {
            EmployeeNumber = dto.EmployeeNumber,
            FirstName = dto.FirstName,
            Prefix = dto.Prefix,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            UserId = newUserId,
            Specialization = dto.Specialization
        });
        await _db.SaveChangesAsync();

        await tx.CommitAsync();
    }
    
    public async Task<List<UserListItemDto>> GetUsersAsync()
{
    return await _db.Users
        .AsNoTracking()
        .OrderBy(u => u.Id)
        .Select(u => new UserListItemDto
        {
            Id = u.Id,
            Email = u.MailAdress
        })
        .ToListAsync();
}

public async Task<List<PatientListItemDto>> GetPatientsAsync()
{
    return await (
        from p in _db.Patients.AsNoTracking()
        join u in _db.Users.AsNoTracking() on p.UserId equals u.Id
        orderby p.LastName, p.FirstName
        select new PatientListItemDto
        {
            PatientNumber = p.PatientNumber,
            Name = (p.FirstName + " " + (p.Prefix ?? "") + " " + p.LastName).Replace("  ", " ").Trim(),
            DateOfBirth = p.DateOfBirth,
            UserId = p.UserId,
            Email = u.MailAdress
        }
    ).ToListAsync();
}

public async Task<List<GeneralPractitionerListItemDto>> GetGeneralPractitionersAsync()
{
    return await (
        from g in _db.GeneralPractitioners.AsNoTracking()
        join u in _db.Users.AsNoTracking() on g.UserId equals u.Id
        orderby g.LastName, g.FirstName
        select new GeneralPractitionerListItemDto
        {
            Id = g.Id,
            Name = (g.FirstName + " " + (g.Prefix ?? "") + " " + g.LastName).Replace("  ", " ").Trim(),
            DateOfBirth = g.DateOfBirth,
            UserId = g.UserId,
            Email = u.MailAdress
        }
    ).ToListAsync();
}

public async Task<List<HospitalStaffListItemDto>> GetHospitalStaffAsync()
{
    return await (
        from h in _db.HospitalStaff.AsNoTracking()
        join u in _db.Users.AsNoTracking() on h.UserId equals u.Id
        orderby h.Specialization, h.LastName, h.FirstName
        select new HospitalStaffListItemDto
        {
            EmployeeNumber = h.EmployeeNumber,
            Name = (h.FirstName + " " + (h.Prefix ?? "") + " " + h.LastName).Replace("  ", " ").Trim(),
            Specialization = h.Specialization,
            DateOfBirth = h.DateOfBirth,
            UserId = h.UserId,
            Email = u.MailAdress
        }
    ).ToListAsync();
}
}