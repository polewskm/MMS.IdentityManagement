﻿using System;
using System.Security.Claims;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public class CreateTokenResult : CommonResult
    {
        public ClaimsIdentity Identity { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public string AccessToken { get; set; }

        public DateTimeOffset AccessTokenExpiresWhen { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset RefreshTokenExpiresWhen { get; set; }
    }
}