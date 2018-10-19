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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Domain.Model.Models.OpenApi.V8;
using Microsoft.EntityFrameworkCore;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    abstract class OpenApiServiceChannelInTranslator<TVmOpenApiServiceChannelIn> : OpenApiBaseTranslator<ServiceChannelVersioned, TVmOpenApiServiceChannelIn> where TVmOpenApiServiceChannelIn : class, IVmOpenApiServiceChannelIn
    {
        ITypesCache typesCache;
        protected OpenApiServiceChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override TVmOpenApiServiceChannelIn TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelInTranslator!");
        }
        public override ServiceChannelVersioned TranslateVmToEntity(TVmOpenApiServiceChannelIn vModel)
        {
            return CreateVmToChannelDefinitions(vModel)
                .GetFinal();
        }

        protected ITranslationDefinitions<TVmOpenApiServiceChannelIn, ServiceChannelVersioned> CreateVmToChannelDefinitions(TVmOpenApiServiceChannelIn vModel)
        {
            if (vModel.VersionId.IsAssigned())
            {
                vModel.ServiceChannelNamesWithType.ForEach(n => n.OwnerReferenceId = vModel.VersionId);
                vModel.ServiceChannelDescriptions.ForEach(d => d.OwnerReferenceId = vModel.VersionId);
                vModel.ServiceHours.ForEach(h => h.OwnerReferenceId = vModel.VersionId);
                vModel.WebPages.ForEach(w => w.OwnerReferenceId = vModel.VersionId);
                vModel.SupportEmails.ForEach(e => e.OwnerReferenceId = vModel.VersionId);
                vModel.SupportPhones.ForEach(p => p.OwnerReferenceId = vModel.VersionId);
            }

            var definition = CreateViewModelEntityDefinition<ServiceChannelVersioned>(vModel)
                .DisableAutoTranslation()
                .DefineEntitySubTree(i => i.Include(j => j.Phones).ThenInclude(j => j.Phone))
                .UseDataContextCreate(i => !i.VersionId.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.VersionId.IsAssigned(), i => o => i.VersionId.Value == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o);

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definition.AddCollectionWithKeep(i => languages, o => o.LanguageAvailabilities, x => true);
            }

            // organization
            if (!string.IsNullOrEmpty(vModel.OrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.OrganizationId), o => o.OrganizationId);
            }

            // names
            if (vModel.ServiceChannelNamesWithType != null && vModel.ServiceChannelNamesWithType.Count > 0)
            {
                definition.AddCollectionWithKeep(i => i.ServiceChannelNamesWithType, o => o.ServiceChannelNames,  x => true);
            }

            // descriptions
            if (vModel.ServiceChannelDescriptions != null && vModel.ServiceChannelDescriptions.Count > 0)
            {
                definition.AddCollectionWithKeep(i => i.ServiceChannelDescriptions, o => o.ServiceChannelDescriptions, x => true);
            }

            // area type
            if (!string.IsNullOrEmpty(vModel.AreaType))
            {
                definition.AddSimple(i => typesCache.Get<AreaInformationType>(vModel.AreaType), o => o.AreaInformationTypeId);
            }

            // Set areas
            if (vModel.Areas.Count > 0)
            {
                var municipalityAreas = vModel.Areas.Where(a => a.Type == AreaTypeEnum.Municipality.ToString()).ToList();
                var otherAreas = vModel.Areas.Where(a => a.Type != AreaTypeEnum.Municipality.ToString()).ToList();
                if (municipalityAreas.Count > 0)
                {
                    var municipalities = new List<VmOpenApiStringItem>();
                    municipalityAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(m => municipalities.Add(new VmOpenApiStringItem { Value = m, OwnerReferenceId = vModel.VersionId }));
                    });
                    definition.AddCollectionWithRemove(i => municipalities, o => o.AreaMunicipalities, x => true); // Update municipalities
                    if (otherAreas.Count == 0)
                    {
                        definition.AddCollectionWithRemove(i => new List<VmOpenApiArea>(), o => o.Areas, x => true); // Remove possible old areas
                    }
                }
                if (otherAreas.Count > 0)
                {
                    var areas = new List<VmOpenApiArea>();
                    otherAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(a => areas.Add(new VmOpenApiArea { Type = area.Type, Code = a, OwnerReferenceId = vModel.VersionId }));
                    });
                    definition.AddCollectionWithRemove(i => areas, o => o.Areas, x => true); // Update areas
                    if (municipalityAreas.Count == 0)
                    {
                        definition.AddCollectionWithRemove(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, x => true); // Remove possible old municipalities
                    }
                }
            }
            else if (vModel.VersionId.IsAssigned() && !vModel.AreaType.IsNullOrEmpty() && vModel.AreaType != AreaInformationTypeEnum.AreaType.ToString())
            {
                // Area type has been changed into WholeCountry or WholeCountryExceptAlandIslands so we need to remove possible old municipalities and areas.
                definition.AddCollectionWithRemove(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, x => true);
                definition.AddCollectionWithRemove(i => new List<VmOpenApiArea>(), o => o.Areas, x => true);
            }

            // Set connection type
            var connectionType = ServiceChannelConnectionTypeEnum.NotCommon;
            if (vModel.IsVisibleForAll)
            {
                connectionType = ServiceChannelConnectionTypeEnum.CommonForAll;
            }
            var connectionTypeId = typesCache.Get<ServiceChannelConnectionType>(connectionType.ToString());
            definition.AddSimple(i => connectionTypeId, o => o.ConnectionTypeId);

            // languages
            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                var order = 1;
                var languageList = new List<VmOpenApiStringItem>();
                vModel.Languages.ForEach(l => languageList.Add(new VmOpenApiStringItem
                {
                    Value = l,
                    Order = order++,
                    OwnerReferenceId = vModel.VersionId
                }));
                definition.AddCollectionWithRemove(i => languageList, o => o.Languages, x => true);
            }

            // web pages
            if (vModel.DeleteAllWebPages || vModel.WebPages?.Count > 0)
            {
                // Is order already set? (PTV-3705)
                if (vModel.WebPages != null && !vModel.WebPages.Any(w => w.OrderNumber != 0))
                {
                    var order = 1;
                    vModel.WebPages?.ForEach(w => w.OrderNumber = order++);
                }
                definition.AddCollectionWithRemove(i => i.WebPages, o => o.WebPages, x => true);
            }

            // service hours
            if (vModel.DeleteAllServiceHours || vModel.ServiceHours?.Count > 0)
            {
                // Set service hours order
                var sortedServiceHours = vModel.ServiceHours.OrderBy(x => x, new ServiceHourOrderComparer<V8VmOpenApiDailyOpeningTime>()).ToList();

                // Append ordering number for each item
                var index = 1;
                foreach (var serviceHour in sortedServiceHours)
                {
                    serviceHour.OrderNumber = index++;
                }

                definition.AddCollectionWithRemove(i => sortedServiceHours, o => o.ServiceChannelServiceHours, x => true);//o => o.ServiceHours, false);
            }

            // support phones
            if (vModel.DeleteAllSupportPhones || vModel.SupportPhones?.Count > 0)
            {
                var order = 1;
                
                vModel.SupportPhones.ForEach(p => p.OrderNumber = order++);
                definition.AddCollectionWithRemove(input => vModel.SupportPhones, output => output.Phones, x => true);
            }

            // support emails
            if (vModel.DeleteAllSupportEmails || vModel.SupportEmails?.Count > 0)
            {
                var emails = new List<V4VmOpenApiEmail>();
                var order = 1;
                vModel.SupportEmails.ForEach(e => emails.Add(new V4VmOpenApiEmail()
                { Value = e.Value, Language = e.Language, OwnerReferenceId = e.OwnerReferenceId, ExistsOnePerLanguage = false, OrderNumber = order++ }));
                definition.AddCollectionWithRemove(input => emails, output => output.Emails, x => true);
            }
            
            return definition;
        }

        private List<VmOpenApiLanguageAvailability> GetAvailableLanguages(TVmOpenApiServiceChannelIn vModel)
        {
            var languages = new List<VmOpenApiLanguageAvailability>();
            vModel.AvailableLanguages.ForEach(item =>
            {
                if (!languages.Select(l => l.Language).ToList().Contains(item))
                {
                    languages.Add(new VmOpenApiLanguageAvailability() {
                        Language = item,
                        OwnerReferenceId = vModel.VersionId,
                        PublishingStatus = vModel.PublishingStatus,
                        PublishAt = vModel.ValidFrom,
                        ArchiveAt = vModel.ValidTo,
                        ReviewedBy = vModel.UserName
                    });
                }
            });
            
            return languages;
        }
        
    }
}
