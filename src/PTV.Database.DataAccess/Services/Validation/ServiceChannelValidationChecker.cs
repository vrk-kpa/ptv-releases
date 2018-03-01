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
* THE SOFTWARE.C:\Projects\PTV_TEST\src\PTV.Database.DataAccess\Services\Security\
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(IBaseValidationChecker<ServiceChannelVersioned>), RegisterType.Transient)]
    internal class ServiceChannelValidationChecker : BaseValidationChecker<ServiceChannelVersioned>
    {
        private readonly ITypesCache typesCache;
        private IVersioningManager VersioningManager;

        public ServiceChannelValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
            this.VersioningManager = resolveManager.Resolve<IVersioningManager>();
        }

        public override ServiceChannelVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            return GetEntity<ServiceChannelVersioned>(id, unitOfWork,
                q => q.Include(i => i.ServiceChannelNames)
                    .Include(i => i.LanguageAvailabilities)
                    .Include(i => i.ServiceChannelDescriptions)
                    .Include(i => i.Organization).ThenInclude(i => i.Versions)
                    .Include(i => i.Languages)
                    .Include(i => i.Phones).ThenInclude(i => i.Phone)
                    .Include(i => i.Emails).ThenInclude(i => i.Email)
                    .Include(i => i.Attachments).ThenInclude(i => i.Attachment)
                    .Include(i => i.Areas)
                    .Include(i => i.AreaMunicipalities)
                    .Include(i => i.WebPages).ThenInclude(i => i.WebPage)
                    .Include(i => i.ElectronicChannels).ThenInclude(i => i.LocalizedUrls)
                    .Include(i => i.WebpageChannels).ThenInclude(i => i.LocalizedUrls)
                    .Include(i => i.PrintableFormChannels).ThenInclude(i => i.ChannelUrls)
                    .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                    .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                    .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                    .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)

            );
        }

        public override Dictionary<Guid, List<ValidationMessage>> ValidateEntity(Guid? language)
        {
            foreach (var entityLanguageId in entityOrPublishedLanguagesAvailabilityIds)
            {
                SetValidationLanguage(entityLanguageId);

                var organizationStatus = VersioningManager.ApplyPublishingStatusFilterFallback(entity.Organization.Versions)?.PublishingStatusId;
                NotBeTrue("organization", x => organizationStatus != typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()));

                NotEmptyString("name", x => x.ServiceChannelNames
                    .Where(y => y.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) && y.LocalizationId == entityLanguageId)
                    .Select(y => y.Name)
                    .FirstOrDefault());
                NotEmptyString("shortDescription",
                    x => x.ServiceChannelDescriptions
                        .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) && y.LocalizationId == entityLanguageId)
                        .Select(y => y.Description)
                        .FirstOrDefault());
                NotEmptyTextEditorString("description",
                    x => x.ServiceChannelDescriptions
                        .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && y.LocalizationId == entityLanguageId)
                        .Select(y => y.Description)
                        .FirstOrDefault());

                ValidateEmails(entityLanguageId);
                
                //channels
                if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString()) && entity.ElectronicChannels.Any())
                {
                    ValidateAreaInformations();
                    ValidatePhonesByPhoneType(entityLanguageId);
                    ValidateAttachments(entityLanguageId);

                    CheckEntityWithMergeResult(entity.ElectronicChannels.First());

                }
                else if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString()) && entity.WebpageChannels.Any())
                {
                    NotEmptyList("languages", x => x.Languages);
                    ValidatePhonesByPhoneType(entityLanguageId);

                    CheckEntityWithMergeResult(entity.WebpageChannels.First());
                }
                else if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString()) && entity.PrintableFormChannels.Any())
                {
                    ValidateAreaInformations();
                    ValidatePhonesByPhoneType(entityLanguageId);
                    ValidateAttachments(entityLanguageId);

                    CheckEntityWithMergeResult(entity.PrintableFormChannels.First());

                }
                else if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString()))
                {
                    ValidateAreaInformations();
                    NotEmptyList("languages", x => x.Languages);
                    if (!NotEmptyListFunc("phoneNumbers", x => x.Phones.Where(y => y.Phone?.LocalizationId == entityLanguageId)))
                    {
                        ValidateAllPhones(entityLanguageId);
                    }
                }
                else if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString()) && entity.ServiceLocationChannels.Any())
                {
                    ValidateAreaInformations();
                    NotEmptyList("languages", x => x.Languages);
                    ValidatePhonesByPhoneType(entityLanguageId);
                    ValidatePhonesByFaxType(entityLanguageId);
                    ValidateWebpages(entityLanguageId);

                    CheckEntityWithMergeResult(entity.ServiceLocationChannels.First());
                }
            }

            return validationMessagesDictionary;
        }

        private void ValidateWebpages(Guid entityLanguageId)
        {
            foreach (var webPage in entity.WebPages.Where(x => x.WebPage?.LocalizationId == entityLanguageId)
                .Select(x => x.WebPage))
            {
                CheckEntityWithMergeResult(webPage);
            }
        }

        private void ValidateAreaInformations()
        {
            if (entity.AreaInformationTypeId.IsAssigned())
            {
                if (entity.AreaInformationTypeId ==
                    typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString()))
                {
                    NotBeTrue("areaType", x => !(x.Areas.Any() || x.AreaMunicipalities.Any()));
                }
            }
        }

        private void ValidateAttachments(Guid languageId)
        {

            foreach (var attachment in entity.Attachments.Where(x => x.Attachment?.LocalizationId == languageId)
                .Select(x => x.Attachment))
            {
                CheckEntityWithMergeResult(attachment);
            }
        }

        private void ValidateEmails(Guid languageId)
        {
            foreach (var email in entity.Emails.Where(x => x.Email?.LocalizationId == languageId).Select(x => x.Email))
            {
                CheckEntityWithMergeResult(email);
            }
        }

        private void ValidatePhonesByPhoneType(Guid languageId)
        {
            foreach (var phone in entity.Phones.Where(x => x.Phone?.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())
                                                           && x.Phone?.LocalizationId == languageId).Select(x => x.Phone))
            {
                CheckEntityWithMergeResult(phone);
            }
        }

        private void ValidatePhonesByFaxType(Guid languageId)
        {
            foreach (var phone in entity.Phones.Where(x => x.Phone?.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString())
                                                           && x.Phone?.LocalizationId == languageId).Select(x => x.Phone))
            {
                CheckEntityWithMergeResult(phone);
            }
        }

        private void ValidateAllPhones(Guid languageId)
        {
            foreach (var phone in entity.Phones.Where(x => x.Phone?.LocalizationId == languageId).Select(x => x.Phone))
            {
                CheckEntityWithMergeResult(phone);
            }
        }
    }
}
