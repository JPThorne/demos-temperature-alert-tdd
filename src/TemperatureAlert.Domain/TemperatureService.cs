using System;
using System.Threading.Tasks;

namespace TemperatureAlert.Domain
{
    public class TemperatureService
    {
        public TemperatureService(ITemperatureRepository repository)
        {
        }

        public Task<AnalysisResult> AnalyzeTemperature(string deviceId, decimal abnormalTemperature)
        {
            throw new NotImplementedException();
        }
    }
}