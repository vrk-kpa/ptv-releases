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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.V2.Common;
using System.Collections.Generic;
using System.Linq;
using VmDigitalAuthorization = PTV.Domain.Model.Models.V2.Common.Connections.VmDigitalAuthorization;
using PTV.Domain.Model.Enums;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Translators.Common.V2
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmConnectionInput>), RegisterType.Transient)]
    internal class ConnectionInputTranslator : Translator<ServiceServiceChannel, VmConnectionInput>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        public ConnectionInputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmConnectionInput TranslateEntityToVm(ServiceServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmConnectionInput vModel)
        {
            var descriptions = new List<VmDescription>();
            descriptions.AddNullRange(vModel.BasicInformation?.Description?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.Description)));
            descriptions.AddNullRange(vModel.BasicInformation?.AdditionalInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ChargeTypeAdditionalInfo)));

            var result = CreateViewModelEntityDefinition(vModel)
                .DefineEntitySubTree(i => i.Include(j=>j.ServiceServiceChannelServiceHours).ThenInclude(j=>j.ServiceHours))
                .UseDataContextUpdate(input => true, input => output => input.MainEntityId == output.ServiceId && input.ConnectedEntityId == output.ServiceChannelId,
                def => def.UseDataContextCreate(input => true).AddSimple(input => input.MainEntityId, output => output.ServiceId)
                )
                .AddSimple(i => i.ConnectedEntityId, o => o.ServiceChannelId)
                .AddSimple(i => i.BasicInformation?.ChargeType, o => o.ChargeTypeId)
                .AddCollection(
                    i => descriptions,
                    o => o.ServiceServiceChannelDescriptions,
                    true
                )
                .AddCollectionWithRemove(
                    i => i.DigitalAuthorization?.DigitalAuthorizations?.Select(x => new VmDigitalAuthorization
                    {
                        Id = x,
                        OwnerReferenceId = i.MainEntityId,
                        OwnerReferenceId2 = i.ConnectedEntityId
                    }),
                    o => o.ServiceServiceChannelDigitalAuthorizations,
                    x => true
                )
                .AddCollectionWithRemove(
                    i => i.ContactDetails?.Emails?.SelectMany(pair => {
                        var localizationId = languageCache.Get(pair.Key);
                        return pair.Value.Select((email, orderNumber) =>
                        {
                            email.OwnerReferenceId = i.MainEntityId;
                            email.OwnerReferenceId2 = i.ConnectedEntityId;
                            email.LanguageId = localizationId;
                            email.OrderNumber = orderNumber;
                            return email;
                        });
                    }),
                    o => o.ServiceServiceChannelEmails,
                    x => true
                )
                .AddCollectionWithRemove(
                    i => i.ContactDetails?.WebPages?.SelectMany(pair => {
                        var localizationId = languageCache.Get(pair.Key);
                        return pair.Value.Where(x => !x.UrlAddress.IsNullOrWhitespace())
                            .Select((webPage, orderNumber) =>
                        {
                            webPage.LocalizationId = localizationId;
                            webPage.OrderNumber = orderNumber;
                            return webPage;
                        });
                    }),
                    o => o.ServiceServiceChannelWebPages,
                    r => true
                )
                .AddCollectionWithRemove(
                    i =>
                    {
                        var faxNumberTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());
                        var phoneNumberTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
                        var faxNumbers = i.ContactDetails?.FaxNumbers?.SelectMany(pair =>
                        {
                            var localizationId = languageCache.Get(pair.Key);
                            return pair.Value.Select((faxNumber, orderNumber) =>
                            {
                                faxNumber.LanguageId = localizationId;
                                faxNumber.TypeId = faxNumberTypeId;
                                return faxNumber;
                            });
                        });
                        var phoneNumbers = i.ContactDetails?.PhoneNumbers?.SelectMany(pair =>
                        {
                            var localizationId = languageCache.Get(pair.Key);
                            return pair.Value.Select((phoneNumber, orderNumber) =>
                            {
                                phoneNumber.LanguageId = localizationId;
                                phoneNumber.TypeId = phoneNumberTypeId;
                                return phoneNumber;
                            });
                        });
                        var contactNumbers = phoneNumbers?
                            .Concat(faxNumbers)
                            .Select((contactNumber, index) =>
                            {
                                contactNumber.OwnerReferenceId = i.MainEntityId;
                                contactNumber.OwnerReferenceId2 = i.ConnectedEntityId;
                                contactNumber.OrderNumber = index;
                                return contactNumber;
                            });
                        return contactNumbers;
                    },
                    o => o.ServiceServiceChannelPhones,
                    r => true
                )
                .AddCollectionWithRemove(
                    i => i.ContactDetails?.PostalAddresses?.Select((postalAddress, orderNumber) =>
                    {
                        postalAddress.OwnerReferenceId = i.MainEntityId;
                        postalAddress.OwnerReferenceId2 = i.ConnectedEntityId;
                        postalAddress.OrderNumber = orderNumber;
                        return postalAddress;
                    }),
                    o => o.ServiceServiceChannelAddresses,
                    x => true
                )
                .AddCollectionWithRemove(
                    i => i.OpeningHours?.StandardHours?.Select((standardHour, orderNumber) =>
                    {
                        standardHour.OwnerReferenceId = i.MainEntityId;
                        standardHour.OwnerReferenceId2 = i.ConnectedEntityId;
                        standardHour.OrderNumber = orderNumber;
                        standardHour.ServiceHoursType = ServiceHoursTypeEnum.Standard;
                        return standardHour;
                    }),
                    o => o.ServiceServiceChannelServiceHours, r => r.ServiceHours != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Standard.ToString())
                )
                .AddCollectionWithRemove(
                    i => i.OpeningHours?.ExceptionHours?.Select((exceptionalHour, orderNumber) =>
                    {
                        exceptionalHour.OwnerReferenceId = i.MainEntityId;
                        exceptionalHour.OwnerReferenceId2 = i.ConnectedEntityId;
                        exceptionalHour.OrderNumber = orderNumber;
                        exceptionalHour.ServiceHoursType = ServiceHoursTypeEnum.Exception;
                        return exceptionalHour;
                    }),
                    o => o.ServiceServiceChannelServiceHours, r => r.ServiceHours != null && r.ServiceHours.HolidayServiceHour == null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString())
                )
                .AddCollectionWithRemove(
                    i => i.OpeningHours?.SpecialHours?.Select((specialHour, orderNumber) =>
                    {
                        specialHour.OwnerReferenceId = i.MainEntityId;
                        specialHour.OwnerReferenceId2 = i.ConnectedEntityId;
                        specialHour.OrderNumber = orderNumber;
                        specialHour.ServiceHoursType = ServiceHoursTypeEnum.Special;
                        return specialHour;
                    }),
                    o => o.ServiceServiceChannelServiceHours, r => r.ServiceHours != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Special.ToString())
                )
                .AddCollectionWithRemove(
                    i => i.OpeningHours?.HolidayHours?.Select((specialHour, orderNumber) =>
                    {
                        specialHour.OwnerReferenceId = i.MainEntityId;
                        specialHour.OwnerReferenceId2 = i.ConnectedEntityId;
                        specialHour.OrderNumber = orderNumber;
                        specialHour.ServiceHoursType = ServiceHoursTypeEnum.Exception;
                        return specialHour;
                    }),
                    o => o.ServiceServiceChannelServiceHours, r => r.ServiceHours?.HolidayServiceHour != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString())
                );

            switch (vModel.MainEntityType)
            {
                case DomainEnum.Channels:
                    if (vModel.ServiceOrderNumber != 0)
                    {
                        result.AddSimple(i => i.ServiceOrderNumber, o => o.ServiceOrderNumber);
                    }
                    break;
                case DomainEnum.Services:
                    if (vModel.ChannelOrderNumber != 0)
                    {
                        result.AddSimple(i => i.ChannelOrderNumber, o => o.ChannelOrderNumber);
                    }
                    break;
            }

            return result.GetFinal();
        }
        private VmDescription CreateDescription(string language, string value, VmConnectionInput vModel, DescriptionTypeEnum typeEnum, bool isEmpty = false)
        {
            return new VmDescription
            {
                Description = isEmpty ? null : value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.MainEntityId,
                OwnerReferenceId2 = vModel.ConnectedEntityId,
                LocalizationId = languageCache.Get(language)
            };
        }
    }
}
