using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MMS.IdentityManagement.Api.Services;

namespace MMS.IdentityManagement.Api.Controllers
{
    // POST /api/v1/tokens/keycode
    // POST /api/v1/tokens/refresh

    [Route("api/v1/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IClientValidator _clientValidator;
        private readonly IKeyCodeValidator _keyCodeValidator;

        public TokenController(IClientValidator clientValidator)
        {
            _clientValidator = clientValidator;
        }

        [HttpPost]
        [Route("keycode")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public virtual async Task<IActionResult> KeyCode([FromBody, Required] KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest();

            var clientValidationResult = await _clientValidator.ValidateClientAsync(request.ClientId, request.ClientSecret, cancellationToken).ConfigureAwait(false);
            if (!clientValidationResult.IsValid)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = clientValidationResult.Error,
                    Detail = clientValidationResult.ErrorDescription,
                };
                return BadRequest(problemDetails);
            }

            var keyCodeValidationResult = await _keyCodeValidator.ValidateKeyCodeAsync(request.KeyCode, cancellationToken).ConfigureAwait(false);
            if (!keyCodeValidationResult.IsValid)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = keyCodeValidationResult.Error,
                    Detail = keyCodeValidationResult.ErrorDescription,
                };
                return BadRequest(problemDetails);
            }

            return Ok();
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public virtual IActionResult Refresh([FromBody, Required] TokenRefreshRequest request)
        {
            if (request == null)
                return BadRequest();

            return StatusCode(StatusCodes.Status501NotImplemented);
        }

    }

}