namespace AirTravelAggregatorAPI.Models.FirstServiceModels
{
    public class FirstFlight
    {
        public string Id { get; set; }
        public string Airline { get; set; }
        public FirstFlightTransfer ArrivalPoint { get; set; }
        public FirstFlightTransfer DeparturePoint  { get; set; }
        public FirstFlightTransfer[] Transfers { get; set; }
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }
    }
}
