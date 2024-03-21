using AirTravelAggregatorAPI.Models.ResultModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AirTravelAggregatorAPI.Controllers
{
    public abstract class ApiControllerBase : Controller
    {
        public AirTravelApiResponse<T> GetResponse<T>(T result)
        {
            var response = new AirTravelApiResponse<T>();
            if (result == null)
            {
                throw new KeyNotFoundException();
            }
            response.Result = result;
            return response;
        }
        public AirTravelApiResponse<IEnumerable<T>> GetResponse<T>(IEnumerable<T> result)
        {
            var response = new AirTravelApiResponse<IEnumerable<T>>();
            if (result.IsNullOrEmpty())
            {
                throw new KeyNotFoundException();
            }
            response.Result = result;
            return response;
        }
        public AirTravelApiResponse GetResponse()
        {
            var response = new AirTravelApiResponse();
            return response;
        }
    }
}
