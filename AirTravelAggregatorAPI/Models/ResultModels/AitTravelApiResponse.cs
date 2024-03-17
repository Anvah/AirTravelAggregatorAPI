namespace AirTravelAggregatorAPI.Models.ResultModels
{
    public class AitTravelApiResponse
    {
        public ApiError Error { get; set; }
        public bool IsSuccess { get { return Error is null; }}
    }
    public class ApiResponse<T> : AitTravelApiResponse
    {
        public T Result { get; set; }
    }
}
