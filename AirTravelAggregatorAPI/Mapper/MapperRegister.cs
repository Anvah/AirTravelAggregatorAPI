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
             .Map(dest => dest.Id, src => src.Id)
             .Map(dest => dest.OriginalId, src => src.Id)
             .Map(dest => dest.Airline, src => new Airline { Name = src.Airline })
             .Map(dest => dest.ArrivalPoint, src => CreateDestinationFromFirstFlightTransfer(src.ArrivalPoint))
             .Map(dest => dest.DeparturePoint, src => CreateDestinationFromFirstFlightTransfer(src.DeparturePoint))
             .Map(dest => dest.Transfers, src => CreateDestinationsFromFirstFlightTransfer(src.Transfers))
             .Map(dest => dest.Price, src => src.Price)
             .Map(dest => dest.Baggage, src => new Baggage { IsAvailable = true})
             .Map(dest => dest.Sourse, src => FlightSourse.FirstFlightService);


            config.NewConfig<SecondFlight, Flight>()
                .Map(dest => dest.OriginalId, src => src.Id)
                .Map(agf => agf.Airline, f => new Airline { Name = f.Airline })
                .Map(agf => agf.Price, f => f.Price)
                .Map(agf => agf.DeparturePoint, f => new Destination {AirportName = f.DepartureAirport, CityName = f.DepartureCity, CountryName = f.DepartureCountry, DepartureTime = DateTime.Parse(f.DepartureTime) })
                .Map(agf => agf.ArrivalPoint, f => new Destination { AirportName = f.ArrivalAirport, CityName = f.ArrivalCity, CountryName = f.ArrivalCountry, ArrivalTime = DateTime.Parse(f.ArrivalTime) })
                .Map(agf => agf.Transfers, f => CreateDestinationsFroSecondFlightTransfer(f.Transfres, f.TransfersArivalDateTime, f.TransfersDepartureDateTime))
                .Map(agf => agf.Baggage, f => new Baggage { IsAvailable = f.IsBaggageAvaible, Price = f.BaggagePrice })
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
                /*DateTime arrivalDate;
                bool isArrDateCovert = DateTime.TryParse(AirportCityCountry[i],out arrivalDate);
                DateTime departureDate;
                bool isDepDateCovert = DateTime.TryParse(departureDates[i],out departureDate);
                if(!(isArrDateCovert || isArrDateCovert))
                    return null;*/
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
