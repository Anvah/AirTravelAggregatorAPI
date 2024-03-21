using AirTravelAggregatorAPI.Models.FirstServiceModels;
using Refit;

namespace AirTravelAggregatorAPI.Services.Interfaces
{
    public interface IFirstFlightService
    {
        [Get("/getFlights")]
        Task<ApiResponse<IEnumerable<FirstFlight>>> GetFlights(DateTime date, decimal maxPrice = decimal.MaxValue, int maxTransfersCount = int.MaxValue, CancellationToken cancellationToken = default);
        [Post("/bookFlight/{id}")]
        Task<FirstFlight> Book(string id, CancellationToken cancellationToken = default);
    }
}
