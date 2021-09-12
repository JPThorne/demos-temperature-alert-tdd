using System;
using System.Threading.Tasks;

namespace TemperatureAlert.Domain
{
    public interface IAlertService
    {
        Task SendTemperatureAlert(string deviceId, decimal temperature, DateTime dateTime);
    }
}
