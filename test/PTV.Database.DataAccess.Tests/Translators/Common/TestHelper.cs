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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;

namespace PTV.Database.DataAccess.Tests.Translators.Common
{
    public static class TestHelper
    {
        public static void SetupLanguagesAvailabilities(ILanguagesAvailabilities model, string lang = null)
        {
            var vmLanguageAvailabilityInfo = string.IsNullOrEmpty(lang)
                ? new VmLanguageAvailabilityInfo()
                : new VmLanguageAvailabilityInfo {LanguageId = lang.GetGuid()};

            model.LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>
            {
                vmLanguageAvailabilityInfo
            };
        }


        public static VmChannelAttachment CreateVmChannelAttachmentModel(Guid? localizationId = null)
        {
            return new VmChannelAttachment
            {
                Id = "xxx".GetGuid(),
                UrlAddress = "attachmentUrl",
                Description = "attachmentDescription",
                Name = "attachmentName",
                LocalizationId = localizationId
            };
        }
        public static VmChannelAttachment CreateVmChannelUrlModel()
        {
            return new VmChannelAttachment
            {
                Id = "initial".GetGuid(),
                UrlAddress = "attachmenturl",
            };
        }

        public static VmDescription CreateVmDescriptionModel(DescriptionTypeEnum descriptionType)
        {
            return new VmDescription
            {
                Description = "channelDescription",
                TypeId = Guid.NewGuid()//descriptionType
            };
        }

        public static VmName CreateVmNameModel(NameTypeEnum nameType)
        {
            return new VmName
            {
                Name = "channelName",
                TypeId = nameType.ToString().GetGuid()
            };
        }

        public static VmPhone CreateVmPhoneModel()
        {
            return new VmPhone
            {
                ChargeDescription = "description",
                ChargeType = Guid.NewGuid(),
                Number = "456789",
                DialCode = new VmDialCode { Id = Guid.NewGuid() },
                AdditionalInformation = "info",
                TypeId = Guid.NewGuid()
            };
        }

        public static T CreateVmHoursModel<T>(ServiceHoursTypeEnum serviceHoursTypeEnum) where T : VmOpeningHour, new()
        {
            return new T
            {
                Name = new Dictionary<string, string> { { "fi", "name" } },
                Id = Guid.Empty,
                ServiceHoursType = serviceHoursTypeEnum,
                DateFrom = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                DateTo = DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds(),
                //                DailyHours = new List<VmDailyHours>()
            };
        }

        public static void SetupVmChannelDescriptionModel(VmServiceChannel model)
        {
            model.Description = new Dictionary<string, string> { { "fi", "Channel description" } };
            model.Name = new Dictionary<string, string> { { "fi", "Channel name" } };
            model.ShortDescription = new Dictionary<string, string> { { "fi", "Channel short description" } };
            model.OrganizationId = Guid.NewGuid();
        }

        public static VmElectronicChannel CreateVmElectronicChannelModel(bool isAccessibiltyClassificationSet = false)
        {
            var itemListGenerator = new ItemListModelGenerator();
            var step1 = new VmElectronicChannel
            {
                IsOnLineAuthentication = true,
                IsOnlineSign = true,
                SignatureCount = 10,
                Emails = new Dictionary<string, List<VmEmailData>> { { "fi", new List<VmEmailData> { CreateVmEmailDataModel() } } },
                PhoneNumbers = new Dictionary<string, List<VmPhone>> { { "fi", new List<VmPhone> { CreateVmPhoneModel() } } },
                Attachments = new Dictionary<string, List<VmChannelAttachment>> { { "fi", new List<VmChannelAttachment> { CreateVmChannelUrlModel(), CreateVmChannelAttachmentModel() } } },
                Languages = itemListGenerator.CreateList("fi;sv;en", x => x.GetGuid())?.ToList(),
                OpeningHours = new VmOpeningHours
                {
                    ExceptionHours = new List<VmExceptionalHours>
                    {
                        CreateVmHoursModel<VmExceptionalHours>(ServiceHoursTypeEnum.Exception)
                    },
                    StandardHours = new List<VmNormalHours>
                    {
                        CreateVmHoursModel<VmNormalHours>(ServiceHoursTypeEnum.Standard)
                    },
                    SpecialHours = new List<VmSpecialHours>
                    {
                        CreateVmHoursModel<VmSpecialHours>(ServiceHoursTypeEnum.Special)
                    }
                },
                AccessibilityClassifications = isAccessibiltyClassificationSet ?
                    new Dictionary<string, VmAccessibilityClassification> {{ "fi", CreateAccessibilityClassificationModel()}} :
                    null
            };
            SetupVmChannelDescriptionModel(step1);
            return step1;
        }

        public static VmWebPageChannel CreateVmWebPageChannelModel()
        {
            var step1 = new VmWebPageChannel
            {
                Emails = new Dictionary<string, List<VmEmailData>> { { "fi", new List<VmEmailData>  { CreateVmEmailDataModel() } } },
//                PhoneNumbers = new List<VmPhone> { CreateVmPhoneModel() },
//                Languages = new List<Guid>() { Guid.NewGuid() },
//                WebPage = new VmWebPage()
            };
            SetupVmChannelDescriptionModel(step1);
            return new VmWebPageChannel
            {
//                PublishingStatusId = Guid.NewGuid(),
//                Step1Form = step1,
            };
        }

//        public static VmPrintableFormChannel CreateVmPrintableFormChannelModel()
//        {
//            var step1 = new VmPrintableFormChannelStep1()
//            {
//                PhoneNumbers = new List<VmPhone> { CreateVmPhoneModel() },
//                Emails = new List<VmEmailData> { CreateVmEmailDataModel() },
//                FormIdentifier = new Dictionary<string, string>() { { "fi", "Form Identifier" } },
//                FormReceiver = new Dictionary<string, string>() { { "fi", "Form Receiver" } },
//                DeliveryAddress = CreateVmAdressSimpleModel(),
//                WebPages = new List<VmWebPage>()
//                {
//                    CreateVmWebPageUrlModel()
//                },
//                UrlAttachments = new List<VmChannelAttachment>()
//                {
//                    CreateVmChannelAttachmentModel()
//                }
//            };
//            SetupVmChannelDescriptionModel(step1);
//            return new VmPrintableFormChannel()
//            {
//                PublishingStatusId = Guid.NewGuid(),
//                Step1Form = step1,
//            };
//        }

        public static VmAddressSimple CreateVmAdressSimpleModel()
        {
            var poBox = new Dictionary<string, string>();
            poBox.Add("fi", "pobox");
            return new VmAddressSimple
            {
                PoBox = poBox,
                PostalCode = new VmPostalCode { Id = Guid.NewGuid() },
                //                Street = "street",
                StreetType = "StreetType",
                AddressCharacter = AddressCharacterEnum.Visiting,
            };
        }

        public static VmWebPage CreateVmWebPageUrlModel()
        {
            return new VmWebPage
            {
                Name = "UrlName",
                TypeId = Guid.NewGuid(),
                UrlAddress = "url"
            };
        }

        public static VmStringText CreateVmStringTextodel()
        {
            return new VmStringText
            {
                Text = "Text"
            };
        }

        public static VmEmailData CreateVmEmailDataModel()
        {
            return new VmEmailData
            {
                AdditionalInformation = "additionalInfo",
                Email = "test@test.com",
                OwnerReferenceId = Guid.NewGuid(),
            };
        }

        public static VmAccessibilityClassification CreateAccessibilityClassificationModel()
        {
            return new VmAccessibilityClassification
            {
                AccessibilityClassificationLevelTypeId = AccessibilityClassificationLevelTypeEnum.Unknown.ToString().GetGuid(),
                WcagLevelTypeId = Guid.NewGuid(),
                Name = "urlName",
                UrlAddress = "https://www.urlAddress.com/",
                LocalizationId = "fi".GetGuid(),
                OwnerReferenceId = Guid.NewGuid()
            };
        }

//        public static VmJsonOrganization CreateVmJsonOrganizationModel()
//        {
//            return new VmJsonOrganization()
//            {
//                BusinessID = Guid.NewGuid().ToString(),
//                Identifier = Guid.NewGuid().ToString(),
//                Name = "Name",
//                MunicipalityCode = "00123",
//                OrganizationType = OrganizationTypeEnum.Company.ToString(),
//            };
//        }
    }
}
