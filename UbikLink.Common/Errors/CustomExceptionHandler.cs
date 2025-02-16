using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UbikLink.Common.Errors
{
    public class CustomExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<CustomExceptionHandler> log) : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _log = log;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _log.LogError(exception, "An error occurred: {message}",exception.Message);

            var problemDetails = new ProblemDetails();

            if (exception is IFeatureError problemDetailsException)
            {
                problemDetails = CustomTypedResults.Problem(problemDetailsException).ProblemDetails;
                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            }
            else
            {
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An internal server error occurred",
                    Type = exception.GetType().Name,
                    Detail = exception.Message,
                };
            }

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                Exception = exception is IFeatureError ? null : exception,
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
        }
    }

}
