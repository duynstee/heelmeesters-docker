using System.Threading;
using System.Threading.Tasks;

namespace HeelmeestersAPI.Features.Shared.Auth.Interfaces
{
    public interface IPatientIdentityService
    {
        Task<long> GetPatientNumberForUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
