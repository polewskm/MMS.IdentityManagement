namespace MMS.IdentityManagement.Validation
{
    public static class ErrorFactoryExtensions
    {
        public static T InvalidRequest<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.InvalidRequest, description);
        }

        public static T InvalidClient<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.InvalidClient, description);
        }

        public static T InvalidGrant<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.InvalidGrant, description);
        }

        public static T UnauthorizedClient<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.UnauthorizedClient, description);
        }

        public static T UnsupportedGrantType<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.UnsupportedGrantType, description);
        }

        public static T UnsupportedResponseType<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.UnsupportedResponseType, description);
        }

        public static T AccessDenied<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.AccessDenied, description);
        }

        public static T ExpiredToken<T>(this IErrorFactory<T> factory, string description)
            where T : CommonResult, new()
        {
            return factory.Error(ErrorCodes.ExpiredToken, description);
        }

    }
}