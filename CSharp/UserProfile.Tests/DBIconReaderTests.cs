using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ploeh.Samples.UserProfile.Tests
{
    public class DBIconReaderTests
    {
        [Theory]
        [InlineData(  42, "Foo", "foo@example.com",  true,  true, "https://example.com/users/42/icon")]
        [InlineData(1337, "Bar", "bar@example.org", false, false, "https://example.com/users/1337/icon")]
        [InlineData(2112, "Baz", "baz@example.net",  true, false, "https://example.com/users/2112/icon")]
        public void ReadPresentIcon(
            int id,
            string name,
            string email,
            bool useGravatar,
            bool useIdenticon,
            string expected)
        {
            var repo = new FakeUserRepository
            {
                { 99, "other junk" },
                { id, id.ToString() },
                { 10, "more stuff" }
            };
            var sut = new DBIconReader(repo, new DefaultIconReader());

            var user = new User(id, name, email, useGravatar, useIdenticon);
            var actual = sut.ReadIcon(user);

            Assert.Equal(expected, actual.Url.ToString());
        }

        [Theory]
        [InlineData(  42, "Foo", "foo@example.com",  true,  true)]
        [InlineData(1337, "Bar", "bar@example.org", false, false)]
        [InlineData(2112, "Baz", "baz@example.net",  true, false)]
        public void ReadAbsentIcon(
            int id,
            string name,
            string email,
            bool useGravatar,
            bool useIdenticon)
        {
            var repo = new FakeUserRepository
            {
                { 99, "other junk" },
                { 10, "more stuff" }
            };
            var sut = new DBIconReader(repo, new DefaultIconReader());

            var user = new User(id, name, email, useGravatar, useIdenticon);
            var actual = sut.ReadIcon(user);

            Assert.Equal(Icon.Default, actual);
        }
    }
}
