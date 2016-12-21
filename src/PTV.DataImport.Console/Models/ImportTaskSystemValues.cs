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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.DataImport.ConsoleApp.Models
{
    internal class ImportTaskSystemValues
    {
        public ImportTaskSystemValues()
        {
            PublishingStatuses = new SysPublishingStatuses();
            AddressTypes = new SysAddressTypes();
            WebpageTypes = new SysWebpageTypes();
            RoleTypes = new SysRoleTypes();
            PhoneNumberTypes = new SysPhoneNumberTypes();
            NameTypes = new SysNameTypes();
            DescriptionTypes = new SysDescriptionTypes();
            OrganizationTypes = new SysOrganizationTypes();
            ServiceChannelTypes = new SysServiceChannelTypes();
            ServiceChargeTypes = new SysServiceChargeTypes();
            ServiceHourTypes = new SysServiceHourTypes();
            ExceptionHoursStatusTypes = new SysExceptionHoursStatusTypes();
            PrintableFormChannelUrlTypes = new SysPrintableFormChannelUrlTypes();
            ServiceCoverageTypes = new SysServiceCoverageTypes();
            ProvisionTypes = new SysProvisonTypes();
        }

        public SysPublishingStatuses PublishingStatuses { get; private set; }

        public SysAddressTypes AddressTypes { get; private set; }

        public SysWebpageTypes WebpageTypes { get; private set; }

        public SysRoleTypes RoleTypes { get; private set; }

        public SysPhoneNumberTypes PhoneNumberTypes { get; private set; }

        public SysNameTypes NameTypes { get; private set; }

        public SysDescriptionTypes DescriptionTypes { get; private set; }

        public SysOrganizationTypes OrganizationTypes { get; private set; }

        public SysServiceChannelTypes ServiceChannelTypes { get; private set; }

        public SysServiceChargeTypes ServiceChargeTypes { get; private set; }

        public SysServiceHourTypes ServiceHourTypes { get; private set; }

        public SysExceptionHoursStatusTypes ExceptionHoursStatusTypes { get; private set; }

        public SysPrintableFormChannelUrlTypes PrintableFormChannelUrlTypes { get; private set; }

        public SysServiceCoverageTypes ServiceCoverageTypes { get; private set; }

        public SysProvisonTypes ProvisionTypes { get; private set; }

        public Language DefaultLanguage { get; set; }

        public Country DefaultCountry { get; set; }

        public List<Language> Languages { get; set; }
    }

    internal class SysPublishingStatuses
    {
        public PublishingStatusType PublishedStatusType { get; set; }

        public PublishingStatusType DraftStatusType { get; set; }

        public PublishingStatusType DeletedStatusType { get; set; }
    }

    internal class SysAddressTypes
    {
        public AddressType Postal { get; set; }
        public AddressType Visiting { get; set; }
    }

    internal class SysWebpageTypes
    {
        public WebPageType Home { get; set; }
        public WebPageType Social { get; set; }
    }

    internal class SysRoleTypes
    {
        public RoleType Responsible { get; set; }
        public RoleType Producer { get; set; }
    }

    internal class SysPhoneNumberTypes
    {
        public PhoneNumberType Phone { get; set; }
        public PhoneNumberType Sms { get; set; }
        public PhoneNumberType Fax { get; set; }
    }

    internal class SysNameTypes
    {
        public NameType Name { get; set; }
        public NameType AlternateName { get; set; }
    }

    internal class SysDescriptionTypes
    {
        public DescriptionType Description { get; set; }
        public DescriptionType ShortDescription { get; set; }
        public DescriptionType ServiceUserInstruction { get; set; }
    }

    internal class SysOrganizationTypes
    {
        public OrganizationType State { get; set; }
        public OrganizationType Municipality { get; set; }
        public OrganizationType RegionalOrganization { get; set; }
        public OrganizationType Organization { get; set; }
        public OrganizationType Company { get; set; }
    }

    internal class SysServiceChannelTypes
    {
        public ServiceChannelType EChannel { get; set; }
        public ServiceChannelType Webpage { get; set; }
        public ServiceChannelType PrintableForm { get; set; }
        public ServiceChannelType Phone { get; set; }
        public ServiceChannelType ServiceLocation { get; set; }
    }

    internal class SysServiceChargeTypes
    {
        public ServiceChargeType Other { get; set; }
        public ServiceChargeType Free { get; set; }
        public ServiceChargeType Charged { get; set; }
    }

    internal class SysServiceHourTypes
    {
        public ServiceHourType Standard { get; set; }

        public ServiceHourType Exception { get; set; }
    }

    internal class SysExceptionHoursStatusTypes
    {
        public ExceptionHoursStatusType Open { get; set; }

        public ExceptionHoursStatusType Closed { get; set; }
    }

    internal class SysPrintableFormChannelUrlTypes
    {
        public PrintableFormChannelUrlType Word { get; set; }

        public PrintableFormChannelUrlType Excel { get; set; }

        public PrintableFormChannelUrlType Pdf { get; set; }
    }

    internal class SysServiceCoverageTypes
    {
        public ServiceCoverageType Local { get; set; }

        public ServiceCoverageType Nationwide { get; set; }
    }

    internal class SysProvisonTypes
    {
        public ProvisionType SelfProduced { get; set; }

        public ProvisionType VoucherServices { get; set; }

        public ProvisionType PurchaseServices { get; set; }

        public ProvisionType Other { get; set; }
    }
}
