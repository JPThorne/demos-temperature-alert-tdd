using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TemperatureAlert.Domain.Tests
{
    public class TemperatureServiceTests
    {
        private IAlertService AlertService { get; init; }

        public TemperatureServiceTests()
        {
            AlertService = Substitute.For<IAlertService>();
        }

        [Fact]
        public void Test_CanConstruct()
        {
            //arrange
            var repository = Substitute.For<ITemperatureRepository>();

            //act
            var service = new TemperatureService(repository, AlertService);

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

            var service = new TemperatureService(repository, AlertService);

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

            var service = new TemperatureService(repository, AlertService);

            //act
            var result = await service.AnalyzeTemperature(deviceId, normalTemperature, dateTime);

            //assert
            Assert.NotNull(result);
            Assert.Equal("Normal", result.Status);
            Assert.Equal($"{normalTemperature} was OK.", result.Message);
            await repository.Received(1).GetNormalTemperatureRange(deviceId);
            await repository.Received(0).RecordTemperatureAnomaly(Arg.Any<string>(), Arg.Any<decimal>());
        }

        [Theory]
        [InlineData(5, 10)]
        [InlineData(6, 10)]
        public async Task Test_XOrMoreAnomaliesForTimeRangeY_ShouldTriggerAlert(int numberOfAnomalies, int numberOfMinutes)
        {
            //arrange
            var deviceId = "1";
            var abnormalTemperature = 42m;
            var now = DateTime.UtcNow;

            var normalMinTemperature = 10m;
            var normalMaxTemperature = 35m;

            var repository = Substitute.For<ITemperatureRepository>();

            repository.GetNormalTemperatureRange(deviceId).Returns(new TemperatureRule
            {
                MinTemperature = normalMinTemperature,
                MaxTemperature = normalMaxTemperature,
                MaximumMinutes = 10,
                MaximumNumberOfAnomalies = 5
            });

            var nowMinusXMinutes = now.AddMinutes(-numberOfMinutes);

            repository.GetTemperatureAnomalyCount(deviceId, nowMinusXMinutes).Returns(numberOfAnomalies);

            var service = new TemperatureService(repository, AlertService);

            //act
            var result = await service.AnalyzeTemperature(deviceId, abnormalTemperature, now);

            //assert
            Assert.NotNull(result);
            Assert.Equal("Abnormal", result.Status);
            Assert.Equal($"{abnormalTemperature} was higher than allowed maximum: {normalMaxTemperature}", result.Message);
            await repository.Received(1).GetNormalTemperatureRange(deviceId);
            await repository.Received(1).RecordTemperatureAnomaly(deviceId, abnormalTemperature);
            await repository.Received(1).GetTemperatureAnomalyCount(deviceId, nowMinusXMinutes);
            await AlertService.Received(1).SendTemperatureAlert(deviceId, abnormalTemperature, now);
        }
    }
}
