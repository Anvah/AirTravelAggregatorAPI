namespace AirTravelAggregatorAPI.Models.ResultModels
{
    public class AirTravelApiResponse
    {
        public ApiError Error { get; set; }
        public bool IsSuccess { get { return Error is null; }}
    }
    public class AirTravelApiResponse<T> : AirTravelApiResponse
    {
        public T Result { get; set; }
    }
}
