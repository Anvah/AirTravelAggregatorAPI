using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.ResultModels;
using AirTravelAggregatorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AirTravelAggregatorAPI.Controllers
{
    public class FlightController : ApiControllerBase
    {
        private readonly IFlightAggregatorService _flightAggregatorService;
        public FlightController(IFlightAggregatorService flightAggregatorService)
        {
            _flightAggregatorService = flightAggregatorService;
        }
        /// <summary>
        /// Получение билетов
        /// </summary>
        /// <param name="date">Дата вылета</param>
        /// <param name="sortProperty">Свойство, по которому будет сортироваться объект</param>
        /// <param name="maxPrice">Максимальная цена</param>
        /// <param name="airlineName">Название авиалинии перевозчика</param>
        /// <param name="maxTransfersCount">Максимальное количество пересадок</param>
        /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("/fligts/get")]
        public async Task<ActionResult<AirTravelApiResponse<IEnumerable<Flight>>>> GetFlights(DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue, CancellationToken cancellationToken = default)
        {
            var flights = await _flightAggregatorService.GetFlights(cancellationToken, date, sortProperty, maxPrice, airlineName, maxTransfersCount);
            return GetResponse(flights);
        }
        /// <summary>
        /// Бронирование билета
        /// </summary>
        /// <param name="originalId">Id билета из оригинального источника</param>
        /// <param name="sourse">Источник, из которого получен билет</param>
        /// /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/fligts/book/{originalId}/{sourse}")]
        public async Task<ActionResult<AirTravelApiResponse<Flight>>> Book(string originalId, FlightSourse sourse, CancellationToken cancellationToken)
        {
            var bookedFlight = await _flightAggregatorService.Book(originalId, sourse, cancellationToken);
            return GetResponse(bookedFlight);
        }

    }
}
