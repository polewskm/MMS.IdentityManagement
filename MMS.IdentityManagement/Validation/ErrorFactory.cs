using System;
using System.Collections.Generic;
using System.Linq;

namespace MMS.IdentityManagement.Validation
{
    public interface IErrorFactory<out T>
        where T : CommonResult, new()
    {
        T Clone(CommonResult other);

        T Error(string error, string description, IEnumerable<KeyValuePair<string, object>> extensions);

        T Error(string error, string description);

        T Error(string error);
    }

    public class ErrorFactory<T> : IErrorFactory<T>
        where T : CommonResult, new()
    {
        public static IErrorFactory<T> Instance { get; } = new ErrorFactory<T>();

        public virtual T Clone(CommonResult other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.Success)
                throw new InvalidOperationException();

            return Error(other.Error, other.ErrorDescription, other.Extensions);
        }

        public virtual T Error(string error, string description, IEnumerable<KeyValuePair<string, object>> extensions)
        {
            return new T
            {
                Error = error,
                ErrorDescription = description,
                Extensions = extensions.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal),
            };
        }

        public virtual T Error(string error, string description)
        {
            return new T
            {
                Error = error,
                ErrorDescription = description,
            };
        }

        public virtual T Error(string error)
        {
            return new T
            {
                Error = error,
            };
        }

    }
}