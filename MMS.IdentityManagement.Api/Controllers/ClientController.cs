using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MMS.IdentityManagement.Api.Services;

namespace MMS.IdentityManagement.Api.Controllers
{
    [Authorize]
    [Route("api/v1/clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ClientReference>))]
        public virtual async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var clients = await _clientService.GetClientsAsync(cancellationToken).ConfigureAwait(false);
            return Ok(clients);
        }

        [HttpGet]
        [Route("{clientId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Client))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public virtual async Task<IActionResult> GetById(string clientId, CancellationToken cancellationToken = default)
        {
            var client = await _clientService.GetClientByIdAsync(clientId, cancellationToken).ConfigureAwait(false);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

    }
}