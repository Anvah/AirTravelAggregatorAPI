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
                    Id = "AA123",
                    Airline = "American Airlines",
                    Transfers = Enumerable.Empty<FirstFlightTransfer>(),
                    Price = 250.00m
                },
                new FirstFlight
                {
                    Id = "KL456",
                    Airline = "KLM Royal Dutch Airlines",
                    Transfers = new List<FirstFlightTransfer>
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "Amsterdam",
                            ArrivalDataTime = new DateTime(2024, 03, 16, 10, 30, 0),
                            DepartureDataTime = new DateTime(2024, 03, 16, 12, 0, 0)
                        }
                    },
                    Price = 180.50m
                },
                new FirstFlight
                {
                    Id = "OZ789",
                    Airline = "Asiana Airlines",
                    Transfers = Enumerable.Empty<FirstFlightTransfer>(),
                    Price = 300.00m
                },
                new FirstFlight
                {
                    Id = "TG101",
                    Airline = "Thai Airways",
                    Transfers = new List<FirstFlightTransfer>
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "Bangkok",
                            ArrivalDataTime = new DateTime(2024, 03, 16, 8, 0, 0),
                            DepartureDataTime = new DateTime(2024, 03, 16, 9, 30, 0)
                        },
                        new FirstFlightTransfer
                        {
                            Airport = "Singapore",
                            ArrivalDataTime = new DateTime(2024, 03, 16, 14, 0, 0),
                            DepartureDataTime = new DateTime(2024, 03, 16, 16, 0, 0)
                        }
                    },
                    Price = 400.00m
                },
                new FirstFlight
                {
                    Id = "SU567",
                    Airline = "Aeroflot",
                    Transfers = Enumerable.Empty<FirstFlightTransfer>(),
                    Price = 100.00m
                },
                new FirstFlight
                {
                    Id = "LH654",
                    Airline = "Lufthansa",
                    Transfers = new List<FirstFlightTransfer>
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "Frankfurt",
                            ArrivalDataTime = new DateTime(2024, 03, 16, 11, 0, 0),
                            DepartureDataTime = new DateTime(2024, 03, 16, 12, 30, 0)
                        }
                    },
                    Price = 120.00m
                },
                new FirstFlight
                {
                    Id = "IB987",
                    Airline = "Iberia",
                    Transfers = Enumerable.Empty<FirstFlightTransfer>(),
                    Price = 220.00m
                },
                new FirstFlight
                {
                    Id = "EK753",
                    Airline = "Emirates",
                    Transfers = new List<FirstFlightTransfer>
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "Mumbai",
                            ArrivalDataTime = new DateTime(2024, 03, 16, 9, 0, 0),
                            DepartureDataTime = new DateTime(2024, 03, 16, 10, 30, 0)
                        }
                    },
                    Price = 280.00m
                },
                new FirstFlight
                {
                    Id = "KE888",
                    Airline = "Korean Air",
                    Transfers = Enumerable.Empty<FirstFlightTransfer>(),
                    Price = 200.00m
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