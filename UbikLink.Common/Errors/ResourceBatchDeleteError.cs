using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.Errors
{
    public record ResourceBatchDeleteError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public ResourceBatchDeleteError(string resourceName, IEnumerable<Guid> missingIds)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"{resourceName}_BAD_IDS",
                ErrorFriendlyMessage = $"The ids are not present.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "Ids", string.Join(", ", missingIds) }
                }
            }];
        }
    }
}
