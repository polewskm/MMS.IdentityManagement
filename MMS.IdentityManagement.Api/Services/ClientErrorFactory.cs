using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MMS.IdentityManagement.Api.Services
{
    public class ClientErrorFactory : IClientErrorFactory
    {
        private readonly IProblemDetailsFactory _problemDetailsFactory;

        public ClientErrorFactory(IProblemDetailsFactory problemDetailsFactory)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
        }

        public virtual IActionResult GetClientError(ActionContext actionContext, IClientErrorActionResult clientError)
        {
            var problemDetails = _problemDetailsFactory.FromClientError(actionContext, clientError);

            ObjectResult result;
            switch (problemDetails.Status)
            {
                case StatusCodes.Status400BadRequest:
                    result = new BadRequestObjectResult(problemDetails);
                    break;

                case StatusCodes.Status401Unauthorized:
                    result = new UnauthorizedObjectResult(problemDetails);
                    break;

                case StatusCodes.Status404NotFound:
                    result = new NotFoundObjectResult(problemDetails);
                    break;

                case StatusCodes.Status409Conflict:
                    result = new ConflictObjectResult(problemDetails);
                    break;

                case StatusCodes.Status422UnprocessableEntity:
                    result = new UnprocessableEntityObjectResult(problemDetails);
                    break;

                default:
                    result = new ObjectResult(problemDetails)
                    {
                        StatusCode = problemDetails.Status,
                    };
                    break;
            }

            result.ContentTypes.Add("application/problem+json");
            result.ContentTypes.Add("application/problem+xml");

            return result;
        }

    }
}