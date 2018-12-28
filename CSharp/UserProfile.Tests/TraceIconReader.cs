using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserProfile.Tests
{
    public class TraceIconReader<T> : IIconReader
    {
        private readonly ICollection<T> observations;
        private readonly IIconReader reader;

        public TraceIconReader(T label, ICollection<T> observations, IIconReader reader)
        {
            Label = label;
            this.observations = observations;
            this.reader = reader;
        }

        public T Label { get; }

        public Icon ReadIcon(User user)
        {
            observations.Add(Label);
            return reader.ReadIcon(user);
        }
    }
}
