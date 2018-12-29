using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public class CompositeIconReader : IIconReader
    {
        private readonly IIconReader[] iconReaders;

        public CompositeIconReader(params IIconReader[] iconReaders)
        {
            this.iconReaders = iconReaders;
        }

        public Maybe<Icon> ReadIcon(User user)
        {
            foreach (var iconReader in iconReaders)
            {
                var mIcon = iconReader.ReadIcon(user);
                if (IsPopulated(mIcon))
                    return mIcon;
            }

            return new Maybe<Icon>();
        }

        private static bool IsPopulated<T>(Maybe<T> m)
        {
            return m.Aggregate(false, _ => true);
        }
    }
}
