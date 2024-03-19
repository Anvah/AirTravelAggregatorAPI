using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.ResultModels;
using AirTravelAggregatorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AirTravelAggregatorAPI.Controllers
{
    public class FightController : ApiControllerBase
    {
        private readonly IFlightAggregatorService _flightAggregatorService;
        public FightController(IFlightAggregatorService flightAggregatorService)
        {
            _flightAggregatorService = flightAggregatorService;
        }
        
        [HttpGet]
        [Route("/fligts/get")]
        public async Task<ActionResult<AirTravelApiResponse<IEnumerable<Flight>>>> GetFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            var flights = await _flightAggregatorService.GetFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);
            return GetResponse(flights);
        }
        //[Authorize]
        [HttpPost]
        [Route("/fligts/book")]
        public async Task<ActionResult<AirTravelApiResponse<Flight>>> Book(CancellationToken cancellationToken, string originalId, FlightSourse sourse)
        {
            var bookedFlight = await _flightAggregatorService.Book(cancellationToken, originalId, sourse);
            return GetResponse(bookedFlight);
        }

    }
}
