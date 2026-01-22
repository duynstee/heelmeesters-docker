namespace HeelmeestersAPI.Features.Shared.Auth.Models;

public class HospitalStaff
{
    public long EmployeeNumber { get; set; }

    public string FirstName { get; set; } = "";
    public string? Prefix { get; set; }
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }  

    public string Specialization { get; set; } = "";

    public int UserId { get; set; }
}