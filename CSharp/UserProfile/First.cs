using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public sealed class First<T>
    {
        public First(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Item = item;
        }

        public T Item { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is First<T> other))
                return false;

            return Equals(Item, other.Item);
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }
    }
}
