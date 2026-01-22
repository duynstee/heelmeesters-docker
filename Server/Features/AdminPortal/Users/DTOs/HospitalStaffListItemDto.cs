namespace HeelmeestersAPI.Features.AdminPortal.Users.DTOs;

public class HospitalStaffListItemDto
{
    public long EmployeeNumber { get; set; }
    public string Name { get; set; } = "";
    public string Specialization { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = "";
}