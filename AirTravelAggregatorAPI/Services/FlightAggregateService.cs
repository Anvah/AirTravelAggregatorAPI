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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AirTravelAggregatorAPI.Services
{
    public class FlightAggregateService: IFlightAggregatorService
    {
        private readonly IFirstFlightService _firstFlightService;
        private readonly ISecondFlightService _secondFlightService;
        private readonly IMapper _mapper;
        private readonly ILogger<FlightAggregateService> _logger;
        private IMemoryCache memoryCache;

        public FlightAggregateService(IFirstFlightService firstFlightService, ISecondFlightService secondFlightService, IMapper mapper, ILogger<FlightAggregateService> logger, IMemoryCache memoryCache)
        {
            _firstFlightService = firstFlightService;
            _secondFlightService = secondFlightService;
            _mapper = mapper;  
            _logger = logger;
            this.memoryCache = memoryCache;
        }
        public async Task<IEnumerable<Flight>> GetFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            _logger.LogInformation("start getting flights with params: date: {@date}, sort: {@sortProperty}, maxPrice: {@maxPrice}, airlineName: {@airlineName}, maxTransfersCount: {@maxTransfersCount}"
                ,date.Date, sortProperty.ToString(), maxPrice, airlineName, maxTransfersCount);
            IEnumerable<Flight> flights;
            if(memoryCache.TryGetValue(date, out flights))
            {
                _logger.LogInformation("data found in cache");
                if(flights == null)
                    return Enumerable.Empty<Flight>();
                flights.Where(f => f.Price < maxPrice
                        && f.Transfers.Length < maxTransfersCount);
            }
            else
            {
                _logger.LogInformation("data not found in cache, strating requests");
                var fristFlights = await GetFirstServiceFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);
                var secondFlights = await GetSecondServiceFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);
                flights = fristFlights.Select(f => _mapper.Map<FirstFlight, Flight>(f));
                flights = flights.Concat(secondFlights.Select(f => _mapper.Map<SecondFlight, Flight>(f)));
                memoryCache.Set(date, flights, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            }
            flights = flights
                .Where(f => f.Airline.Name.ToLower().Contains(airlineName.ToLower()))
                .OrderBy(f => sortProperty == SortProperty.ByPrice ? f.Price : f.Transfers.Count());
            _logger.LogInformation("fligts aggregate success");
            return flights;
        }
        public async Task<Flight> Book(CancellationToken cancellationToken, string originalId, FlightSourse sourse)
        {
            _logger.LogInformation("start booking flight from {@sourse} with id: {@originalId}", sourse.ToString(), originalId);
            switch (sourse)
            {
                case FlightSourse.FirstFlightService:
                    Flight flight = null;
                    try
                    {
                        var firstFlight = await _firstFlightService.Book(originalId, cancellationToken);
                        flight = _mapper.Map<FirstFlight, Flight>(firstFlight);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    return flight;
                        
                case FlightSourse.SecondFlightService:
                    var secondFlight = await _secondFlightService.Book(originalId, cancellationToken);
                    return _mapper.Map<SecondFlight, Flight>(secondFlight);
                default:
                    return null;
            }

        }
        private async Task<IEnumerable<FirstFlight>> GetFirstServiceFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            _logger.LogInformation("start getting flights from FirstFlightService");
            IEnumerable<FirstFlight> flights;
            try
            {
                var apiResponse = await _firstFlightService.GetFlights(cancellationToken, date, maxPrice, maxTransfersCount);
                flights = apiResponse.Content != null ? apiResponse.Content : new List<FirstFlight>();
                _logger.LogInformation("firstFligt service request success");
            }
            catch (OperationCanceledException)
            {
                flights = new List<FirstFlight>();
                _logger.LogWarning("firstFligt service request time out");
            }
            return flights;
        }
        private async Task<IEnumerable<SecondFlight>> GetSecondServiceFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            _logger.LogInformation("start getting flights from SecondFlightService");
            IEnumerable<SecondFlight> flights;
            try
            {
                var apiResponse = await _secondFlightService.GetFlights(cancellationToken, date, sortProperty, maxPrice);
                flights = apiResponse.Content != null ? apiResponse.Content :  new List<SecondFlight>();
                flights = flights.Where(f => f.Transfres.Length < maxTransfersCount);
                _logger.LogInformation("secondFligtService request success");
            }
            catch (OperationCanceledException)
            {
                flights = new List<SecondFlight>();
                _logger.LogWarning("secondFligtService request time out");
            }
            return flights;
        }
       
    }
}
