using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public class GravatarReader : IIconReader
    {
        public Maybe<Icon> ReadIcon(User user)
        {
            if (user.UseGravatar)
                return new Maybe<Icon>(new Icon(new Gravatar(user.Email).Url));

            return new Maybe<Icon>();
        }
    }
}
