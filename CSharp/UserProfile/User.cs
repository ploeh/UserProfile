using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public sealed class User
    {
        public User(int id, string name, string email, bool useGravatar, bool useIdenticon)
        {
            Id = id;
            Name = name;
            Email = email;
            UseGravatar = useGravatar;
            UseIdenticon = useIdenticon;
        }

        public int Id { get; }
        public string Name { get; }
        public string Email { get; }
        public bool UseGravatar { get; }
        public bool UseIdenticon { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is User other))
                return false;

            return Equals(Id, other.Id)
                && Equals(Name, other.Name)
                && Equals(Email, other.Email)
                && Equals(UseGravatar, other.UseGravatar)
                && Equals(UseIdenticon, other.UseIdenticon);
        }

        public override int GetHashCode()
        {
            return
                Id.GetHashCode() ^
                Name.GetHashCode() ^
                Email.GetHashCode() ^
                UseGravatar.GetHashCode() ^
                UseIdenticon.GetHashCode();
        }
    }
}
