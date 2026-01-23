using System.Collections.Generic;
using System.Linq;

namespace HeelmeestersAPI.Features.Shared.MedicalRecords;

public class MedicalRecordResponseDto
{
    public int LineNumber { get; set; }
    public long PatientNumber { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public byte[]? File { get; set; }
}

public static class MedicalRecordResponseMapper
{
    public static MedicalRecordResponseDto Map(MedicalRecord record) => new()
    {
        LineNumber = record.LineNumber,
        PatientNumber = record.PatientNumber,
        PatientName = $"Patient {record.PatientNumber}",
        Description = record.Description,
        CreatedOn = record.CreatedOn,
        File = record.File
    };

    public static List<MedicalRecordResponseDto> MapMany(IEnumerable<MedicalRecord> records) => records
        .Select(Map)
        .ToList();
}
