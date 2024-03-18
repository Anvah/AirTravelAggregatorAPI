using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using Refit;
using AirTravelAggregatorAPI.Services.Interfaces;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using AirTravelAggregatorAPI.Services;
using System.Net.Http;
using Mapster;
using AirTravelAggregatorAPI.Models.AggregatedModels;
using MapsterMapper;
using AirTravelAggregatorAPI.Mapper;

namespace AirTravelAggregatorTests
{
    public class FlightAggregateServiceTests
    {
        [Fact]
        public async Task GetFlights_Returns_Combined_Flights_From_Both_Services()
        {
            /*var cancellationToken = CancellationToken.None;
            var config = new TypeAdapterConfig();
            var register = new MapperRegister();
            register.Register(config);
            var mapper = new Mapper(config);
            var firstFlights = CreateFirstFlightTestData();
            var secondFlights = CreateSecondFlightTestData();

            var firstFlightServiceMock = new Mock<IFirstFlightService>();
            firstFlightServiceMock.Setup(service => service.GetFlights(cancellationToken))
                                  .ReturnsAsync(new ApiResponse<IEnumerable<FirstFlight>>(new HttpResponseMessage(), firstFlights, new RefitSettings()));

            var secondFlightServiceMock = new Mock<ISecondFlightService>();
            secondFlightServiceMock.Setup(service => service.GetFlights(cancellationToken))
                                   .ReturnsAsync(new ApiResponse<IEnumerable<SecondFlight>>(new HttpResponseMessage(), secondFlights, new RefitSettings()));

            var flightAggregateService = new FlightAggregateService(firstFlightServiceMock.Object, secondFlightServiceMock.Object, mapper);

            // Act
            var flights = await flightAggregateService.GetFlights(cancellationToken);

            // Assert
            Assert.Contains(flights, flight => flight.Airline.Name == "Airline6s");
            Assert.Contains(flights, flight => flight.Airline.Name == "Airline6f");*/
        }
        private List<FirstFlight> CreateFirstFlightTestData()
        {
            return new List<FirstFlight>
            {
                new FirstFlight
                {
                    Id = "NYC-LON-PAR-1",
                    Airline = "Example Airways",
                    DeparturePoint = new FirstFlightTransfer
                    {
                        Airport = "JFK JFK JFK",
                        DepartureDataTime = DateTime.UtcNow,
                    },
                    ArrivalPoint = new FirstFlightTransfer
                    {
                        Airport = "LHR JFK JFK",
                        ArrivalDataTime = DateTime.UtcNow.AddHours(8),
                    },
                    Transfers = new FirstFlightTransfer[]
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "CDG JFK JFK",
                            ArrivalDataTime = DateTime.UtcNow.AddHours(3),
                            DepartureDataTime = DateTime.UtcNow.AddHours(4),
                        }
                    },
                    Price = 1200.00m
                },
               new FirstFlight
               {
                   Id = "PAR-TYO-1",
                   Airline = "Example Airways",
                   DeparturePoint = new FirstFlightTransfer
                   {
                       Airport = "CDG JFK JFK",
                       DepartureDataTime = DateTime.UtcNow,
                   },
                   ArrivalPoint = new FirstFlightTransfer
                   {
                       Airport = "HND JFK JFK",
                       ArrivalDataTime = DateTime.UtcNow.AddHours(14),
                   },
                   Transfers = new FirstFlightTransfer[] { },
                   Price = 1800.00m
               },
               new FirstFlight
               {
                   Id = "LAX-SYD-AKL-1",
                   Airline = "Example Airways",
                   DeparturePoint = new FirstFlightTransfer
                   {
                       Airport = "LAX JFK JFK",
                       DepartureDataTime = DateTime.UtcNow,
                   },
                   ArrivalPoint = new FirstFlightTransfer
                   {
                       Airport = "SYD JFK JFK",
                       ArrivalDataTime = DateTime.UtcNow.AddHours(20),
                   },
                   Transfers = new FirstFlightTransfer[]
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "AKL JFK JFK",
                            ArrivalDataTime = DateTime.UtcNow.AddHours(15),
                            DepartureDataTime = DateTime.UtcNow.AddHours(16),
                        }
                    },
                   Price = 2500.00m
               },
               new FirstFlight
               {
                   Id = "SHA-ICN-1",
                   Airline = "Example Airways",
                   DeparturePoint = new FirstFlightTransfer
                   {
                       Airport = "PVG JFK JFK",
                       DepartureDataTime = DateTime.UtcNow,
                   },
                   ArrivalPoint = new FirstFlightTransfer
                   {
                       Airport = "ICN JFK JFK",
                       ArrivalDataTime = DateTime.UtcNow.AddHours(2),
                   },
                   Transfers = new FirstFlightTransfer[] { },
                   Price = 900.00m
               }

            };
        }

        private List<SecondFlight> CreateSecondFlightTestData()
        {
            return new List<SecondFlight>
            {
                new SecondFlight
                {
                    Id = "SF001",
                    Airline = "Airline1s",
                    DepartureAirport = "Airport1s",
                    DepartureCity = "City1s",
                    ArrivalAirport = "Airport2s",
                    ArrivalCity = "City2s",
                    DepartureTime = "08:00",
                    ArrivalTime = "10:00",
                    Price = 250.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 25.00m
                },
                new SecondFlight
                {
                    Id = "SF002",
                    Airline = "Airline2s",
                    DepartureAirport = "Airport3s",
                    DepartureCity = "City3s",
                    ArrivalAirport = "Airport4s",
                    ArrivalCity = "City4s",
                    DepartureTime = "10:00",
                    ArrivalTime = "12:00",
                    Price = 300.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 20.00m
                },
                new SecondFlight
                {
                    Id = "SF003",
                    Airline = "Airline3s",
                    DepartureAirport = "Airport5s",
                    DepartureCity = "City5s",
                    ArrivalAirport = "Airport6s",
                    ArrivalCity = "City6s",
                    DepartureTime = "12:00",
                    ArrivalTime = "14:00",
                    Price = 280.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 15.00m
                },
                new SecondFlight
                {
                    Id = "SF004",
                    Airline = "Airline4s",
                    DepartureAirport = "Airport7s",
                    DepartureCity = "City7s",
                    ArrivalAirport = "Airport8s",
                    ArrivalCity = "City8",
                    DepartureTime = "14:00",
                    ArrivalTime = "16:00",
                    Price = 320.00m,
                    IsBaggageAvaible = false,
                    BaggagePrice = 0.00m
                },
                new SecondFlight
                {
                    Id = "SF005",
                    Airline = "Airline5s",
                    DepartureAirport = "Airport9s",
                    DepartureCity = "City9s",
                    ArrivalAirport = "Airport10s",
                    ArrivalCity = "City10",
                    DepartureTime = "16:00",
                    ArrivalTime = "18:00",
                    Price = 350.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 25.00m
                },
                new SecondFlight
                {
                    Id = "SF006",
                    Airline = "Airline6s",
                    DepartureAirport = "Airport11s",
                    DepartureCity = "City11s",
                    ArrivalAirport = "Airport12s",
                    ArrivalCity = "City12",
                    DepartureTime = "18:00",
                    ArrivalTime = "20:00",
                    Price = 270.00m,
                    IsBaggageAvaible = false,
                    BaggagePrice = 0.00m
                }

            };
        }
    }
}