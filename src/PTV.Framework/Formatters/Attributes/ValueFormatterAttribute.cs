using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Framework.Formatters.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ValueFormatterAttribute : Attribute
    {
        public int Order { get; set; }

        public string ValueFormatterName { get; }
        public Type ValueFormatterType { get; }

        public ValueFormatterAttribute(Type type, int order = 1)
        {
            Order = order;
            ValueFormatterType = type;
            ValueFormatterName = type.AssemblyQualifiedName;
        }
    }
}
