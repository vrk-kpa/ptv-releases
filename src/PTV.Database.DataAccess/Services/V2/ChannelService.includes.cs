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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using IChannelService = PTV.Database.DataAccess.Interfaces.Services.V2.IChannelService;
using PTV.Database.DataAccess.Interfaces.Repositories;

namespace PTV.Database.DataAccess.Services.V2
{
    internal partial class ChannelService : EntityServiceBase<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>, IChannelService
    {
        private static IIncludableQueryable<ServiceChannelVersioned, ServiceChannel> IncludeEChannelDetails(IQueryable<ServiceChannelVersioned> q)
        {
            return q.Include(x => x.ServiceChannelNames)
                .Include(x => x.ServiceChannelDescriptions)
                .Include(x => x.Versioning)
                .Include(x => x.PublishingStatus)
                .Include(x => x.AreaMunicipalities)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                .Include(x => x.ConnectionType)
                .Include(x => x.AccessibilityClassifications).ThenInclude(j => j.AccessibilityClassification)
                .Include(x => x.ElectronicChannels).ThenInclude(x => x.LocalizedUrls).ThenInclude(x => x.WebPage)
                .Include(x => x.ElectronicChannels).ThenInclude(x => x.LocalizedUrls).ThenInclude(x => x.Localization)
                .Include(x => x.UnificRoot);
        }

        private static IIncludableQueryable<ServiceChannelVersioned, ServiceChannel> IncludePhoneChannelDetails(IQueryable<ServiceChannelVersioned> q)
        {
            return q.Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .Include(x => x.PublishingStatus)
                .Include(j => j.ServiceChannelDescriptions)
                .Include(j => j.ServiceChannelNames)
                .Include(x => x.AreaMunicipalities)
                .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                .Include(j => j.WebPages).ThenInclude(j => j.Localization)
                .Include(x => x.UnificRoot);
        }

        private static IIncludableQueryable<ServiceChannelVersioned, ServiceChannel> IncludePrintableFormDetails(IQueryable<ServiceChannelVersioned> q)
        {
            return q.Include(x => x.ServiceChannelNames)
                .Include(x => x.Versioning)
                .Include(x => x.ConnectionType)
                .Include(x => x.ServiceChannelDescriptions)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.PublishingStatus)
                .Include(x => x.AreaMunicipalities)
                .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                .Include(x => x.PrintableFormChannels).ThenInclude(x => x.ChannelUrls).ThenInclude(x => x.Type)
                .Include(x => x.PrintableFormChannels).ThenInclude(x => x.ChannelUrls).ThenInclude(x => x.WebPage)
                .Include(x => x.PrintableFormChannels).ThenInclude(x => x.ChannelUrls).ThenInclude(x => x.Localization)
                .Include(x => x.PrintableFormChannels).ThenInclude(x => x.FormIdentifiers)
                .Include(x => x.UnificRoot);
        }

        private static IIncludableQueryable<ServiceChannelVersioned, ICollection<ServiceChannelSocialHealthCenter>> IncludeServiceLocationDetails(IQueryable<ServiceChannelVersioned> q)
        {
            return q.Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .Include(x => x.PublishingStatus)
                .Include(j => j.ServiceChannelDescriptions)
                .Include(j => j.ServiceChannelNames)
                .Include(x => x.AreaMunicipalities)
                .Include(x => x.DisplayNameTypes)
                .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                .Include(j => j.WebPages).ThenInclude(j => j.Localization)
                .Include(x => x.UnificRoot).ThenInclude(j => j.SocialHealthCenters);
        }

        private static IIncludableQueryable<ServiceChannelVersioned, ServiceChannel> IncludeWebPageDetails(IQueryable<ServiceChannelVersioned> q)
        {
            return q.Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .Include(j => j.ServiceChannelDescriptions)
                .Include(j => j.ServiceChannelNames)
                .Include(j => j.WebpageChannels).ThenInclude(j => j.LocalizedUrls).ThenInclude(j => j.WebPage)
                .Include(j => j.WebpageChannels).ThenInclude(j => j.LocalizedUrls).ThenInclude(j => j.Localization)
                .Include(x => x.AreaMunicipalities)
                .Include(x => x.AccessibilityClassifications).ThenInclude(j => j.AccessibilityClassification)
                .Include(x => x.UnificRoot);
        }

        private void IncludeCommonDetails(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var channelAreasRepo = unitOfWork.CreateRepository<IServiceChannelAreaRepository>();
            var channelLanguageRepo = unitOfWork.CreateRepository<IServiceChannelLanguageRepository>();
            var channelEmailRepo = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();
            var channelPhoneRepo = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();

            var areas = channelAreasRepo.All()
                .Include(x => x.Area)
                .Where(x => x.ServiceChannelVersionedId == entity.Id)
                .ToList();
            var languages = channelLanguageRepo.All()
                .Include(x => x.Language)
                .Where(x => x.ServiceChannelVersionedId == entity.Id)
                .ToList();
            var emails = channelEmailRepo.All()
                .Include(x => x.Email)
                .Where(x => x.ServiceChannelVersionedId == entity.Id)
                .ToList();
            var phones = channelPhoneRepo.All()
                .Include(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country)
                .ThenInclude(x => x.CountryNames)
                .Where(x => x.ServiceChannelVersionedId == entity.Id)
                .ToList();

            entity.Areas = areas;
            entity.Languages = languages;
            entity.Emails = emails;
            entity.Phones = phones;
        }

        private void IncludeServiceHours(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var serviceHoursRepo = unitOfWork.CreateRepository<IServiceChannelServiceHoursRepository>();

            var serviceHours = serviceHoursRepo.All()
                .Include(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                .Include(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.ServiceHours).ThenInclude(i => i.HolidayServiceHour)
                .Where(x => x.ServiceChannelVersionedId == entity.Id)
                .ToList();

            entity.ServiceChannelServiceHours = serviceHours;
        }

        private void IncludeAddresses(IUnitOfWork unitOfWork, ServiceChannelVersioned entity, ServiceChannelTypeEnum type)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var channelAddressRepo = unitOfWork.CreateRepository<IServiceChannelAddressRepository>();
            var query = channelAddressRepo.All()
                .Include(i => i.Character)
                .Include(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(x => x.Address).ThenInclude(x => x.ClsAddressPoints).ThenInclude(x => x.Municipality)
                .Include(x => x.Address).ThenInclude(x => x.ClsAddressPoints).ThenInclude(x => x.AddressStreet).ThenInclude(x => x.StreetNames)
                .Include(x => x.Address).ThenInclude(x => x.ClsAddressPoints).ThenInclude(x => x.AddressStreet).ThenInclude(x => x.StreetNumbers).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                .Include(x => x.Address).ThenInclude(x => x.ClsAddressPoints).ThenInclude(x => x.AddressStreetNumber).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                .Include(x => x.Address).ThenInclude(x => x.ClsAddressPoints).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                as IQueryable<ServiceChannelAddress>;

            switch (type)
            {
                case ServiceChannelTypeEnum.PrintableForm:
                    query = query
                        .Include(i => i.Address).ThenInclude(i => i.Receivers);
                    break;
                case ServiceChannelTypeEnum.ServiceLocation:
                    query = query
                        .Include(x => x.Address).ThenInclude(i => i.AddressOthers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .Include(x => x.Address).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames);
                    break;
                default:
                    throw new NotImplementedException();
            }

            var addresses = query.Where(x => x.ServiceChannelVersionedId == entity.Id).ToList();
            entity.Addresses = addresses;

            // SFIPTV-2272: Service location loading crash
            foreach (var address in entity.Addresses)
            {
                var additionalInfoRepo = unitOfWork.CreateRepository<IAddressAdditionalInformationRepository>();
                var additionalInfo = additionalInfoRepo.All().Where(ai => ai.AddressId == address.AddressId).ToList();
                address.Address.AddressAdditionalInformations = additionalInfo;
                
                var coordinateRepo = unitOfWork.CreateRepository<IAddressCoordinateRepository>();
                var coordinates = coordinateRepo.All().Where(c => c.RelatedToId == address.AddressId).ToList();
                address.Address.Coordinates = coordinates;
            }
        }

        private void IncludeConnections(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var connectionRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var connections = connectionRepo.All()
                .Include(j => j.ServiceServiceChannelDescriptions)
                .Include(j => j.ServiceServiceChannelDigitalAuthorizations).ThenInclude(j => j.DigitalAuthorization)
                .Include(j => j.ServiceServiceChannelExtraTypes)
                .ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions)
                .Where(x => x.ServiceChannelId == entity.UnificRootId)
                .ToList();

            entity.UnificRoot.ServiceServiceChannels = connections;
        }

        private void IncludeAccessibilityRegisters(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var arRepo = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
            var accessibilityRegisters = arRepo.All()
                .Include(i => i.Address)
                .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Values)
                .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences)
                .ThenInclude(i => i.Values)
                .Include(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.AddressOthers)
                .ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Where(i => i.ServiceChannelId == entity.UnificRootId)
                .ToList();

            entity.UnificRoot.AccessibilityRegisters = accessibilityRegisters;
        }
    }
}
