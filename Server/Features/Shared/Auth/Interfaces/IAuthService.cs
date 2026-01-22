namespace HeelmeestersAPI.Features.Shared.Auth.Interfaces;

public interface IAuthService
{
    string? Authenticate(string username, string password);
    
}