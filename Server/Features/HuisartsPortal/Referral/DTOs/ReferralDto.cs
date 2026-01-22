namespace HeelmeestersAPI.Features.HuisartsPortal.Referral.DTOs;

public class ReferralDto
{
    public int Id { get; set; }
    public long PatientNumber { get; set; }
    public string CareCode { get; set; } = "";
    
    public string TreatmentDescription { get; set; } = "";
    
    public bool IsUsed {get; set;}
    public DateTime? UsedOn { get; set; }
}