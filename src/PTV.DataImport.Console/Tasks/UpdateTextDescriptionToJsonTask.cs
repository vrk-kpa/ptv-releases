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
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Framework.Interfaces;
using PTV.Framework.TextManager;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Caches;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    [RegisterService(typeof(UpdateTextDescriptionToJsonTask), RegisterType.Transient)]
    public class UpdateTextDescriptionToJsonTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        private ITextManager _textManager;
        private ILoggerFactory _loggerFactory;

        public UpdateTextDescriptionToJsonTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<UpdateTextDescriptionToJsonTask>();
            _loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            _logger.LogDebug("UpdateTextDescriptionToJsonTask .ctor");
            _textManager = new TextManager(_loggerFactory);
        }

        public void CheckAndUpdateTextDescriptionToJson()
        {
            Console.WriteLine("Check and update text descriptions...");

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var commonService = serviceScope.ServiceProvider.GetService<ICommonService>();
                int updatedItemCount = 0;

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    //Organization description
                    updatedItemCount = 0;
                    var organizationDescriptionRep = unitOfWork.CreateRepository<IOrganizationDescriptionRepository>();
                    organizationDescriptionRep.All().ForEach(entity =>
                        {
                            if (!string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                            {
                                entity.Description = _textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                                updatedItemCount++;
                            }
                        });
                    Console.WriteLine($@"Update OrganizationDescription is done. Result updated {updatedItemCount}.");


                    //Service description
                    updatedItemCount = 0;
                    var serviceDescriptionRep = unitOfWork.CreateRepository<IServiceDescriptionRepository>();
                    serviceDescriptionRep.All().ForEach(entity =>
                    {
                        if ((commonService.IsDescriptionEnumType(entity.TypeId, DescriptionTypeEnum.Description.ToString()) || commonService.IsDescriptionEnumType(entity.TypeId, DescriptionTypeEnum.ServiceUserInstruction.ToString()))
                            && !string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                        {
                            entity.Description = _textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                            updatedItemCount++;
                        }
                    });
                    Console.WriteLine($@"Update ServiceDescription is done. Result updated {updatedItemCount}.");

                    //Service requirement
                    updatedItemCount = 0;
                    var serviceRequirementRep = unitOfWork.CreateRepository<IServiceRequirementRepository>();
                    serviceRequirementRep.All().ForEach(entity =>
                    {
                        if (!string.IsNullOrEmpty(entity.Requirement) && !IsValidJson(entity.Requirement))
                        {
                            entity.Requirement = _textManager.ConvertTextWithLineBreaksToJson(entity.Requirement);
                            updatedItemCount++;
                        }
                    });
                    Console.WriteLine($@"Update ServiceRequirement is done. Result updated {updatedItemCount}.");

                    //ServiceChannel description
                    updatedItemCount = 0;
                    var serviceChannelDescriptionRep = unitOfWork.CreateRepository<IServiceChannelDescriptionRepository>();
                    serviceChannelDescriptionRep.All().ForEach(entity =>
                    {
                        if (commonService.IsDescriptionEnumType(entity.TypeId, DescriptionTypeEnum.Description.ToString()) && !string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                        {
                            entity.Description = _textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                            updatedItemCount++;
                        }
                    });
                    Console.WriteLine($@"Update ServiceChannelDescription is done. Result updated {updatedItemCount}.");

                    //Statutory service requirement
                    updatedItemCount = 0;
                    var statutoryServiceRequirementRep = unitOfWork.CreateRepository<IStatutoryServiceRequirementRepository>();
                    statutoryServiceRequirementRep.All().ForEach(entity =>
                    {
                        if (!string.IsNullOrEmpty(entity.Requirement) && !IsValidJson(entity.Requirement))
                        {
                            entity.Requirement = _textManager.ConvertTextWithLineBreaksToJson(entity.Requirement);
                            updatedItemCount++;
                        }
                    });
                    Console.WriteLine($@"Update StatutoryServiceRequirement is done. Result updated {updatedItemCount}.");

                    //Statutory service description
                    updatedItemCount = 0;
                    var statutoryServiceDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceDescriptionRepository>();
                    statutoryServiceDescriptionRep.All().ForEach(entity =>
                    {
                        if ((commonService.IsDescriptionEnumType(entity.TypeId, DescriptionTypeEnum.Description.ToString())
                                || commonService.IsDescriptionEnumType(entity.TypeId, DescriptionTypeEnum.BackgroundDescription.ToString())
                                || commonService.IsDescriptionEnumType(entity.TypeId, DescriptionTypeEnum.ServiceUserInstruction.ToString()))
                            && !string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                        { 
                            entity.Description = _textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                            updatedItemCount++;
                        }
                    });
                    Console.WriteLine($@"Update StatutoryServiceDescription is done. Result updated {updatedItemCount}.");

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    Console.WriteLine("Done.");
                });
            }


        }

        private bool IsValidJson(string json)
        {
            return (json.StartsWith("{") && json.EndsWith("}")) || (json.StartsWith("[") && json.EndsWith("]"));
        }
    }
}
