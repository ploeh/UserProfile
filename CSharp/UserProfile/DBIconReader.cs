using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public class DBIconReader : IIconReader
    {
        private readonly IUserRepository repository;
        private readonly IIconReader next;

        public DBIconReader(IUserRepository repository, IIconReader next)
        {
            this.repository = repository;
            this.next = next;
        }

        public Icon ReadIcon(User user)
        {
            Maybe<string> mid = repository.ReadIconId(user.Id);
            Lazy<Icon> lazyResult = mid.Aggregate(
                @default: new Lazy<Icon>(() => next.ReadIcon(user)),
                func: id => new Lazy<Icon>(() => CreateIcon(id)));
            return lazyResult.Value;
        }

        private Icon CreateIcon(string id)
        {
            var parameters = new Dictionary<string, string>
            {
                { "iconId", id }
            };
            return new Icon(urlTemplate.BindByName(baseUrl, parameters));
        }

        private readonly Uri baseUrl = new Uri("https://example.com");
        private readonly UriTemplate urlTemplate = new UriTemplate("users/{iconId}/icon");
    }
}
