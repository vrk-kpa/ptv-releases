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

using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.ExternalSources;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IServiceNumberService), RegisterType.Transient)]
    internal class ServiceNumberService : IServiceNumberService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationViewModel translationVmToEnt;
        private readonly ILogger logger;
        private readonly ResourceManager resourceManager;
        
        public ServiceNumberService(IContextManager contextManager, ResourceManager resourceManager, ILogger<ServiceService> logger, ITranslationViewModel translationVmToEnt)
        {
            this.contextManager = contextManager;
            this.translationVmToEnt = translationVmToEnt;
            this.logger = logger;
            this.resourceManager = resourceManager;
        }

        public void SeedServiceNumbersByJsonFile()
        {
            var serviceNumbersJson = resourceManager.GetJsonResource(JsonResources.ServiceNumbers);
            if (string.IsNullOrEmpty(serviceNumbersJson))
            {
                logger.LogError("Seeding of json service numbers - source is empty.");
            }
            
            SeedServiceNumbers(serviceNumbersJson);
        }

        public void SeedServiceNumbers(string jsonData)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                try
                {
                    var vmServiceNumberInput = JsonConvert.DeserializeObject<VmServiceNumberInput>(jsonData);
                    var vmServiceNumbers = vmServiceNumberInput?.Value;
                
                    if (vmServiceNumbers == null || !vmServiceNumbers.Any())
                    {
                        logger.LogError("Seeding of service numbers - source is empty.");
                        return;
                    }
                
                    vmServiceNumbers.ForEach(x => x.IsValid = true);
                    translationVmToEnt.TranslateAll<VmServiceNumber, ServiceNumber>(vmServiceNumbers, unitOfWork);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
                catch (Exception e)
                {
                    var errorMsg = $"Error occured in method SeedServiceNumbers. {e.Message}";
                    logger.LogError(e, errorMsg);
                    Console.WriteLine(errorMsg);
                }
            });
        }
    }
}
