namespace AirTravelAggregatorAPI.Models.AggregatedModels
{
    public class Destination
    {
        public string AirportName { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }

    }
}
