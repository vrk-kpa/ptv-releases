﻿/**
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
using PTV.Database.DataAccess.Interfaces.Repositories;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(ILoadingValidationChecker<ServiceChannelVersioned>), RegisterType.Transient)]
    internal class ServiceChannelValidationChecker : BaseLoadingValidationChecker<ServiceChannelVersioned>
    {
        private readonly ITypesCache typesCache;
        private IVersioningManager VersioningManager;
        private IUnitOfWork unitOfWork;

        public ServiceChannelValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
            this.VersioningManager = resolveManager.Resolve<IVersioningManager>();
        }

        protected override ServiceChannelVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            return GetEntity<ServiceChannelVersioned>(id, unitOfWork,
                q => q.Include(i => i.ServiceChannelNames)
                    .Include(i => i.LanguageAvailabilities)
                    .Include(i => i.ServiceChannelDescriptions)
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
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet)
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.PostalCode)
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.Municipality)
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreetNumber)
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                    .Include(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                    .Include(i => i.AccessibilityClassifications).ThenInclude(i => i.AccessibilityClassification)
            );
        }
        
        private string GetName(ServiceChannelVersioned channel, Guid entityLanguageId)
        {
            return channel.ServiceChannelNames
                .Where(y => y.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) &&
                            y.LocalizationId == entityLanguageId)
                .Select(y => y.Name)
                .FirstOrDefault();
        }        
        
        private string GetShortDescription(ServiceChannelVersioned channel, Guid entityLanguageId)
        {
            return entity.ServiceChannelDescriptions
                .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) && y.LocalizationId == entityLanguageId)
                .Select(y => y.Description)
                .FirstOrDefault();
        }

        protected override void ValidateEntityInternal(Guid? language)
        {
            CheckEntityWithMergeResult<Organization>(entity.OrganizationId, unitOfWork);
            CheckEntityWithMergeResult<List<ServiceHours>>(entity.Id, unitOfWork);
            
            foreach (var entityLanguageId in entityOrPublishedLanguagesAvailabilityIds)
            {
                SetValidationLanguage(entityLanguageId);

                bool nameOrShortDescriptionEmpty = NotEmptyString("name", x => GetName(x, entityLanguageId));
                nameOrShortDescriptionEmpty |= NotEmptyString("shortDescription", x => GetShortDescription(x, entityLanguageId));
                NotEmptyTextEditorString("description",
                    x => x.ServiceChannelDescriptions
                        .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && y.LocalizationId == entityLanguageId)
                        .Select(y => y.Description)
                        .FirstOrDefault());
                
                if(!nameOrShortDescriptionEmpty)
                {
                    NotBeTrue
                    (
                        "shortDescription",
                        x => GetName(x, entityLanguageId) == GetShortDescription(x, entityLanguageId),
                        ValidationErrorTypeEnum.ValueIsSame
                    );
                }

                ValidateEmails(entityLanguageId);
                
                // validate published Organization language
                var publishedOrganizationLanguageExists = false; 
                var publishedOrgInfo = VersioningManager.GetLastPublishedVersion<OrganizationVersioned>(unitOfWork, entity.OrganizationId);
                if (publishedOrgInfo != null)
                {
                    var orgRep = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
                    publishedOrganizationLanguageExists = orgRep.All()
                        .Any(x => x.OrganizationVersionedId == publishedOrgInfo.EntityId
                                  && x.StatusId ==
                                  typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString())
                                  && x.LanguageId == entityLanguageId);
                    NotBeTrue("organization", x => !publishedOrganizationLanguageExists, ValidationErrorTypeEnum.PublishedOrganizationLanguageMandatoryField);                    
                }
                NotEmptyList("languages", x => x.Languages);
                
                //channels
                if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString()) && entity.ElectronicChannels.Any())
                {
                    ValidateAreaInformations();
                    ValidatePhonesByPhoneType(entityLanguageId);
                    ValidateAttachments(entityLanguageId);
                    ValidateAccessibilityClassification(entityLanguageId);

                    CheckEntityWithMergeResult(entity.ElectronicChannels.First());

                }
                else if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString()) && entity.WebpageChannels.Any())
                {
                    ValidatePhonesByPhoneType(entityLanguageId);
                    ValidateAccessibilityClassification(entityLanguageId);

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
                    if (!NotEmptyListFunc("phoneNumbers", x => x.Phones.Where(y => y.Phone?.LocalizationId == entityLanguageId)))
                    {
                        ValidateAllPhones(entityLanguageId);
                    }
                }
                else if (entity.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString()))
                {
                    ValidateAreaInformations();
                    ValidatePhonesByPhoneType(entityLanguageId);
                    ValidatePhonesByFaxType(entityLanguageId);
                    ValidateWebpages(entityLanguageId);
                    //Visiting Addresses
                    NotEmptyList("visitingAddresses", x => entity.Addresses.Where(y => y.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString())));

                }
                               
                //Validate Addresses
                foreach (var postalAddress in entity.Addresses.Select(x => x.Address))
                {
                    CheckEntityWithMergeResult(postalAddress);
                }
            }
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
        
        private void ValidateAccessibilityClassification(Guid languageId)
        {
            foreach (var accessibilityClassification in entity.AccessibilityClassifications
                .Where(x => x.AccessibilityClassification?.LocalizationId == languageId)
                .Select(x => x.AccessibilityClassification))
            {
                CheckEntityWithMergeResult(accessibilityClassification);
            }
        }
    }
}