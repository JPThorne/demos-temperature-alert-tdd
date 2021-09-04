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

            //act
            var service = new TemperatureService();

            //assert
            Assert.NotNull(service);
        }
    }
}