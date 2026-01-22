namespace HeelmeestersAPI.Features.Shared.Auth.Models;

public class Patient 
{
    public long PatientNumber { get; set; }

    public string FirstName { get; set; } = "";
    public string? Prefix { get; set; }
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }

    public int UserId { get; set; }
}