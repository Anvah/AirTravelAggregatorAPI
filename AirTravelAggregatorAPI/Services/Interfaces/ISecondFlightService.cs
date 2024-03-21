using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using Refit;

namespace AirTravelAggregatorAPI.Services.Interfaces
{
    public interface ISecondFlightService
    {
        [Get("/getFlights")]
        Task<ApiResponse<IEnumerable<SecondFlight>>> GetFlights(DateTime date, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, CancellationToken cancellationToken = default);
        [Post("/bookFlight/{id}")]
        Task<SecondFlight> Book(string id, CancellationToken cancellationToken = default);
    }
}
