namespace HeelmeestersAPI.Features.AdminPortal.Users.DTOs;

public class GeneralPractitionerListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = "";
}