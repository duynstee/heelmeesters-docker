using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeelmeestersAPI.Features.Shared.MedicalRecords
{
    public interface IMedicalRecordRepository
    {
        Task<List<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<List<MedicalRecord>> GetAllAsync();
        Task AddAsync(MedicalRecord record);
        Task<MedicalRecord?> GetByIdAsync(int id);
    }
}