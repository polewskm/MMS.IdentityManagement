using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public class SecretProtectorHmacSha256Options
    {
        public byte[] Key { get; set; }

        public int SaltLength { get; set; }
    }

    public class SecretProtectorHmac256 : SecretProtector
    {
        private readonly SecretProtectorHmacSha256Options _options;

        public SecretProtectorHmac256(IOptions<SecretProtectorHmacSha256Options> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public override string CipherType => CipherTypes.HmacSha256;

        public override string Protect(string plainText)
        {
            var textBytes = Encoding.UTF7.GetBytes(plainText);

            using (var rng = RandomNumberGenerator.Create())
            using (var hasher = new HMACSHA256(_options.Key))
            {
                var saltBytes = new byte[_options.SaltLength];
                rng.GetNonZeroBytes(saltBytes);

                var combinedBytes = new byte[saltBytes.Length + textBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(textBytes, 0, combinedBytes, saltBytes.Length, textBytes.Length);

                var hashBytes = hasher.ComputeHash(combinedBytes);

                return StringFormat(saltBytes, hashBytes);
            }
        }

        public override bool Verify(string plainText, string cipherText)
        {
            if (plainText == null || string.IsNullOrEmpty(cipherText))
                return false;

            // $hmac-sha256${salt}${hash}
            var parts = cipherText.Split("$", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3 || parts[0] != CipherType)
                return false;

            var saltBase64 = parts[1];
            var expectedBase64 = parts[2];

            var textBytes = Encoding.UTF7.GetBytes(plainText);

            var saltBytes = Convert.FromBase64String(saltBase64);
            var expectedBytes = Convert.FromBase64String(expectedBase64);

            using (var hasher = new HMACSHA256(_options.Key))
            {
                var combinedBytes = new byte[saltBytes.Length + textBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(textBytes, 0, combinedBytes, saltBytes.Length, textBytes.Length);

                var actualBytes = hasher.ComputeHash(combinedBytes);

                return ByteArraysEqual(expectedBytes, actualBytes);
            }
        }

    }
}