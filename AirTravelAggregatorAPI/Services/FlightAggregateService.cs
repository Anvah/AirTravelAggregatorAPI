using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using AirTravelAggregatorAPI.Services.Interfaces;
using MapsterMapper;
using AirTravelAggregatorAPI.Models.Enums;
using Microsoft.Extensions.Caching.Memory;

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
        /// <summary>
        /// Аггрегация данных о билетах из разных источников и кэширование результата
        /// </summary>
        ///<remarks>
        /// Для более эффективного использования кэша лучше отправлять в сервисы запросы без фильтрации.
        /// Пользователи часто могут запрашивать данные о билетах на одни и те же даты с разными фильтрами и сортировкой
        /// соответственно в данной ситуации кэшировать лучше данные, где ключом будет дата перелета,
        /// а фильтрации и сортировки выполнять на стороне приложения
        /// </remarks>
        /// <param name="date">Дата вылета</param>
        /// <param name="sortProperty">Свойство, по которому будет сортироваться объект</param>
        /// <param name="maxPrice">Максимальная цена</param>
        /// <param name="airlineName">Название авиалинии перевозчика</param>
        /// <param name="maxTransfersCount">Максимальное количество пересадок</param>
        /// <returns></returns>
        public async Task<IEnumerable<Flight>> GetFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            _logger.LogInformation("start getting flights with params: date: {@date}, sort: {@sortProperty}, maxPrice: {@maxPrice}, airlineName: {@airlineName}, maxTransfersCount: {@maxTransfersCount}"
                ,date.Date, sortProperty.ToString(), maxPrice, airlineName, maxTransfersCount);
            IEnumerable<Flight> flights;
            if(memoryCache.TryGetValue(date, out flights))
            {
                _logger.LogInformation("data found in cache");
                if(flights == null)
                    return Enumerable.Empty<Flight>();
            }
            else
            {
                _logger.LogInformation("data not found in cache, starting requests");
                var fristFlights = await GetFirstServiceFlights(cancellationToken, date);
                var secondFlights = await GetSecondServiceFlights(cancellationToken, date);
                flights = fristFlights.Select(f => _mapper.Map<FirstFlight, Flight>(f));
                flights = flights.Concat(secondFlights.Select(f => _mapper.Map<SecondFlight, Flight>(f)));
                memoryCache.Set(date, flights, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            }
            flights = flights
                .Where(f => f.Price < maxPrice
                        && f.Transfers.Length < maxTransfersCount)
                .Where(f => f.Airline.Name.ToLower().Contains(airlineName.ToLower()))
                .OrderBy(f => sortProperty == SortProperty.ByPrice ? f.Price : f.Transfers.Count());
            _logger.LogInformation("fligts aggregate success");
            return flights;
        }
        /// <summary>
        /// Бронирование билета
        /// </summary>
        ///<remarks>
        /// Для бранирование в зависимотсти от парамтра sourse
        /// Отправляется запрос в соответствующий источник
        /// </remarks>
        /// <param name="originalId">Id билета из оригинального источника</param>
        /// <param name="sourse">Источник, из которого получен билет</param>
        /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns></returns>
        public async Task<Flight> Book(string originalId, FlightSourse sourse, CancellationToken cancellationToken)
        {
            _logger.LogInformation("start booking flight from {@sourse} with id: {@originalId}", sourse.ToString(), originalId);
            switch (sourse)
            {
                case FlightSourse.FirstFlightService:
                    var firstFlightResponse = await _firstFlightService.Book(originalId);
                    var firstFlight = firstFlightResponse.Content;
                    if (firstFlight == null)
                    {
                        return null;
                    }
                    return _mapper.Map<FirstFlight, Flight>(firstFlight);

                case FlightSourse.SecondFlightService:
                    var secondFlightResponse = await _secondFlightService.Book(originalId);
                    var secondFlight = secondFlightResponse.Content;
                    if (secondFlight == null)
                    {
                        return null;
                    }
                    return _mapper.Map<SecondFlight, Flight>(secondFlight);
                default:
                    return null;
            }

        }
        private async Task<IEnumerable<FirstFlight>> GetFirstServiceFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
        {
            _logger.LogInformation("start getting flights from FirstFlightService");
            IEnumerable<FirstFlight> flights;
            try
            {
                var apiResponse = await _firstFlightService.GetFlights(date, maxPrice, maxTransfersCount, cancellationToken);
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
        private async Task<IEnumerable<SecondFlight>> GetSecondServiceFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue)
{
            _logger.LogInformation("start getting flights from SecondFlightService");
            IEnumerable<SecondFlight> flights;
            try
            {
                var apiResponse = await _secondFlightService.GetFlights(date, sortProperty, maxPrice, cancellationToken);
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
