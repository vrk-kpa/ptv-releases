using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Framework.Interfaces;

namespace PTV.Framework.Formatters
{
    public class ValueFormatter : IValueFormatter
    {
        public virtual object Format(object value)
        {
            return null;
        }
    }
}
