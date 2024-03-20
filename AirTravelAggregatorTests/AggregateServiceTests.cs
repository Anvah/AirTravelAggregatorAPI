using AirTravelAggregatorAPI.Services;
using AirTravelAggregatorAPI.Mapper;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Services.TestServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace AirTravelAggregatorTests
{
    public class FlightAggregateServiceTests
    {
        [Fact]
        public async Task GetFlightsFromBothServices()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var date = DateTime.UtcNow;
            var sortProperty = SortProperty.ByPrice;
            var maxPrice = decimal.MaxValue;
            var airlineName = "Some Airline";
            var maxTransfersCount = int.MaxValue;

            var firstFlightServiceMock = new FirstFlightTestService();

            var secondFlightServiceMock = new SecondFlightTestService();

            var config = new TypeAdapterConfig();
            var register = new MapperRegister();
            register.Register(config);
            var mapper = new Mapper(config);

            var loggerMock = new Mock<ILogger<FlightAggregateService>>();
            var memoryCache = new Mock<IMemoryCache>();

            var service = new FlightAggregateService(firstFlightServiceMock, secondFlightServiceMock, mapper, loggerMock.Object, memoryCache.Object);

            // Act
            var flights = await service.GetFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);

            // Assert
            Assert.Equal(4, flights.Count());
            Assert.Contains(flights, flight => flight.Id == "NYC-LON-PAR-1");
            Assert.Contains(flights, flight => flight.Id== "SU123");
        }

        [Fact]
        public async Task BookFlightFromFirstService()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var originalId = "NYC-LON-PAR-1";
            var source = FlightSourse.FirstFlightService;
            var firstFlightServiceMock = new FirstFlightTestService();

            var config = new TypeAdapterConfig();
            var register = new MapperRegister();
            register.Register(config);
            var mapper = new Mapper(config);

            var loggerMock = new Mock<ILogger<FlightAggregateService>>();
            var memoryCache = new Mock<IMemoryCache>();

            var service = new FlightAggregateService(firstFlightServiceMock, null, mapper, loggerMock.Object, memoryCache.Object);

            // Act
            var bookedFlight = await service.Book(cancellationToken, originalId, source);

            // Assert
            Assert.NotNull(bookedFlight);
        }
        [Fact]
        public async Task BookFlightFromSecondService()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var originalId = "SU123";
            var source = FlightSourse.SecondFlightService;
            var secondFlightServiceMock = new SecondFlightTestService();

            var config = new TypeAdapterConfig();
            var register = new MapperRegister();
            register.Register(config);
            var mapper = new Mapper(config);

            var loggerMock = new Mock<ILogger<FlightAggregateService>>();
            var memoryCache = new Mock<IMemoryCache>();

            var service = new FlightAggregateService(null, secondFlightServiceMock, mapper, loggerMock.Object, memoryCache.Object);

            // Act
            var bookedFlight = await service.Book(cancellationToken, originalId, source);

            // Assert
            Assert.NotNull(bookedFlight);
        }
    }
}