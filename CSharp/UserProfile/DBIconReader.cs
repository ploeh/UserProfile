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

        public DBIconReader(IUserRepository repository)
        {
            this.repository = repository;
        }

        public Maybe<Icon> ReadIcon(User user)
        {
            return repository.ReadIconId(user.Id).Select(CreateIcon);
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
        private readonly UriTemplate.Core.UriTemplate urlTemplate = new UriTemplate.Core.UriTemplate("users/{iconId}/icon");
    }
}
