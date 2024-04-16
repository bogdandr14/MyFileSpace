using Microsoft.AspNetCore.Mvc;
using MyFileSpace.SharedKernel.Exceptions;
using Serilog;

namespace MyFileSpace.Api.Middlewares
{
    public class CustomExceptionHandlerMiddleware : IMiddleware
    {
        public CustomExceptionHandlerMiddleware()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = CreateProblemDetails(context, ex);
                context.Response.StatusCode = problemDetails.Status!.Value;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private ProblemDetails CreateProblemDetails(HttpContext context, Exception ex)
        {
            ProblemDetails problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Detail = ex.Message
            };

            switch (ex)
            {
                case UnauthorizedException:
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Status = (int)StatusCodes.Status401Unauthorized;
                    break;
                case InvalidException:
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    problemDetails.Title = "Bad request";
                    problemDetails.Status = (int)StatusCodes.Status400BadRequest;
                    break;
                case NotFoundException:
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    problemDetails.Title = "Not found";
                    problemDetails.Status = (int)StatusCodes.Status404NotFound;
                    break;
                case ForbiddenException:
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                    problemDetails.Title = "Forbidden";
                    problemDetails.Status = (int)StatusCodes.Status403Forbidden;
                    break;
                default:
                    var traceId = Guid.NewGuid();
                    problemDetails = new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Title = "Internal Server Error",
                        Status = (int)StatusCodes.Status500InternalServerError,
                        Instance = context.Request.Path,
                        Detail = $"Internal server error occured, traceId : {traceId}",
                    };
                    Log.Logger.Error($"Error occured while processing the request, TraceId : ${traceId}, Message : ${ex.Message}, StackTrace: ${ex.StackTrace}");
                    break;
            }

            if (problemDetails.Status.Value != StatusCodes.Status500InternalServerError)
            {
                Log.Logger.Debug($"{problemDetails.Title} error occured while processing the request {problemDetails.Instance}, Message : ${ex.Message}, StackTrace: ${ex.StackTrace}");
            }

            return problemDetails;
        }
    }
}
