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
        Task<ApiResponse<IEnumerable<FirstFlight>>> GetFlights(CancellationToken cancellationToken, DateTime date, decimal maxPrice = decimal.MaxValue, int maxTransfersCount = int.MaxValue);
        [Get("/bookFlight")]
        Task<FirstFlight> Book(string Id, CancellationToken cancellationToken);
    }
}
