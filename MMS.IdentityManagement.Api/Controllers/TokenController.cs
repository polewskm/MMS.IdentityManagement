using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MMS.IdentityManagement.Api.Controllers
{
    // POST /api/v1/tokens/keycode
    // POST /api/v1/tokens/refresh

    [Route("api/v1/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        [Route("keycode")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public IActionResult KeyCode([FromBody, Required] KeyCodeAuthenticationRequest request)
        {
            if (request == null)
                return BadRequest();

            throw new NotImplementedException();

            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public IActionResult Refresh([FromBody, Required] TokenRefreshRequest request)
        {
            if (request == null)
                return BadRequest();

            return StatusCode(StatusCodes.Status501NotImplemented);
        }

    }
}