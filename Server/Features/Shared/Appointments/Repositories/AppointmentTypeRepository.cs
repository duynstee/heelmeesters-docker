using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*
 kan waarschijnlijk weg
 
namespace HeelmeestersAPI.Features.Shared.Appointments
{
    public class AppointmentTypeRepository : IAppointmentTypeRepository
    {
        private readonly List<AppointmentType> _types = new()
        {
            new AppointmentType { Id = 1, Name = "Consult", Duration = TimeSpan.FromMinutes(30) },
            new AppointmentType { Id = 2, Name = "Check-up", Duration = TimeSpan.FromMinutes(60) },
            new AppointmentType { Id = 3, Name = "Telefonisch Consult", Duration = TimeSpan.FromMinutes(15) },
            new AppointmentType { Id = 4, Name = "Operatie Intake", Duration = TimeSpan.FromMinutes(45) }
        };

        public Task<AppointmentType?> GetByIdAsync(int id)
        {
            return Task.FromResult(_types.FirstOrDefault(t => t.Id == id));
        }

        public Task<List<AppointmentType>> GetAllAsync()
        {
            return Task.FromResult(_types.ToList());
        }

        public Task<List<AppointmentType>> GetAppointmentTypesAsync()
        {
            return Task.FromResult(_types.ToList());
        }
    }
} */