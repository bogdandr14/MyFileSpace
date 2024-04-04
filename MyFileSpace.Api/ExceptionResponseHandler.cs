using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Filters;
using MyFileSpace.SharedKernel.Exceptions;

namespace MyFileSpace.Api
{
    public static class ExceptionResponseHandler
    {
        public static ObjectResult HandleObjectResult<T>(this T exception) where T : Exception
        {
            switch (exception)
            {
                case UnauthorizedException:
                    return new UnauthorizedObjectResult(exception);
                case InvalidException:
                    return new BadRequestObjectResult(exception);
                case NotFoundException:
                    return new NotFoundObjectResult(exception);
                case ForbiddenException:
                    return new ObjectResult(exception) { StatusCode = StatusCodes.Status403Forbidden };
                default:
                    return new ObjectResult(exception) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        internal static ErrorModel HandleErrorResult<T>(this T exception) where T : Exception
        {
            switch (exception)
            {
                case UnauthorizedException:
                    return new ErrorModel(StatusCodes.Status401Unauthorized, exception);
                case InvalidException:
                    return new ErrorModel(StatusCodes.Status400BadRequest, exception);
                case NotFoundException:
                    return new ErrorModel(StatusCodes.Status404NotFound, exception);
                case ForbiddenException:
                    return new ErrorModel(StatusCodes.Status403Forbidden, exception);
                default:
                    return new ErrorModel(StatusCodes.Status500InternalServerError, exception);
            }
        }

        public static ActionResult HandleResult<T>(this T exception) where T : Exception
        {
            ErrorModel errorModel = exception.HandleErrorResult();
            //TODO add logging

            return new JsonResult(errorModel) { StatusCode = errorModel.StatusCode };
        }
    }
}
