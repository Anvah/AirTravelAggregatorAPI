using System;
using AirTravelAggregatorAPI.Mapper;
using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;

namespace AirTravelAggregatorAPI.Mapper
{
    public partial class FlightMapper : IFlightMapper
    {
        private Func<FirstFlightTransfer, Destination> CreateDestinationFromFirstFlightTransfer1;
        private Func<FirstFlightTransfer[], Destination[]> CreateDestinationsFromFirstFlightTransfer1;
        private Func<string[], string[], string[], Destination[]> CreateDestinationsFroSecondFlightTransfer1;
        
        public Flight MapTo(FirstFlight p1)
        {
            return p1 == null ? null : new Flight()
            {
                Id = p1.Id,
                OriginalId = p1.Id,
                ArrivalPoint = funcMain1(CreateDestinationFromFirstFlightTransfer1.Invoke(p1.ArrivalPoint)),
                DeparturePoint = funcMain2(CreateDestinationFromFirstFlightTransfer1.Invoke(p1.DeparturePoint)),
                Transfers = funcMain3(CreateDestinationsFromFirstFlightTransfer1.Invoke(p1.Transfers)),
                Airline = funcMain4(new Airline() {Name = p1.Airline}),
                Price = p1.Price,
                Baggage = funcMain5(new Baggage() {IsAvailable = true}),
                IsBooked = p1.IsBooked,
                Sourse = FlightSourse.FirstFlightService
            };
        }
        public Flight MapTo(SecondFlight p7)
        {
            return p7 == null ? null : new Flight()
            {
                Id = p7.Id,
                OriginalId = p7.Id,
                ArrivalPoint = funcMain6(new Destination()
                {
                    AirportName = p7.ArrivalAirport,
                    CityName = p7.ArrivalCity,
                    CountryName = p7.ArrivalCountry
                }),
                DeparturePoint = funcMain7(new Destination()
                {
                    AirportName = p7.DepartureAirport,
                    CityName = p7.DepartureCity,
                    CountryName = p7.DepartureCountry
                }),
                Transfers = funcMain8(CreateDestinationsFroSecondFlightTransfer1.Invoke(p7.Transfres, p7.TransfersArivalDateTime, p7.TransfersDepartureDateTime)),
                Airline = funcMain9(new Airline() {Name = p7.Airline}),
                Price = p7.Price,
                Baggage = funcMain10(new Baggage()
                {
                    IsAvailable = p7.IsBaggageAvaible,
                    Price = p7.BaggagePrice
                }),
                IsBooked = p7.IsBooked,
                Sourse = FlightSourse.SecondFlightService
            };
        }
        
        private Destination funcMain1(Destination p2)
        {
            return p2 == null ? null : new Destination()
            {
                AirportName = p2.AirportName,
                CityName = p2.CityName,
                CountryName = p2.CountryName,
                ArrivalTime = p2.ArrivalTime,
                DepartureTime = p2.DepartureTime
            };
        }
        
        private Destination funcMain2(Destination p3)
        {
            return p3 == null ? null : new Destination()
            {
                AirportName = p3.AirportName,
                CityName = p3.CityName,
                CountryName = p3.CountryName,
                ArrivalTime = p3.ArrivalTime,
                DepartureTime = p3.DepartureTime
            };
        }
        
        private Destination[] funcMain3(Destination[] p4)
        {
            if (p4 == null)
            {
                return null;
            }
            Destination[] result = new Destination[p4.Length];
            
            int v = 0;
            
            int i = 0;
            int len = p4.Length;
            
            while (i < len)
            {
                Destination item = p4[i];
                result[v++] = item == null ? null : new Destination()
                {
                    AirportName = item.AirportName,
                    CityName = item.CityName,
                    CountryName = item.CountryName,
                    ArrivalTime = item.ArrivalTime,
                    DepartureTime = item.DepartureTime
                };
                i++;
            }
            return result;
            
        }
        
        private Airline funcMain4(Airline p5)
        {
            return p5 == null ? null : new Airline()
            {
                Name = p5.Name,
                Description = p5.Description,
                Rating = p5.Rating
            };
        }
        
        private Baggage funcMain5(Baggage p6)
        {
            return p6 == null ? null : new Baggage()
            {
                IsAvailable = p6.IsAvailable,
                Price = p6.Price
            };
        }
        
        private Destination funcMain6(Destination p8)
        {
            return p8 == null ? null : new Destination()
            {
                AirportName = p8.AirportName,
                CityName = p8.CityName,
                CountryName = p8.CountryName,
                ArrivalTime = p8.ArrivalTime,
                DepartureTime = p8.DepartureTime
            };
        }
        
        private Destination funcMain7(Destination p9)
        {
            return p9 == null ? null : new Destination()
            {
                AirportName = p9.AirportName,
                CityName = p9.CityName,
                CountryName = p9.CountryName,
                ArrivalTime = p9.ArrivalTime,
                DepartureTime = p9.DepartureTime
            };
        }
        
        private Destination[] funcMain8(Destination[] p10)
        {
            if (p10 == null)
            {
                return null;
            }
            Destination[] result = new Destination[p10.Length];
            
            int v = 0;
            
            int i = 0;
            int len = p10.Length;
            
            while (i < len)
            {
                Destination item = p10[i];
                result[v++] = item == null ? null : new Destination()
                {
                    AirportName = item.AirportName,
                    CityName = item.CityName,
                    CountryName = item.CountryName,
                    ArrivalTime = item.ArrivalTime,
                    DepartureTime = item.DepartureTime
                };
                i++;
            }
            return result;
            
        }
        
        private Airline funcMain9(Airline p11)
        {
            return p11 == null ? null : new Airline()
            {
                Name = p11.Name,
                Description = p11.Description,
                Rating = p11.Rating
            };
        }
        
        private Baggage funcMain10(Baggage p12)
        {
            return p12 == null ? null : new Baggage()
            {
                IsAvailable = p12.IsAvailable,
                Price = p12.Price
            };
        }
    }
}