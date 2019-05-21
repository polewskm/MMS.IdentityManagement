using System;

namespace MMS.IdentityManagement.Validation
{
    public static class CommonResultExtensions
    {
        public static T AsError<T>(this CommonResult result)
            where T : CommonResult, new()
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (result.Success)
                throw new InvalidOperationException();

            return new T
            {
                Error = result.Error,
                ErrorDescription = result.ErrorDescription,
                Exception = result.Exception,
            };
        }

    }
}