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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.DataImport.Console.Tasks
{
    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    public class FixFintoTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public FixFintoTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            logger.LogDebug("FixFintoTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetService<ISeedingService>();
                service?.FixDuplicatedFintoItems();
            }
            System.Console.WriteLine();
        }
    }




    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    public class FixMultiplePublishedEntitiesTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public FixMultiplePublishedEntitiesTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            logger.LogDebug("FixMultiplePublishedEntitiesTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var seedingService = serviceScope.ServiceProvider.GetService<ISeedingService>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    seedingService.FixMultiplePublishedEntities(unitOfWork);
                });
            }
        }
    }


    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    public class PrintGdWithoutLaw
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public PrintGdWithoutLaw(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            logger.LogDebug("FixMultiplePublishedEntitiesTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var seedingService = serviceScope.ServiceProvider.GetService<ISeedingService>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    seedingService.PrintGeneralDescriptionsWithoutLaw(unitOfWork);
                });
            }
        }
    }

    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    public class CopyLawsOfGeneralDescs
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public CopyLawsOfGeneralDescs(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            logger.LogDebug("FixMultiplePublishedEntitiesTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var seedingService = serviceScope.ServiceProvider.GetService<ISeedingService>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    seedingService.CopyLawsFromPreviousToPublishedGeneralDesc(unitOfWork);
                });
            }
        }

//        public void PrintMissingSweLaws()
//        {
//            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
//            {
//                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
//                var seedingService = serviceScope.ServiceProvider.GetService<ISeedingService>();
//                scopedCtxMgr.ExecuteWriter(unitOfWork =>
//                {
//                    seedingService.PrintMissingSweLaws(unitOfWork);
//                });
//            }
//        }
    }
}
