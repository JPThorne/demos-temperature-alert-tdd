using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TemperatureAlert.Domain.Tests
{
    public class TemperatureServiceTests
    {
        public TemperatureServiceTests() { }

        [Fact]
        public void Test_CanConstruct()
        {
            //arrange
            var repository = Substitute.For<ITemperatureRepository>();

            //act
            var service = new TemperatureService(repository);

            //assert
            Assert.NotNull(service);
        }

        [Fact]
        public async Task Test_AnomalyIsRecordedIfTemperatureIsAbnormal()
        {
            //arrange
            var deviceId = "1";
            var abnormalTemperature = 45.534234m;
            var dateTime = DateTime.UtcNow;

            var normalMinTemperature = 10m;
            var normalMaxTemperature = 35m;

            var repository = Substitute.For<ITemperatureRepository>();

            repository.GetNormalTemperatureRange(deviceId).Returns(new TemperatureRule
            {
                MinTemperature = normalMinTemperature,
                MaxTemperature = normalMaxTemperature
            });

            var service = new TemperatureService(repository);

            //act
            var result = await service.AnalyzeTemperature(deviceId, abnormalTemperature, dateTime);

            //assert
            Assert.NotNull(result);
            Assert.Equal("Abnormal", result.Status);
            Assert.Equal($"{abnormalTemperature} was higher than allowed maximum: {normalMaxTemperature}", result.Message);
            await repository.Received(1).GetNormalTemperatureRange(deviceId);
            await repository.Received(1).RecordTemperatureAnomaly(deviceId, abnormalTemperature);
        }

        [Fact]
        public async Task Test_IfTemperatureIsNormal_NoAnomalyIsRecorded()
        {
            //arrange
            var deviceId = "1";
            var normalTemperature = 25.123m;
            var dateTime = DateTime.UtcNow;

            var normalMinTemperature = 10m;
            var normalMaxTemperature = 35m;

            var repository = Substitute.For<ITemperatureRepository>();

            repository.GetNormalTemperatureRange(deviceId).Returns(new TemperatureRule
            {
                MinTemperature = normalMinTemperature,
                MaxTemperature = normalMaxTemperature
            });

            var service = new TemperatureService(repository);

            //act
            var result = await service.AnalyzeTemperature(deviceId, normalTemperature, dateTime);

            //assert
            Assert.NotNull(result);
            Assert.Equal("Normal", result.Status);
            Assert.Equal($"{normalTemperature} was OK.", result.Message);
            await repository.Received(1).GetNormalTemperatureRange(deviceId);
            await repository.Received(0).RecordTemperatureAnomaly(Arg.Any<string>(), Arg.Any<decimal>());
        }
    }
}
