using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public interface IIconReader
    {
        Maybe<Icon> ReadIcon(User user);
    }
}
