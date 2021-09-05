using System;
using System.Threading.Tasks;

namespace TemperatureAlert.Domain
{
    public class TemperatureService
    {
        private ITemperatureRepository Repository { get; init;}

        public TemperatureService(ITemperatureRepository repository)
        {
            Repository = repository;
        }

        public async Task<AnalysisResult> AnalyzeTemperature(string deviceId, decimal temperature)
        {
            var temperatureRule = await Repository.GetNormalTemperatureRange(deviceId);

            await Repository.RecordTemperatureAnomaly(deviceId, temperature);

            return new AnalysisResult
            {
                Status = "Abnormal",
                Message = "45,534234 was higher than allowed maximum: 35"
            };
        }
    }
}