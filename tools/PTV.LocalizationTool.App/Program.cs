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
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PTV.Framework;

namespace PTV.LocalizationTool.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.local.json", optional: true);

                var configuration = builder.Build();
                var serviceProvider = RegisterServices(configuration);
                var buildCommands = serviceProvider.GetService<BuildCommands>();

                CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
                buildCommands.AddCommands(commandLineApplication, configuration);

                commandLineApplication.HelpOption(
                    "-? | -h | --help | -x"); // top level help -- '-h | -? | --help would be consumed by 'dotnet run'
                commandLineApplication.Execute(args);
                if (args.Length == 0)
                {
                    commandLineApplication.ShowHelp();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.InnerException.Message);
                    Console.WriteLine(e.InnerException.StackTrace);
                }
            }
        }

        private static ServiceProvider RegisterServices(IConfigurationRoot configuration)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Application Configuration is null. You must call BuildConfiguration() before calling this method.");
            }
            
            var services = new ServiceCollection();
            services.AddLogging();
            RegisterServiceManager.RegisterFromAllAssemblies(services, configuration);
            services.AddSingleton(configuration);
            return services.BuildServiceProvider();
        }
        
    }
}
