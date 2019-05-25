namespace MMS.IdentityManagement.Api.Data
{
    public class MemberKeyCode
    {
        public int MemberId { get; set; }

        public string CipherType { get; set; }

        public string CipherValue { get; set; }
    }
}