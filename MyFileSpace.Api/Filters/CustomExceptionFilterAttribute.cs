using Microsoft.AspNetCore.Mvc.Filters;

namespace MyFileSpace.Api.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public CustomExceptionFilterAttribute()
        {
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            OnException(context);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            context.Result = context.Exception.HandleResult();
        }
    }
}
