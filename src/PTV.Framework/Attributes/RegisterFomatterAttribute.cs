using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PTV.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterFomatterAttribute : RegisterServiceAttribute
    {
        public RegisterFomatterAttribute(Type type) : base(type, RegisterType.Singleton)
        {
        }
    }
}
