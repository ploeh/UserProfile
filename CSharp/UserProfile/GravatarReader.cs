using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public class GravatarReader : IIconReader
    {
        private readonly IIconReader next;

        public GravatarReader(IIconReader next)
        {
            this.next = next;
        }

        public Icon ReadIcon(User user)
        {
            if (user.UseGravatar)
                return new Icon(new Gravatar(user.Email).Url);

            return next.ReadIcon(user);
        }
    }
}
