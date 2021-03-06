﻿
using System;

namespace MMS.IdentityManagement.Api.Models
{
    public class CreateTokenRequest
    {
        public string AuthenticationType { get; set; }

        public DateTimeOffset AuthenticationTime { get; set; }

        public Client Client { get; set; }

        public Member Member { get; set; }

        public string Nonce { get; set; }
    }
}