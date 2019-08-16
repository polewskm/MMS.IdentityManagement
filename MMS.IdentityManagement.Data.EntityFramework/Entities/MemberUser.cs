using Microsoft.AspNetCore.Identity;

namespace MMS.IdentityManagement.Data.EntityFramework.Entities
{
    public class MemberUser : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}