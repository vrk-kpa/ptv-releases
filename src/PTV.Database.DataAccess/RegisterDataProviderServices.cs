/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Utils;
using PTV.Framework.Attributes;

namespace PTV.Database.DataAccess
{
    public static class RegisterDataProviderServices
    {
        public static void RegisterFromAssembly(IServiceCollection services)
        {
            Dictionary<string, Type> namedServices = new Dictionary<string, Type>();

            var typesToRegister = typeof(RegisterDataProviderServices).GetTypeInfo().Assembly.DefinedTypes.Where(i => (typeof(IDataProviderService)).IsAssignableFrom(i.AsType()));
            foreach (var typeToRegister in typesToRegister)
            {
                var typeToRegisterAsType = typeToRegister.AsType();
                var typeAttributes = typeToRegister.GetCustomAttributes<DataProviderServiceNameAttribute>(false);
                var registerWithNames = typeAttributes.Select(i => i.Name).ToList();
                if (!registerWithNames.Any())
                {
                    registerWithNames = new List<string>() { typeToRegisterAsType.Name };
                }
                services.AddTransient(typeToRegisterAsType);
                foreach (var registerName in registerWithNames)
                {
                    namedServices.Add(registerName, typeToRegisterAsType);
                }
            }
            var namedServiceResolver = new NamedServiceResolver(services, namedServices);
            services.AddSingleton<INamedServiceResolver>(namedServiceResolver);
        }
    }
}
