/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Localization.Services.Handlers;
using PTV.Localization.Services.Model;

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
                    .AddJsonFile("appsettings.local.json", optional: true)
                    .AddCommandLine(args);

                var configuration = builder.Build();
                var serviceProvider = RegisterServices(configuration);
                var buildCommands = serviceProvider.GetService<BuildCommands>();

                var transifexConfiguration = new TransifexConfiguration();
                configuration.GetSection("transifex").Bind(transifexConfiguration);

                CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
                buildCommands.AddCommands(commandLineApplication);

                commandLineApplication.Option(
                    "-dir|--folder <workingFolder>",
                    "Working folder for source and output files",
                    CommandOptionType.SingleValue);
                commandLineApplication.Option(
                    "-l|--languages <languageList>",
                    "language codes which will be processed (-l fi -l en), if it is left empty, default list is used",
                    CommandOptionType.MultipleValue);
                commandLineApplication.HelpOption(
                    "-? | -h | --help | -x"); // top level help -- '-h | -? | --help would be consumed by 'dotnet run'
                var commandArguments = args.SkipWhile(x => x.Contains("transifex:")).ToArray();
                Console.WriteLine($"Command arguments: {string.Join(";", commandArguments)}");
                commandLineApplication.Execute(commandArguments);
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
            services.Configure<TransifexConfiguration>(options => configuration.GetSection("transifex").Bind(options));
            return services.BuildServiceProvider();
        }

    }
}
