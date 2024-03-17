using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using Refit;

namespace AirTravelAggregatorAPI.Services.Interfaces
{
    public interface ISecondFlightService
    {
        [Get("/getFlights")]
        Task<ApiResponse<IEnumerable<SecondFlight>>> GetFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue);
        [Get("/bookFlight")]
        Task<SecondFlight> Book(string Id, CancellationToken cancellationToken);
    }
}
