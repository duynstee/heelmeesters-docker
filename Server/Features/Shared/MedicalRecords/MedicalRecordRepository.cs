using HeelmeestersAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeelmeestersAPI.Features.Shared.MedicalRecords
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly AppDbContext _context;

        public MedicalRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MedicalRecord record, CancellationToken cancellationToken = default)
        {
            if (record.LineNumber == 0)
            {
                var maxLine = await _context.MedicalRecords
                    .Select(r => (int?)r.LineNumber)
                    .MaxAsync(cancellationToken) ?? 0;
                record.LineNumber = maxLine + 1;
            }

            if (record.CreatedOn == default)
            {
                record.CreatedOn = DateTime.UtcNow;
            }

            _context.Add(record);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<List<MedicalRecord>> GetByPatientNumberAsync(long patientNumber)
        {
            return _context.Set<MedicalRecord>()
                .Where(r => r.PatientNumber == patientNumber)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public Task<List<MedicalRecord>> GetAllAsync()
        {
            return _context.Set<MedicalRecord>()
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public Task<MedicalRecord?> GetByLineNumberAsync(int lineNumber, CancellationToken cancellationToken = default)
        {
            return _context.Set<MedicalRecord>()
                .FirstOrDefaultAsync(r => r.LineNumber == lineNumber, cancellationToken);
        }
    }
}