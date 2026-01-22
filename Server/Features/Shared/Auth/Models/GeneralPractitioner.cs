namespace HeelmeestersAPI.Features.Shared.Auth.Models;

public class GeneralPractitioner
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string? Prefix { get; set; }
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }

    public int UserId { get; set; }
}