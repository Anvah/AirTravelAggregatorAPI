using System.Text.Json.Serialization;

namespace AirTravelAggregatorAPI.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlightSource
    {
        FirstFlightService,
        SecondFlightService
    }
}
