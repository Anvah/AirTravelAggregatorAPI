using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using AirTravelAggregatorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Refit;
using MapsterMapper;
using System.ComponentModel;
using AirTravelAggregatorAPI.Models.Enums;
using System.Security.Cryptography.Xml;

namespace AirTravelAggregatorAPI.Services
{
    public class FlightAggregateService: IFlightAggregatorService
    {
        private readonly IFirstFlightService _firstFlightService;
        private readonly ISecondFlightService _secondFlightService;
        private readonly IMapper _mapper;

        public FlightAggregateService(IFirstFlightService firstFlightService, ISecondFlightService secondFlightService, IMapper mapper)
        {
            _firstFlightService = firstFlightService;
            _secondFlightService = secondFlightService;
            _mapper = mapper;  
        }
        public async Task<IEnumerable<Flight>> GetFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            var fristFlights = await GetFirstServiceFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);
            var secondFlights = await GetSecondServiceFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);
            var flights = fristFlights.Select(f => _mapper.Map<FirstFlight, Flight>(f));
            flights = flights.Concat(secondFlights.Select(f => _mapper.Map<SecondFlight, Flight>(f)))
                .OrderBy(f => sortProperty == SortProperty.ByPrice ? f.Price : f.Transfers.Count());
            return flights;
        }
        public async Task<Flight> Book(CancellationToken cancellationToken, string originalId, FlightSourse sourse)
        {
            switch (sourse)
            {
                case FlightSourse.FirstFlightService:
                    var firstFlight = await _firstFlightService.Book(originalId, cancellationToken);
                    return _mapper.Map<FirstFlight, Flight>(firstFlight);
                case FlightSourse.SecondFlightService:
                    var secondFlight = await _secondFlightService.Book(originalId, cancellationToken);
                    return _mapper.Map<SecondFlight, Flight>(secondFlight);
                default:
                    return null;
            }

        }
        private async Task<IEnumerable<FirstFlight>> GetFirstServiceFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            IEnumerable<FirstFlight> flights;
            try
            {
                var apiResponse = await _firstFlightService.GetFlights(cancellationToken, date, maxPrice, maxTransfersCount);
                flights = apiResponse.Content != null ? apiResponse.Content : new List<FirstFlight>();
            }
            catch (OperationCanceledException)
            {
                flights = new List<FirstFlight>();
            }
            return flights;
        }
        private async Task<IEnumerable<SecondFlight>> GetSecondServiceFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            IEnumerable<SecondFlight> flights;
            try
            {
                var apiResponse = await _secondFlightService.GetFlights(cancellationToken, date, sortProperty, maxPrice);
                flights = apiResponse.Content != null ? apiResponse.Content :  new List<SecondFlight>();
                flights = flights.Where(f => f.Transfres.Length < maxTransfersCount);
            }
            catch (OperationCanceledException)
            {
                flights = new List<SecondFlight>();
            }
            return flights;
        }
       
    }
}
