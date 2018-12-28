using Devcorner.NIdenticon;
using Devcorner.NIdenticon.BrushGenerators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public class IdenticonReader : IIconReader
    {
        private readonly IIconReader next;

        public IdenticonReader(IIconReader next)
        {
            this.next = next;
        }

        public Icon ReadIcon(User user)
        {
            if (user.UseIdenticon)
                return new Icon(new Uri(baseUrl, HashUser(user)));

            return next.ReadIcon(user);
        }

        private readonly static Uri baseUrl =
            new Uri("https://example.com/identicon/");

        private string HashUser(User user)
        {
            var sBuilder = new StringBuilder();
            sBuilder.Append(user.Id);
            sBuilder.Append(user.Name);
            sBuilder.Append(user.Email);

            var inBytes = Encoding.UTF8.GetBytes(sBuilder.ToString());
            var hash = mD5.ComputeHash(inBytes);

            sBuilder.Clear();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < hash.Length; i++)
                sBuilder.Append(hash[i].ToString("x2"));

            return sBuilder.ToString();
        }

        private readonly static MD5 mD5 = MD5.Create();

        // Here's what a server-side handler of the above identicon URL could
        // do to generate an identicon:
        public static Bitmap CreateIcon(string s)
        {
            var brushGenerator =
                new StaticColorBrushGenerator(StaticColorBrushGenerator.ColorFromText(s));
            return identiconGenerator.Value
                .WithBrushGenerator(brushGenerator)
                .Create(s);
        }

        private static readonly Lazy<IdenticonGenerator> identiconGenerator =
            new Lazy<IdenticonGenerator>(CreateIdenticonGenerator);

        private static IdenticonGenerator CreateIdenticonGenerator()
        {
            return new IdenticonGenerator().WithSize(80, 80).WithBlocks(5, 5);
        }
    }
}
