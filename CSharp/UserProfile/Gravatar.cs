using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public sealed class Gravatar
    {
        private readonly Lazy<Uri> url;

        public Gravatar(string email)
        {
            Email = email;
            url = new Lazy<Uri>(CreateUrl);
        }

        public string Email { get; }

        public Uri Url => url.Value;

        public override bool Equals(object obj)
        {
            if (!(obj is Gravatar other))
                return false;

            return Equals(Email, other.Email);
        }

        public override int GetHashCode()
        {
            return Email.GetHashCode();
        }

        private readonly static MD5 mD5 = MD5.Create();

        private Uri CreateUrl()
        {
            var inBytes = Encoding.UTF8.GetBytes(Email);
            var hash = mD5.ComputeHash(inBytes);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder =
                new StringBuilder("https://www.gravatar.com/avatar/");

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < hash.Length; i++)
                sBuilder.Append(hash[i].ToString("x2"));

            return new Uri(sBuilder.ToString());
        }
    }
}
