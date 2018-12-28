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
            if (!repository.TryReadIconId(user.Id, out string iconId))
                return next.ReadIcon(user);

            var parameters = new Dictionary<string, string>
            {
                { "iconId", iconId }
            };
            return new Icon(urlTemplate.BindByName(baseUrl, parameters));
        }

        private readonly Uri baseUrl = new Uri("https://example.com");
        private readonly UriTemplate urlTemplate = new UriTemplate("users/{iconId}/icon");
    }
}
