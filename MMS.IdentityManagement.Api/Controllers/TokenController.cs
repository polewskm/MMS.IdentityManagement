using System.ComponentModel.DataAnnotations;
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
        public IActionResult KeyCodeAuth([FromBody, Required] KeyCodeAuthenticationRequest request)
        {
            if (request == null)
                return BadRequest();

            return Ok();
        }

    }
}