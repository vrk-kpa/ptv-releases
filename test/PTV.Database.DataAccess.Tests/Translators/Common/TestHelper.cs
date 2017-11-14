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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Common
{
    public static class TestHelper
    {

        public static VmChannelAttachment CreateVmChannelAttachmentModel()
        {
            return new VmChannelAttachment()
            {
                Id = "xxx".GetGuid(),
                UrlAddress = "attachmentUrl",
                Description = "attachmentDescription",
                Name = "attachmentName"
            };
        }
        public static VmChannelAttachment CreateVmChannelUrlModel()
        {
            return new VmChannelAttachment()
            {
                Id = "initial".GetGuid(),
                UrlAddress = "attachmentUrl",
            };
        }

        public static VmDescription CreateVmDescriptionModel(DescriptionTypeEnum descriptionType)
        {
            return new VmDescription()
            {
                Description = "channelDescription",
                TypeId = Guid.NewGuid()//descriptionType
            };
        }

        public static VmName CreateVmNameModel(NameTypeEnum nameType)
        {
            return new VmName()
            {
                Name = "channelName",
                TypeId = nameType.ToString().GetGuid()
            };
        }

        public static VmPhone CreateVmPhoneModel()
        {
            return new VmPhone()
            {
                ChargeDescription = "description",
                ChargeType = Guid.NewGuid(),
                Number = "456789",
                DialCode = new VmDialCode() { Id = Guid.NewGuid()},
                AdditionalInformation = "info",
                TypeId = Guid.NewGuid()
            };
        }

        public static T CreateVmHoursModel<T>(ServiceHoursTypeEnum serviceHoursTypeEnum ) where T : VmHours, new ()
        {
            return new T()
            {
                AdditionalInformation = "AdditionInfo",
                Id = Guid.Empty,
                ServiceHoursType = serviceHoursTypeEnum,
                ValidFrom = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                ValidTo = DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds(),
//                DailyHours = new List<VmDailyHours>()
            };
        }

        public static void SetupVmChannelDescriptionModel(IVmChannelDescription model)
        {
            model.Description = "Channel description";
            model.Name = "Channel name";
            model.ShortDescription = "Channel short description";
            model.OrganizationId = Guid.NewGuid();
        }

        public static VmElectronicChannel CreateVmElectronicChannelModel()
        {
            var step1 = new VmElectronicChannelStep1()
            {
                IsOnLineAuthentication = true,
                IsOnLineSign = true,
                NumberOfSigns = 10,
                Emails = new List<VmEmailData> { CreateVmEmailDataModel() },
                PhoneNumbers = new List<VmPhone> { CreateVmPhoneModel() }
            };
            SetupVmChannelDescriptionModel(step1);
            return new VmElectronicChannel()
            {
                PublishingStatusId = Guid.NewGuid(),
                Step1Form = step1,
                Step2Form = new VmOpeningHoursStep
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
                UrlAttachments = new List<VmChannelAttachment>()
                {
                    CreateVmChannelUrlModel(),
                    CreateVmChannelAttachmentModel()
                }
            };
        }

        public static VmWebPageChannel CreateVmWebPageChannelModel()
        {
            var step1 = new VmWebPageChannelStep1()
            {
                Emails = new List<VmEmailData> { CreateVmEmailDataModel() },
                PhoneNumbers = new List<VmPhone> { CreateVmPhoneModel() },
                Languages = new List<Guid>() { Guid.NewGuid() },
                WebPage = new VmWebPage()
            };
            SetupVmChannelDescriptionModel(step1);
            return new VmWebPageChannel()
            {
                PublishingStatusId = Guid.NewGuid(),
                Step1Form = step1,
            };
        }

        public static VmPrintableFormChannel CreateVmPrintableFormChannelModel()
        {
            var step1 = new VmPrintableFormChannelStep1()
            {
                PhoneNumbers = new List<VmPhone> { CreateVmPhoneModel() },
                Emails = new List<VmEmailData> { CreateVmEmailDataModel() },
                FormIdentifier = new Dictionary<string, string>() { { "fi", "Form Identifier" } },
                FormReceiver = new Dictionary<string, string>() { { "fi", "Form Receiver" } },
                DeliveryAddress = CreateVmAdressSimpleModel(),
                WebPages = new List<VmWebPage>()
                {
                    CreateVmWebPageUrlModel()
                },
                UrlAttachments = new List<VmChannelAttachment>()
                {
                    CreateVmChannelAttachmentModel()
                }
            };
            SetupVmChannelDescriptionModel(step1);
            return new VmPrintableFormChannel()
            {
                PublishingStatusId = Guid.NewGuid(),
                Step1Form = step1,
            };
        }

        public static VmAddressSimple CreateVmAdressSimpleModel()
        {
            var poBox = new Dictionary<string, string>();
            poBox.Add(LanguageCode.fi.ToString(), "pobox");
            return new VmAddressSimple()
            {
                PoBox = poBox,
                PostalCode = new VmPostalCode() { Id = Guid.NewGuid() },
//                Street = "street",
                StreetType = "StreetType",
                AddressCharacter = AddressCharacterEnum.Visiting,
            };
        }

        public static VmWebPage CreateVmWebPageUrlModel()
        {
            return new VmWebPage()
            {
                Name = "UrlName",
                TypeId = Guid.NewGuid(),
                UrlAddress = "Url"
            };
        }

        public static VmStringText CreateVmStringTextodel()
        {
            return new VmStringText()
            {
               Text = "Text"
            };
        }

        public static VmEmailData CreateVmEmailDataModel()
        {
            return new VmEmailData()
            {
                AdditionalInformation = "additionalInfo",
                Email = "test@test.com",
                OwnerReferenceId = Guid.NewGuid(),
            };
        }

        public static VmJsonOrganization CreateVmJsonOrganizationModel()
        {
            return new VmJsonOrganization()
            {
                BusinessID = Guid.NewGuid().ToString(),
                Identifier = Guid.NewGuid().ToString(),
                Name = "Name",
                MunicipalityCode = "00123",
                OrganizationType = OrganizationTypeEnum.Company.ToString(),
            };
        }
    }
}
