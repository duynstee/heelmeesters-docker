using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace HeelmeestersAPI.Features.Shared.MedicalRecords
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly List<MedicalRecord> _records = new();

        public MedicalRecordRepository()
        {
            // Try to discover PDF files located in the project root (three levels up from the build output directory)
            try
            {
                var baseDir = AppContext.BaseDirectory; // e.g. ...\bin\Debug\net8.0\
                var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..\\..\\..\\"));

                Console.WriteLine($"MedicalRecordRepository scanning for PDFs in: {projectRoot}");

                var pdfFiles = Directory.Exists(projectRoot)
                    ? Directory.GetFiles(projectRoot, "*.pdf", SearchOption.TopDirectoryOnly)
                    : Array.Empty<string>();

                if (pdfFiles.Length == 0)
                {
                    // Fallback: try current directory
                    pdfFiles = Directory.GetFiles(baseDir, "*.pdf", SearchOption.TopDirectoryOnly);
                }

                Console.WriteLine($"Found {pdfFiles.Length} PDF(s) to import.");

                int id = 1;
                foreach (var file in pdfFiles.OrderBy(f => f))
                {
                    try
                    {
                        var bytes = File.ReadAllBytes(file);
                        var title = Path.GetFileNameWithoutExtension(file);
                        var created = File.GetCreationTimeUtc(file);

                        Console.WriteLine($"Importing PDF: {Path.GetFileName(file)} ({bytes.Length} bytes)");

                        _records.Add(new MedicalRecord
                        {
                            Id = id++,
                            PatientId = 101, // demo patient id; adjust as needed
                            Title = title,
                            Description = $"Imported from file {Path.GetFileName(file)}",
                            Notes = $"PDF import: {Path.GetFileName(file)}",
                            Document = bytes,
                            CreatedAt = created.ToLocalTime()
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to import {file}: {ex.Message}");
                        // ignore individual file failures
                    }
                }

                // If still empty, add a small default record so API isn't empty
                if (_records.Count == 0)
                {
                    _records.Add(new MedicalRecord
                    {
                        Id = 1,
                        PatientId = 101,
                        Notes = "Pijn op de borst, onderzocht door arts.",
                        Title = "Cardiale Evaluatie",
                        Document = Array.Empty<byte>(),
                        CreatedAt = DateTime.Now.AddDays(-1).AddHours(-2)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MedicalRecordRepository failed to seed PDFs: {ex.Message}");
                // any errors while trying to seed should not break the app
                _records.Add(new MedicalRecord
                {
                    Id = 1,
                    PatientId = 101,
                    Notes = "Pijn op de borst, onderzocht door arts.",
                    Title = "Cardiale Evaluatie",
                    Document = Array.Empty<byte>(),
                    CreatedAt = DateTime.Now.AddDays(-1).AddHours(-2)
                });
            }
        }

        public Task AddAsync(MedicalRecord record)
        {
            _records.Add(record);
            return Task.CompletedTask;
        }

        public Task<List<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return Task.FromResult(_records.Where(r => r.PatientId == patientId).ToList());
        }

        public Task<List<MedicalRecord>> GetAllAsync()
        {
            return Task.FromResult(_records.ToList());
        }

        public Task<MedicalRecord?> GetByIdAsync(int id)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.Id == id));
        }
    }
}