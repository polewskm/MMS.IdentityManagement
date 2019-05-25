namespace MMS.IdentityManagement
{
    public static class CipherTypes
    {
        // ${algorithm}${version}${m=0,t=0,p=0}${salt}${hash}
        // $argon2id$v=19$m=65536,t=2,p=2$c29tZXNhbHQ$RdescudvJCsgt3ub+b+dWRWJTmaaJObG

        public const string Argon2 = "argon2";
        public const string Pbkdf2 = "pbkdf2";
        public const string BCrypt = "bcrypt";
        public const string Hmac256 = "hmac256";

        // Iterations = 4
        // MemorySize = 64 * 1024
        // DegreeOfParallelism = 4
        // SaltLength = 16
        // OutputLength = 32
    }
}