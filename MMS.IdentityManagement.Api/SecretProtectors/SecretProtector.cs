using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public interface ISecretProtector
    {
        // https://github.com/OWASP/CheatSheetSeries/blob/master/cheatsheets/Password_Storage_Cheat_Sheet.md

        // ${algorithm}${version}${m=0,t=0,p=0}${salt}${hash}
        // $argon2id$v=19$m=65536,t=2,p=2$c29tZXNhbHQ$RdescudvJCsgt3ub+b+dWRWJTmaaJObG

        string CipherType { get; }

        string Protect(string plainText);

        bool Verify(string plainText, string cipherText);
    }

    public abstract class SecretProtector : ISecretProtector
    {
        public abstract string CipherType { get; }

        public abstract string Protect(string plainText);

        public abstract bool Verify(string plainText, string cipherText);

        protected string StringFormat(byte[] saltBytes, byte[] hashBytes)
        {
            var parameters = Enumerable.Empty<KeyValuePair<string, object>>();
            return StringFormat(parameters, saltBytes, hashBytes);
        }

        protected string StringFormat(IEnumerable<KeyValuePair<string, object>> parameters, byte[] saltBytes, byte[] hashBytes)
        {
            // https://github.com/P-H-C/phc-string-format/blob/master/phc-sf-spec.md

            var builder = new StringBuilder();

            builder.Append('$');
            builder.Append(CipherType);

            var first = true;
            foreach (var (key, value) in parameters)
            {
                if (first)
                {
                    first = false;
                    builder.Append('$');
                }
                else
                {
                    builder.Append(',');
                }

                builder.Append(key);
                builder.Append('=');
                builder.Append(value);
            }

            builder.Append('$');
            var saltBase64 = Convert.ToBase64String(saltBytes);
            builder.Append(saltBase64);

            builder.Append('$');
            var hashBase64 = Convert.ToBase64String(hashBytes);
            builder.Append(hashBase64);

            return builder.ToString();
        }

        protected static bool ByteArraysEqual(byte[] array1, byte[] array2)
        {
            if (ReferenceEquals(array1, array2))
                return true;

            if (array1 == null || array2 == null || array1.Length != array2.Length)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < array1.Length; ++index)
            {
                if (array1[index] != array2[index])
                    return false;
            }

            return true;
        }

    }
}