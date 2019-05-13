using System;
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
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("keycode")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public virtual async Task<IActionResult> KeyCode([FromBody, Required] KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest();

            var result = await _tokenService.ValidateKeyCodeAsync(request, cancellationToken).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public virtual IActionResult Refresh([FromBody, Required] TokenRefreshRequest request)
        {
            if (request == null)
                return BadRequest();

            return StatusCode(StatusCodes.Status501NotImplemented);
        }

    }
}