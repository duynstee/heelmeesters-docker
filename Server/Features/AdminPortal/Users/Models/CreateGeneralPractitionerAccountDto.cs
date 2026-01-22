namespace HeelmeestersAPI.Features.AdminPortal.Users.Models;

public class CreateGeneralPractitionerAccountDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    public int Id { get; set; } // gp.id is INT in jouw DB
    public string FirstName { get; set; } = "";
    public string? Prefix { get; set; }
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
}