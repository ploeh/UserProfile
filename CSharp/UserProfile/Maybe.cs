using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile
{
    public sealed class Maybe<T>
    {
        private readonly bool hasItem;
        private readonly T item;

        public Maybe()
        {
            hasItem = false;
        }

        public Maybe(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            hasItem = true;
            this.item = item;
        }

        /// <summary>
        /// Transforms any <see cref="Maybe{T}" /> object, empty or populated,
        /// to a value of a uniform type.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="default">
        /// The value to use in case the <see cref="Maybe{T}" /> object is
        /// empty.
        /// </param>
        /// <param name="func">
        /// The function that gets invoked when the <see cref="Maybe{T}" />
        /// object is populated.
        /// </param>
        /// <returns>
        /// <paramref name="default" />, if the instance is empty; otherwise,
        /// the result of invoking <paramref name="func" /> with the contained
        /// item.
        /// </returns>
        /// <remarks>
        /// <para>
        /// While here called 'Aggregate', this is the catamorphism for Maybe.
        /// Other typical names for this operation are 'Fold', 'Cata', or, if
        /// Church-encoded, 'Match'. If implemented as a Visitor, this would be
        /// the 'Accept' method.
        /// </para>
        /// <para>
        /// Here, the method is called 'Aggregate' because this seems like an
        /// idiomatic name, corresponding to the 'Aggregate' overloads
        /// available for <see cref="IEnumerable{T}" />.
        /// </para>
        /// </remarks>
        public TResult Aggregate<TResult>(
            TResult @default,
            Func<T, TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return hasItem ? func(item) : @default;
        }

        public TAccumulate Aggregate<TAccumulate>(
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return Aggregate(seed, x => func(seed, x));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Maybe<T> other))
                return false;

            return Equals(hasItem, other.hasItem)
                && Equals(item, other.item);
        }

        public override int GetHashCode()
        {
            if (hasItem)
                return hasItem.GetHashCode() ^ item.GetHashCode();
            else
                return hasItem.GetHashCode();
        }
    }
}
