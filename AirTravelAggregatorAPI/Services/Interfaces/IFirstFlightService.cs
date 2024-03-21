using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using Refit;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
