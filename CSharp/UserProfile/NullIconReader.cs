using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public class NullIconReader : IIconReader
    {
        public Maybe<Icon> ReadIcon(User user)
        {
            return new Maybe<Icon>();
        }
    }
}
