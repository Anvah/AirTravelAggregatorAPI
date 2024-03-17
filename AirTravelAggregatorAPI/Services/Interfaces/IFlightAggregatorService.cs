using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;

namespace AirTravelAggregatorAPI.Services.Interfaces
{
    public interface IFlightAggregatorService
    {
        Task<IEnumerable<Flight>> GetFlights(CancellationToken cancellationToken, DateTime date, SortProperty sortProperty, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue);
        Task<Flight> Book(CancellationToken cancellationToken, string originalId, FlightSourse sourse);
    }
}
