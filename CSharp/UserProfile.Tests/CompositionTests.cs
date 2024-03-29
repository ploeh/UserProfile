﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ploeh.Samples.UserProfile.Tests
{
    public class CompositionTests
    {
        [Fact]
        public void Example()
        {
            var repo = new FakeUserRepository();
            FillWithTestValues(repo);
            var reader = new IconReaderFacade(repo);

            // The question corresponding to 42 is 'life, the universe, and
            // everything'. The Scandinavian first name Leif is pronounced like
            // the English 'life', and just so also happens to be the name of
            // my father :)
            var user = new User(42, "Leif Seemann", "leif@example.com", false, false);
            Icon icon = reader.ReadIcon(user);

            var expected = new Icon(new Uri("https://example.com/users/42/icon"));
            Assert.Equal(expected, icon);
        }

        [Fact]
        public void ReadGravatar()
        {
            var repo = new FakeUserRepository();
            FillWithTestValues(repo);
            var observations = new List<int>();
            var sut = ComposeWithTracing(repo, observations);

            var user = new User(42, "Foo", "foo@example.com", true, true);
            var actual = sut(user);

            var expected = new Icon(new Uri("https://www.gravatar.com/avatar/b48def645758b95537d4424c84d1a9ff"));
            Assert.Equal(expected, actual);
            Assert.Equal(new List<int> { 0 }, observations);
        }

        [Fact]
        public void ReadIdenticon()
        {
            var repo = new FakeUserRepository();
            FillWithTestValues(repo);
            var observations = new List<int>();
            var sut = ComposeWithTracing(repo, observations);

            var user = new User(42, "Bar", "bar@example.org", false, true);
            var actual = sut(user);

            var expected = new Icon(new Uri("https://example.com/identicon/baf7c704bc2907f2b43fdd462f850336"));
            Assert.Equal(expected, actual);
            Assert.Equal(new List<int> { 0, 1 }, observations);
        }

        [Fact]
        public void ReadDBIcon()
        {
            var repo = new FakeUserRepository();
            FillWithTestValues(repo);
            var observations = new List<int>();
            var sut = ComposeWithTracing(repo, observations);

            var user = new User(42, "Baz", "baz@example.net", false, false);
            var actual = sut(user);

            var expected = new Icon(new Uri("https://example.com/users/42/icon"));
            Assert.Equal(expected, actual);
            Assert.Equal(new List<int> { 0, 1, 2 }, observations);
        }

        [Fact]
        public void ReadAbsentIcon()
        {
            var repo = new FakeUserRepository();
            FillWithTestValues(repo);
            var observations = new List<int>();
            var sut = ComposeWithTracing(repo, observations);

            // 40 isn't in the 'database'
            var user = new User(40, "Qux", "qux@example.com", false, false);
            var actual = sut(user);

            var expected = new Icon(new Uri("https://example.com/default-icon"));
            Assert.Equal(expected, actual);
            Assert.Equal(new List<int> { 0, 1, 2, 3 }, observations);
        }

        private void FillWithTestValues(FakeUserRepository repo)
        {
            repo.Add(42, "42");
            repo.Add(90125, "90125");
            repo.Add(666, "666");
        }

        private Func<User, Icon> ComposeWithTracing(IUserRepository repository, ICollection<int> observations)
        {
            var readers = new IIconReader[]
            {
                new TraceIconReader<int>(0, observations, new GravatarReader()),
                new TraceIconReader<int>(1, observations, new IdenticonReader()),
                new TraceIconReader<int>(2, observations, new DBIconReader(repository)),
                new TraceIconReader<int>(3, observations, new NullIconReader())
            };
            return u =>
            {
                var lazyIcons = readers
                    .Select(r =>
                        new Lazy<Maybe<First<Icon>>>(() =>
                            r.ReadIcon(u).Select(i => new First<Icon>(i))));
                var m = lazyIcons.FindFirst();
                return m.Value.Aggregate(Icon.Default, fi => fi.Item);
            };
        }
    }
}
