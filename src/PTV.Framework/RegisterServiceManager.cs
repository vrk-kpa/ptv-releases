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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using PTV.Framework.Attributes;
using PTV.Framework.ServiceManager;

namespace PTV.Framework
{
    /// <summary>
    /// Registers all types with RegisterServiceAttribute to IoC container
    /// </summary>
    public class RegisterServiceManager
    {
        /// <summary>
        /// Go throught all assemblies and extract all types that use RegisterServiceAttribute.
        /// Those types are registered for DI into services collection
        /// </summary>
        /// <param name="services">DI container</param>
        /// <param name="logger">Logger instance</param>
        public static void RegisterFromAllAssemblies(IServiceCollection services)
        {
            var assemblies = DependencyContext.Default.GetDefaultAssemblyNames().Where(i => i.Name.StartsWith("PTV") || i.Name.StartsWith("AuthenticationServer")).ToList();

            var typesToRegister = assemblies.Select(info => Assembly.Load(info))
                    .SelectMany(i => i.DefinedTypes)
                    .Where(i => i.CustomAttributes.Any(j => j.AttributeType == typeof(RegisterServiceAttribute) || j.AttributeType == typeof(QueryFilterAttribute) || j.AttributeType == typeof(RegisterFormatterAttribute))).ToList();


            foreach (var typeToRegister in typesToRegister)
            {
                var typeToRegisterAsType = typeToRegister.AsType();
                var typeAttributes = typeToRegister.GetCustomAttributes<RegisterServiceAttribute>(false);
                foreach (var registrationDefinition in typeAttributes)
                {
                    Func<Type, Type, IServiceCollection> registerAction;
                    switch (registrationDefinition.RegistrationType)
                    {
                        case RegisterType.Transient:
                            registerAction = services.AddTransient;
                            break;
                        case RegisterType.Scope:
                            registerAction = services.AddScoped;
                            break;
                        case RegisterType.Singleton:
                            registerAction = services.AddSingleton;
                            break;
                        default:
                            throw new NotImplementedException("Register type is not supported");

                    }
                    bool isTranslator = typeof(ITranslator).IsAssignableFrom(typeToRegisterAsType);
                    if (isTranslator && registrationDefinition.RegistrationType == RegisterType.Singleton)
                    {
                        Console.WriteLine($"!!! Translator '{typeToRegisterAsType.Name}' MUST NOT be registered as Singleton! Overrided to Scoped. !!!");
                        registerAction = services.AddScoped;
                    }
                    registerAction(registrationDefinition.RegisterAs, typeToRegisterAsType);
                }

                if (typeof(IQueryFilter).IsAssignableFrom(typeToRegisterAsType))
                {
                    if (typeToRegister.GetCustomAttributes<QueryFilterAttribute>().Any())
                    {
                        services.AddScoped(typeof(IQueryFilter), typeToRegisterAsType);
                    }
                }
            }
        }

    }
}
