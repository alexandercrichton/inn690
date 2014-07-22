using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data
{
    public abstract class Specification<T>
    {
        public abstract string Condition { get; }
        public abstract IDictionary<string, object> Parameters { get; }
        public int BlockSize { get; protected set; }

        protected Specification()
        {
            BlockSize = 1;
        }

        protected IDictionary<string, object> From(string key, object value)
        {
            return new Dictionary<string, object> { { key, value } };
        }

    }

    public class FindAllSpecification<T> : Specification<T>
    {
        public override string Condition
        {
            get { return string.Empty; }
        }

        public override IDictionary<string, object> Parameters
        {
            get { return null; }
        }
    }
}
