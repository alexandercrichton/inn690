using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Veis.WebInterface.Infrastructure
{
    public abstract class PageBuilder
    {
        public static PageBuilder _instance;

        public static PageBuilder Current
        {
            get
            {
                return _instance ??
                    Interlocked.CompareExchange(ref _instance, new DefaultPageBuilder(), null) ??
                    _instance;
            }
            set { _instance = value; }
        }

        public abstract void Transform(string message, object data, TextWriter output);

        public string Transform(string message, object data)
        {
            var writer = new StringWriter();
            Transform(message, data, writer);
            return writer.ToString();
        }
    }
}
