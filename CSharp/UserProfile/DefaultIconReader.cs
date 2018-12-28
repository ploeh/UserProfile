using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public class DefaultIconReader : IIconReader
    {
        public Icon ReadIcon(User user)
        {
            return Icon.Default;
        }
    }
}
