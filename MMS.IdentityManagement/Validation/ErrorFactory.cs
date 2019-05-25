using System;

namespace MMS.IdentityManagement.Validation
{
    public interface IErrorFactory<out T>
        where T : CommonResult, new()
    {
        T Clone(CommonResult other);

        T Error(string error, string description);

        T Error(string error, Exception exception);
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

            return new T
            {
                Error = other.Error,
                ErrorDescription = other.ErrorDescription,
                Exception = other.Exception,
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

        public virtual T Error(string error, Exception exception)
        {
            return new T
            {
                Error = error,
                Exception = exception,
            };
        }

    }
}