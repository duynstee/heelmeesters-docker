using HeelmeestersAPI.Features.Shared.Auth.Models;

namespace HeelmeestersAPI.Features.Shared.Auth.Interfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);
    string GetUserRole(int userId);
}