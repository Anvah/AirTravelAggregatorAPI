using AirTravelAggregatorAPI.Models.Enums;
using System.Text.Json.Serialization;

namespace AirTravelAggregatorAPI.Models.AggregatedModels
{
    public class Flight
    {
        public Guid Id { get; set; }
        public string OriginalId { get; set; }
        public Destination ArrivalPoint { get; set; }
        public Destination DeparturePoint { get; set; }
        public Destination[] Transfers { get; set; }
        public Airline Airline { get; set; }
        public decimal Price { get; set; }
        public Baggage Baggage { get; set; }
        public bool IsBooked { get; set; }
        public FlightSource Source { get; set; }

    }
}
