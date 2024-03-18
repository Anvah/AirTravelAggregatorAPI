using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AirTravelAggregatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FightController : ControllerBase
    {
        private readonly IFlightAggregatorService _flightAggregatorService;
        public FightController(IFlightAggregatorService flightAggregatorService)
        {
            _flightAggregatorService = flightAggregatorService;
        }
        [Route("/getFligts")]
        [HttpGet]
        public async Task<IEnumerable<Flight>> GetFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            return await _flightAggregatorService.GetFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);
        }
        [Authorize]
        [Route("/bookFlight")]
        [HttpPost]
        public async Task<Flight> Book(CancellationToken cancellationToken, string originalId, FlightSourse sourse)
        {
            return await _flightAggregatorService.Book(cancellationToken, originalId, sourse);
        }

    }
}
