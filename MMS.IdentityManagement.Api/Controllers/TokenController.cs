using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MMS.IdentityManagement.Api.Extensions;
using MMS.IdentityManagement.Api.Services;
using MMS.IdentityManagement.Requests;

namespace MMS.IdentityManagement.Api.Controllers
{
    // POST /api/v1/tokens/keycode
    // POST /api/v1/tokens/refresh

    [Route("api/v1/tokens")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IKeyCodeAuthenticationHandler _keyCodeAuthenticationHandler;

        public TokenController(IKeyCodeAuthenticationHandler keyCodeAuthenticationHandler)
        {
            _keyCodeAuthenticationHandler = keyCodeAuthenticationHandler ?? throw new ArgumentNullException(nameof(keyCodeAuthenticationHandler));
        }

        [HttpPost]
        [Route("keycode")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KeyCodeAuthenticationResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public virtual async Task<IActionResult> KeyCode([FromBody, Required] KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest();

            var result = await _keyCodeAuthenticationHandler.AuthenticateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!result.Success)
                return BadRequest(result.AsProblemDetails());

            return Ok();
        }

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KeyCodeAuthenticationResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public virtual IActionResult Refresh([FromBody, Required] TokenRefreshRequest request)
        {
            if (request == null)
                return BadRequest();

            return StatusCode(StatusCodes.Status501NotImplemented);
        }

    }
}