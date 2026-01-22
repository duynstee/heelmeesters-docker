namespace HeelmeestersAPI.Features.HuisartsPortal.Referral.Models;

public class Referral
{
    public int Id { get; set; }
    public int GeneralPractitionerId { get; set; }
    public long PatientNumber { get; set; }
    public string CareCode { get; set; } = "";
    
    public bool IsUsed { get; set; }
    public DateTime? UsedOn { get; set; }
}