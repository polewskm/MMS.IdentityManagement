using System;
using System.Buffers.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Models;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientValidator
    {
        Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default);
    }

    public interface IClientRepository
    {
        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);
    }

    public class ClientValidator : IClientValidator
    {
        private readonly IClientRepository _repository;
        private readonly ISecretProtector _secretProtector;

        public ClientValidator(IClientRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        private static ClientValidationResult Invalid(string description) => Error("invalid_request", description);

        private static ClientValidationResult Unauthorized(string description) => Error("unauthorized_client", description);

        private static ClientValidationResult Error(string error, string description)
        {
            return new ClientValidationResult
            {
                Error = error,
                ErrorDescription = description,
            };
        }

        public virtual async Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request?.ClientId))
                return Invalid("Invalid client_id");

            var client = await _repository.GetClientByIdAsync(request.ClientId, cancellationToken).ConfigureAwait(false);
            if (client == null)
                return Invalid("Unknown client_id");

            if (client.Disabled)
                return Unauthorized("Disabled client");

            if (client.RequireSecret)
            {
                if (string.IsNullOrEmpty(request.ClientSecret))
                    return Unauthorized("Missing client_secret");

                var secret = client.Secrets.FirstOrDefault(_ => _secretProtector.Verify(request.ClientSecret, _.CipherValue));
            }

            throw new NotImplementedException();
        }

    }

    public static class HashTypes
    {
        public const string Argon2 = "argon2";
        public const string Pbkdf2 = "pbkdf2";
        public const string BCrypt = "bcrypt";

        // Iterations = 4
        // MemorySize = 64 * 1024
        // DegreeOfParallelism = 4
        // SaltLength = 16
        // OutputLength = 32
    }

    // ${algorithm}${version}${m=0,t=0,p=0}${salt}${hash}
    // $argon2id$v=19$m=65536,t=2,p=2$c29tZXNhbHQ$RdescudvJCsgt3ub+b+dWRWJTmaaJObG

    public interface ISecretProtector
    {
        string Protect(string plainText);

        bool Verify(string plainText, string cipherText);
    }

    public class SecretProtectorHMAC : ISecretProtector
    {
        // https://github.com/OWASP/CheatSheetSeries/blob/master/cheatsheets/Password_Storage_Cheat_Sheet.md

        public string Protect(string plainText)
        {
            var keyBytes = Array.Empty<byte>(); // TODO
            var textBytes = Encoding.UTF7.GetBytes(plainText);

            using (var rng = RandomNumberGenerator.Create())
            using (var hasher = new HMACSHA256(keyBytes))
            {
                var saltBytes = new byte[32];
                rng.GetNonZeroBytes(saltBytes);

                var combinedBytes = new byte[saltBytes.Length + textBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(textBytes, 0, combinedBytes, saltBytes.Length, textBytes.Length);

                var hashBytes = hasher.ComputeHash(combinedBytes);

                var saltBase64 = Convert.ToBase64String(saltBytes);
                var hashBase64 = Convert.ToBase64String(hashBytes);

                return "hmac$256$" + saltBase64 + "$" + hashBase64;
            }
        }

        public bool Verify(string plainText, string cipherText)
        {
            var parts = cipherText.Split("$");
            if (parts.Length != 4 || parts[0] != "hmac" || parts[1] != "256")
                return false;

            var saltBase64 = parts[2];
            var expectedBase64 = parts[3];

            var keyBytes = Array.Empty<byte>();
            var textBytes = Encoding.UTF7.GetBytes(plainText);

            var saltBytes = Convert.FromBase64String(saltBase64);
            var expectedBytes = Convert.FromBase64String(expectedBase64);

            using (var hasher = new HMACSHA256(keyBytes))
            {
                var combinedBytes = new byte[saltBytes.Length + textBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(textBytes, 0, combinedBytes, saltBytes.Length, textBytes.Length);

                var actualBytes = hasher.ComputeHash(combinedBytes);

                return BuffersAreEqual(expectedBytes, actualBytes);
            }

        }

        private static bool BuffersAreEqual(byte[] buffer1, byte[] buffer2)
        {
            return false;
        }
    }

}