using System;
using System.Threading.Tasks;

namespace TemperatureAlert.Domain
{
    public class TemperatureService
    {
        private ITemperatureRepository Repository { get; init; }

        public TemperatureService(ITemperatureRepository repository)
        {
            Repository = repository;
        }

        public async Task<AnalysisResult> AnalyzeTemperature(string deviceId, decimal temperature, DateTime dateTime)
        {
            var temperatureRule = await Repository.GetNormalTemperatureRange(deviceId);

            if (temperature < temperatureRule.MinTemperature || temperature > temperatureRule.MaxTemperature)
            {
                await Repository.RecordTemperatureAnomaly(deviceId, temperature);

                return new AnalysisResult
                {
                    Status = "Abnormal",
                    Message = "45,534234 was higher than allowed maximum: 35"
                };
            }
            else
            {
                return new AnalysisResult
                {
                    Status = "Normal",
                    Message = $"25,123 was OK."
                };
            }
        }
    }
}
