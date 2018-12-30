using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile.Tests
{
    public class IconReaderFacade
    {
        private readonly IReadOnlyCollection<IIconReader> readers;

        public IconReaderFacade(IUserRepository repository)
        {
            readers = new IIconReader[]
                {
                    new GravatarReader(),
                    new IdenticonReader(),
                    new DBIconReader(repository)
                };
        }

        public Icon ReadIcon(User user)
        {
            IEnumerable<Lazy<Maybe<First<Icon>>>> lazyIcons = readers
                .Select(r =>
                    new Lazy<Maybe<First<Icon>>>(() =>
                        r.ReadIcon(user).Select(i => new First<Icon>(i))));
            Lazy<Maybe<First<Icon>>> m = lazyIcons.FindFirst();
            return m.Value.Aggregate(Icon.Default, fi => fi.Item);
        }
    }
}
