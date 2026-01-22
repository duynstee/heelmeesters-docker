using HeelmeestersAPI.Features.Shared.Auth.Interfaces;
using HeelmeestersAPI.Features.Shared.Auth.Models;
using HeelmeestersAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HeelmeestersAPI.Features.Shared.Auth.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext dbContext)
    {
        _context = dbContext;
    }
    
    public User? GetByEmail(string email)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.MailAdress == email);
    }
    
    public string GetUserRole(int userId)
    {
        if (_context.Admins.Any(a => a.UserId == userId))
            return "Admin";
        
        if (_context.HospitalStaff.Any(h => h.UserId == userId))
            return "Doctor";

        if (_context.Patients.Any(p => p.UserId == userId))
            return "Patient";

        if (_context.GeneralPractitioners.Any(gp => gp.UserId == userId))
            return "GeneralPractitioner"; 
    
        return "Unknown";
    }
}