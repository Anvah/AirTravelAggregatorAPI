namespace AirTravelAggregatorAPI.Models.SecondServiceModels
{
    public class SecondFlight
    {
        public string Id { get; set; }
        public string Airline { get; set; }
        public string DepartureAirport { get; set; }
        public string DepartureCity { get; set; }
        public string DepartureCountry { get; set; }
        public string ArrivalAirport { get; set; }
        public string ArrivalCity { get; set; }
        public string ArrivalCountry { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string[] Transfres { get; set; }
        public string[] TransfersArivalDateTime{ get; set; }
        public string[] TransfersDepartureDateTime{ get; set; }
        public decimal Price { get; set; }
        public bool IsBaggageAvaible { get; set; }
        public decimal BaggagePrice { get; set; }
        public bool IsBooked { get; set; }
    }
}
