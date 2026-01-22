using System.Collections.Generic;
using System.Threading.Tasks;
using HeelmeestersAPI.Features.Shared.Appointments.Dtos;

namespace HeelmeestersAPI.Features.Shared.Appointments.Interfaces;

public interface ITreatmentProvider
{
    Task<List<Treatment>> GetTreatmentsAsync();
}