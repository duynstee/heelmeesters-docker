namespace HeelmeestersAPI.Features.HuisartsPortal.Patient.DTOs;

public class PatientListItemDto
{
    public long PatientNumber { get; set; }
    public string FirstName { get; set; } = null!;
    public string? Prefix { get; set; }
    public string LastName { get; set; } = null!;
}