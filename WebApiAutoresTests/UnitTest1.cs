using Microsoft.Extensions.Logging;
using Moq;
using WebApiAutores.Controllers;

namespace WebApiAutores.UnitTests
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            var weatherForecast = new WeatherForecastController();

            var result = weatherForecast.Get();

            Assert.IsNotEmpty(result);
        }
    }
}