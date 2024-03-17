using AirTravelAggregatorAPI.Models.ResultModels;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AirTravelAggregatorAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await Handle(httpContext, ex);
            }
        }
        public async Task Handle(HttpContext httpContext, Exception exception)
        {
            HttpStatusCode code;
            switch (exception)
            {
                case KeyNotFoundException
                    or FileNotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case ValidationException:
                    code = HttpStatusCode.InternalServerError;
                    break;
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case ArgumentException
                    or InvalidOperationException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case OperationCanceledException:
                    code = HttpStatusCode.RequestTimeout;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }
            httpContext.Response.StatusCode = (int)code;
            httpContext.Response.ContentType = "application/json";
            var apiResponse = new AitTravelApiResponse
            {
                Error = new ApiError
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = exception.Message,

                }

            };
            await httpContext.Response.WriteAsJsonAsync(apiResponse);
        }
    }
}

