﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public class SecretProtectorHmac256 : ISecretProtector
    {
        // https://github.com/OWASP/CheatSheetSeries/blob/master/cheatsheets/Password_Storage_Cheat_Sheet.md

        // $hmac$256${salt}${combined_hash}

        public virtual string Protect(string plainText)
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

                return "$hmac$256$" + saltBase64 + "$" + hashBase64;
            }
        }

        public virtual bool Verify(string plainText, string cipherText)
        {
            if (plainText == null || string.IsNullOrEmpty(cipherText))
                return false;

            var parts = cipherText.Split("$", StringSplitOptions.RemoveEmptyEntries);
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
            if (ReferenceEquals(buffer1, buffer2))
                return true;

            if (buffer1 == null || buffer2 == null)
                return false;

            if (buffer1.Length != buffer2.Length)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < buffer1.Length; ++index)
            {
                if (buffer1[index] != buffer2[index])
                    return false;
            }
            
            return true;
        }

    }
}