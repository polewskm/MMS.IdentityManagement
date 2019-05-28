namespace MMS.IdentityManagement.Api.Data
{
    public class MemberKeyCode
    {
        public int MemberId { get; set; }

        public Secret Secret { get; set; }
    }
}