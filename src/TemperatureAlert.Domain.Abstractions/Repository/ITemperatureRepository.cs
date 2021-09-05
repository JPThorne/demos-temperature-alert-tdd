using System.Threading.Tasks;

namespace TemperatureAlert.Domain
{
    public interface ITemperatureRepository
    {
        Task<TemperatureRule> GetNormalTemperatureRange(string deviceId);
        Task RecordTemperatureAnomaly(string deviceId, decimal abnormalTemperature);
    }
}
