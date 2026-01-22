namespace HeelmeestersAPI.Features.AdminPortal.Users.Models;

public class CreateHospitalStaffAccountDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    public long EmployeeNumber { get; set; }
    public string FirstName { get; set; } = "";
    public string? Prefix { get; set; }
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }

    public string Specialization { get; set; } = "";
}