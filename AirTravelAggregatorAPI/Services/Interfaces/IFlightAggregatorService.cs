using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.Enums;

namespace AirTravelAggregatorAPI.Services.Interfaces
{
    public interface IFlightAggregatorService
    {
        Task<IEnumerable<Flight>> GetFlights(CancellationToken cancellationToken, DateTime date, bool onlyNotBooked = true, SortProperty sortProperty = SortProperty.ByPrice, decimal maxPrice = decimal.MaxValue, string airlineName = "", int maxTransfersCount = int.MaxValue);
        Task<Flight> Book(string originalId, FlightSourse sourse, CancellationToken cancellationToken);
    }
}
