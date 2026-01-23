namespace HeelmeestersAPI.Features.Shared.Appointments.Dtos;

public class SlotDto
{
    public string Time { get; set; } = "";
    public bool Available { get; set; }
    public string? Reason { get; set; }
}