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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    public class FixFintoTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public FixFintoTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            _logger.LogDebug("FixFintoTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetService<ISeedingService>();
                service?.FixDuplicatedFintoItems();
            }
            Console.WriteLine();
        }
    }




    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    public class FixMultiplePublishedEntitiesTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public FixMultiplePublishedEntitiesTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            _logger.LogDebug("FixMultiplePublishedEntitiesTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
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
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public PrintGdWithoutLaw(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            _logger.LogDebug("FixMultiplePublishedEntitiesTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
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
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public CopyLawsOfGeneralDescs(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<FixFintoTask>();

            _logger.LogDebug("FixMultiplePublishedEntitiesTask .ctor");
        }

        public void Apply()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
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