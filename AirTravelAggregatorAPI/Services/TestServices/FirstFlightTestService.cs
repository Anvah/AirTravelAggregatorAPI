using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using AirTravelAggregatorAPI.Services.Interfaces;
using Refit;
using System.Security.Cryptography.Xml;
using System.Threading;

namespace AirTravelAggregatorAPI.Services.TestServices
{
    public class FirstFlightTestService: IFirstFlightService
    {
        private static List<FirstFlight> firstFlights = new List<FirstFlight>
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
        async public Task<FirstFlight> Book(string Id, CancellationToken cancellationToken = default)
        {
            var flight = firstFlights.FirstOrDefault(f => f.Id == Id);
            if(flight != null)
                flight.IsBooked = true;
            return flight;
        }

        public async Task<ApiResponse<IEnumerable<FirstFlight>>> GetFlights(DateTime date, decimal maxPrice = decimal.MaxValue, int maxTransfersCount = int.MaxValue, CancellationToken cancellationToken = default)
        {
            //await Task.Delay(10000, cancellationToken);
            var sortedFlight = firstFlights
                .Where(f => f.DeparturePoint.DepartureDataTime.Date == date.Date
                && f.Price < maxPrice
                && f.Transfers.Length < maxTransfersCount
                );
            return new ApiResponse<IEnumerable<FirstFlight>>(new HttpResponseMessage(), sortedFlight, new RefitSettings());
        }
    }
}
