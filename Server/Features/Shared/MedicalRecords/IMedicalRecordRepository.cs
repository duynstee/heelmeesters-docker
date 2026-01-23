using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeelmeestersAPI.Features.Shared.MedicalRecords
{
    public interface IMedicalRecordRepository
    {
        Task<List<MedicalRecord>> GetByPatientNumberAsync(long patientNumber);
        Task<List<MedicalRecord>> GetAllAsync();
        Task AddAsync(MedicalRecord record, CancellationToken cancellationToken = default);
        Task<MedicalRecord?> GetByLineNumberAsync(int lineNumber, CancellationToken cancellationToken = default);
    }
}