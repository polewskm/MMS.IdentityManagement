using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public class SecretProtectorPbkdf2Sha256Options
    {
        public int IterationCount { get; set; } = 10000;
        public int SaltLength { get; set; } = 128 / 8; // default 16 bytes
        public int OutputLength { get; set; } = 256 / 8; // default 32 bytes
    }

    public class SecretProtectorPbkdf2Sha256 : SecretProtector
    {
        // https://github.com/aspnet/Identity/blob/rel/2.0.0/src/Microsoft.Extensions.Identity.Core/PasswordHasher.cs

        private readonly SecretProtectorPbkdf2Sha256Options _options;

        public SecretProtectorPbkdf2Sha256(IOptions<SecretProtectorPbkdf2Sha256Options> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public override string CipherType => CipherTypes.Pbkdf2Sha256;

        public override string Protect(string plainText)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var saltBytes = new byte[_options.SaltLength];
                rng.GetNonZeroBytes(saltBytes);

                const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;
                var hashBytes = KeyDerivation.Pbkdf2(plainText, saltBytes, prf, _options.IterationCount, _options.OutputLength);

                var parameters = new Dictionary<string, object>
                {
                    ["t"] = _options.IterationCount,
                };

                return StringFormat(parameters, saltBytes, hashBytes);
            }
        }

        public override bool Verify(string plainText, string cipherText)
        {
            if (plainText == null || string.IsNullOrEmpty(cipherText))
                return false;

            // $pbkdf2-sha256$t={iterationCount}${salt}${hash}
            var parts = cipherText.Split("$", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4 || parts[0] != CipherType)
                return false;

            var options = parts[1];
            var match = Regex.Match(options, @"t=(?<IterationCount>\d+)");
            if (!match.Success || !int.TryParse(match.Groups["IterationCount"].Value, out var iterationCount))
                return false;

            var saltBase64 = parts[2];
            var expectedBase64 = parts[3];

            var saltBytes = Convert.FromBase64String(saltBase64);
            var expectedBytes = Convert.FromBase64String(expectedBase64);
            var outputLength = expectedBytes.Length;

            const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;
            var actualBytes = KeyDerivation.Pbkdf2(plainText, saltBytes, prf, iterationCount, outputLength);

            return ByteArraysEqual(expectedBytes, actualBytes);
        }

    }
}