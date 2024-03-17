using AirTravelAggregatorAPI.Models.Enums;

namespace AirTravelAggregatorAPI.Models.AggregatedModels
{
    public class Flight
    {
        public string Id { get; set; }
        public string OriginalId { get; set; }
        public Destination ArrivalPoint { get; set; }
        public Destination DeparturePoint { get; set; }
        public Destination[] Transfers { get; set; }
        public Airline Airline { get; set; }
        public decimal Price { get; set; }
        public Baggage Baggage { get; set; }
        public bool IsBooked { get; set; }
        public FlightSourse Sourse { get; set; }

    }
}
