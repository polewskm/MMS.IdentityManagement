using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MMS.IdentityManagement.Api.Data
{
    public interface IMemberRepository
    {
        Task<Member> GetMemberByIdAsync(int memberId, CancellationToken cancellationToken = default);

        Task<IEnumerable<MemberKeyCode>> LoadMemberKeyCodesAsync(CancellationToken cancellationToken = default);
    }

    public class MemberRepository : IMemberRepository
    {
        public virtual Task<Member> GetMemberByIdAsync(int memberId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<MemberKeyCode>> LoadMemberKeyCodesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}