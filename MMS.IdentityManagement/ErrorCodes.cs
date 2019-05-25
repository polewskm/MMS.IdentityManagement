namespace MMS.IdentityManagement
{
    public static class ErrorCodes
    {
        public const string InvalidRequest = "invalid_request";
        public const string InvalidClient = "invalid_client";
        public const string InvalidGrant = "invalid_grant";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string UnsupportedGrantType = "unsupported_grant_type";
        public const string UnsupportedResponseType = "unsupported_response_type";
        public const string AccessDenied = "access_denied";
        public const string ExpiredToken = "expired_token";
    }
}