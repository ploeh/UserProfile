using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile.Tests
{
    public class FakeUserRepository : Dictionary<int, string>, IUserRepository
    {
        public Maybe<string> ReadIconId(int userId)
        {
            if (TryGetValue(userId, out var iconId))
                return new Maybe<string>(iconId);

            return new Maybe<string>();
        }
    }
}
