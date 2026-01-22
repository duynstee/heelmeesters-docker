namespace HeelmeestersAPI.Features.AdminPortal.Users.DTOs;

public class PatientListItemDto
{
    public long PatientNumber { get; set; }
    public string Name { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = "";
}