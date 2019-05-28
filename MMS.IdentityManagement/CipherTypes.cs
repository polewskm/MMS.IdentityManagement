namespace MMS.IdentityManagement
{
    public static class CipherTypes
    {
        public const string Argon2 = "argon2";
        public const string Pbkdf2Sha256 = "pbkdf2-sha256";
        public const string HmacSha256 = "hmac-sha256";
        public const string BCrypt = "bcrypt";
    }
}