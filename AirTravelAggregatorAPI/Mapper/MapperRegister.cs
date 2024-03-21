using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using Mapster;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;

namespace AirTravelAggregatorAPI.Mapper
{
    public class MapperRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.NewConfig<FirstFlight, Flight>()
             .Map(dest => dest.Id, src => Guid.NewGuid())
             .Map(dest => dest.OriginalId, src => src.Id)
             .Map(dest => dest.Airline, src => new Airline { Name = src.Airline })
             .Map(dest => dest.ArrivalPoint, src => CreateDestinationFromFirstFlightTransfer(src.ArrivalPoint))
             .Map(dest => dest.DeparturePoint, src => CreateDestinationFromFirstFlightTransfer(src.DeparturePoint))
             .Map(dest => dest.Transfers, src => CreateDestinationsFromFirstFlightTransfer(src.Transfers))
             .Map(dest => dest.Price, src => src.Price)
             .Map(dest => dest.Baggage, src => new Baggage { IsAvailable = true})
             .Map(dest => dest.Sourse, src => FlightSourse.FirstFlightService);


            config.NewConfig<SecondFlight, Flight>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.OriginalId, src => src.Id)              
                .Map(dest => dest.Airline, src => new Airline { Name = src.Airline })
                .Map(dest => dest.Price, src => src.Price)
                .Map(dest => dest.DeparturePoint, src => new Destination {AirportName = src.DepartureAirport, CityName = src.DepartureCity, CountryName = src.DepartureCountry, DepartureTime = DateTime.Parse(src.DepartureTime) })
                .Map(dest => dest.ArrivalPoint, src => new Destination { AirportName = src.ArrivalAirport, CityName = src.ArrivalCity, CountryName = src.ArrivalCountry, ArrivalTime = DateTime.Parse(src.ArrivalTime) })
                .Map(dest => dest.Transfers, src => CreateDestinationsFroSecondFlightTransfer(src.Transfres, src.TransfersArivalDateTime, src.TransfersDepartureDateTime))
                .Map(dest => dest.Baggage, src => new Baggage { IsAvailable = src.IsBaggageAvaible, Price = src.BaggagePrice })
                .Map(dest => dest.Sourse, src => FlightSourse.SecondFlightService);
                
        }

        private static Destination CreateDestinationFromFirstFlightTransfer(FirstFlightTransfer transfer)
        {
            if(transfer == null)
                return null;
            var AirportCityCountry = transfer.Airport.Split(' ');
            if(AirportCityCountry.Length < 3 )
                return null;
            return new Destination
            {
                AirportName = AirportCityCountry[0],
                CityName = AirportCityCountry[1],
                CountryName = AirportCityCountry[2],
                ArrivalTime = transfer.ArrivalDataTime,
                DepartureTime= transfer.DepartureDataTime,
                
            };
        }
        private static Destination[] CreateDestinationsFromFirstFlightTransfer(FirstFlightTransfer[] transfers)
        {
            return transfers.Select(t => CreateDestinationFromFirstFlightTransfer(t)).ToArray();
        }
        private static Destination[] CreateDestinationsFroSecondFlightTransfer(string[] transfers, string[] arrivalDates, string[] departureDates)
        {
            if (!(transfers.Length == arrivalDates.Length && arrivalDates.Length == departureDates.Length))
                return null;
            Destination[] destinations = new Destination[transfers.Length];
            for (int i = 0; i < transfers.Length; i++)
            {
                var AirportCityCountry = transfers[i].Split(' ');
                if(AirportCityCountry.Length < 3)
                {
                    return null;
                }
                destinations[i] = new Destination
                {
                    AirportName = AirportCityCountry[0],
                    CityName = AirportCityCountry[1],
                    CountryName = AirportCityCountry[2],
                    ArrivalTime = arrivalDates[i].Adapt<DateTime>(),
                    DepartureTime = departureDates[i].Adapt<DateTime>()

                };
            }
            return destinations;
        }
    }
}
