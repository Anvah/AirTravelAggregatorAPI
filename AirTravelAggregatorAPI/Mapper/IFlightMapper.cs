using AirTravelAggregatorAPI.Models.AggregatedModels;
using AirTravelAggregatorAPI.Models.FirstServiceModels;
using AirTravelAggregatorAPI.Models.SecondServiceModels;
using Mapster;

namespace AirTravelAggregatorAPI.Mapper
{
    [Mapper]
    public interface IFlightMapper
    {
        Flight MapTo(FirstFlight firstFlight);
        Flight MapTo(SecondFlight secondFlight);
    }
}
