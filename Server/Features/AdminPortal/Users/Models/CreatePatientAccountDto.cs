namespace HeelmeestersAPI.Features.AdminPortal.Users.Models;

public class CreatePatientAccountDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    public long PatientNumber { get; set; }
    public string FirstName { get; set; } = "";
    public string? Prefix { get; set; }
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
}
