using System;
using System.Threading.Tasks;

namespace TemperatureAlert.Domain
{
    public class TemperatureService
    {
        private ITemperatureRepository Repository { get; init; }
        private IAlertService AlertService { get; init; }

        public TemperatureService(ITemperatureRepository repository, IAlertService alertService)
        {
            Repository = repository;
            AlertService = alertService;
        }

        public async Task<AnalysisResult> AnalyzeTemperature(string deviceId, decimal temperature, DateTime dateTime)
        {
            var temperatureRule = await Repository.GetNormalTemperatureRange(deviceId);

            if (temperature < temperatureRule.MinTemperature || temperature > temperatureRule.MaxTemperature)
            {
                await Repository.RecordTemperatureAnomaly(deviceId, temperature);

                var numberOfAnomalies = await Repository.GetTemperatureAnomalyCount(deviceId, dateTime.AddMinutes(-temperatureRule.MaximumMinutes));

                if (numberOfAnomalies >= temperatureRule.MaximumNumberOfAnomalies)
                {
                    await AlertService.SendTemperatureAlert(deviceId, temperature, dateTime);
                }

                return new AnalysisResult
                {
                    Status = "Abnormal",
                    Message = $"{temperature} was higher than allowed maximum: {temperatureRule.MaxTemperature}"
                };
            }
            else
            {
                return new AnalysisResult
                {
                    Status = "Normal",
                    Message = $"{temperature} was OK."
                };
            }
        }
    }
}
