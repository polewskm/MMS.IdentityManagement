using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public static class CommonResultExtensions
    {
        public static ProblemDetails AsProblemDetails(this CommonResult result, int? status = StatusCodes.Status400BadRequest)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (result.Success)
                throw new InvalidOperationException();

            return new ProblemDetails
            {
                Status = status,
                Title = result.Error,
                Detail = result.ErrorDescription,
            };
        }

    }
}