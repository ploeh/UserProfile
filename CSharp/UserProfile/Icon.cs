using System;

namespace Ploeh.Samples.UserProfile
{
    public sealed class Icon
    {
        public readonly static Icon Default =
            new Icon(new Uri("https://example.com/default-icon"));

        public Icon(Uri url)
        {
            Url = url;
        }

        public Uri Url { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Icon other))
                return false;

            return Equals(Url, other.Url);
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }
    }
}