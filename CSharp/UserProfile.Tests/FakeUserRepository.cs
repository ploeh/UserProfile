using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile.Tests
{
    public class FakeUserRepository : Dictionary<int, string>, IUserRepository
    {
        public bool TryReadIconId(int userId, out string iconId)
        {
            return TryGetValue(userId, out iconId);
        }
    }
}
