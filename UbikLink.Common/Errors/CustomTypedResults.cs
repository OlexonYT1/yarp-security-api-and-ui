using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace UbikLink.Common.Errors
{
    public static class CustomTypedResults
    {
        public static ProblemHttpResult Problem(IDictionary<string, string[]> validationErrors)
        {
            ArgumentNullException.ThrowIfNull(validationErrors);
            
            var problemDetails = new ProblemDetails()
            {
                Detail = "Validation error",
                Status = 400,
                Title = "Validation error",
                Extensions = { { "validationErrors", validationErrors } }
            };

            return TypedResults.Problem(problemDetails);
        }

        public static ProblemHttpResult Problem(IFeatureError featureError)
        {
            ArgumentNullException.ThrowIfNull(featureError);

            var problemDetails = new ProblemDetails()
            {
                Detail = featureError.Details,
                Status = (int)featureError.ErrorType,
                Title = "Service - feature error",
                Extensions = { { "errors", featureError.CustomErrors } }
            };

            return TypedResults.Problem(problemDetails);
        }

    }
}
