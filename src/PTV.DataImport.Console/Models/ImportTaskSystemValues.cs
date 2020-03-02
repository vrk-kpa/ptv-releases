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

using System.Collections.Generic;
using PTV.Database.Model.Models;

namespace PTV.DataImport.Console.Models
{
    internal class ImportTaskSystemValues
    {
        public ImportTaskSystemValues()
        {
            PublishingStatuses = new SysPublishingStatuses();
            AddressCharacters = new SysAddressCharacters();
            AddressTypes = new SysAddressTypes();
            WebpageTypes = new SysWebpageTypes();
            PhoneNumberTypes = new SysPhoneNumberTypes();
            NameTypes = new SysNameTypes();
            DescriptionTypes = new SysDescriptionTypes();
            OrganizationTypes = new SysOrganizationTypes();
            ServiceChannelTypes = new SysServiceChannelTypes();
            ServiceChargeTypes = new SysServiceChargeTypes();
            ServiceHourTypes = new SysServiceHourTypes();
            ExceptionHoursStatusTypes = new SysExceptionHoursStatusTypes();
            PrintableFormChannelUrlTypes = new SysPrintableFormChannelUrlTypes();
            //ServiceCoverageTypes = new SysServiceCoverageTypes();
            ProvisionTypes = new SysProvisonTypes();
            AreaInformationTypes = new SysAreaInformationTypes();
            AreaTypes = new SysAreaTypes();
            ServiceChannelConnectionType = new SysServiceChannelConnectionType();
        }

        public SysPublishingStatuses PublishingStatuses { get; private set; }

        public SysAddressCharacters AddressCharacters { get; private set; }
        public SysAddressTypes AddressTypes { get; private set; }

        public SysWebpageTypes WebpageTypes { get; private set; }

        public SysPhoneNumberTypes PhoneNumberTypes { get; private set; }

        public SysNameTypes NameTypes { get; private set; }

        public SysDescriptionTypes DescriptionTypes { get; private set; }

        public SysOrganizationTypes OrganizationTypes { get; private set; }

        public SysServiceChannelTypes ServiceChannelTypes { get; private set; }

        public SysServiceChargeTypes ServiceChargeTypes { get; private set; }

        public SysServiceHourTypes ServiceHourTypes { get; private set; }

        public SysExceptionHoursStatusTypes ExceptionHoursStatusTypes { get; private set; }

        public SysPrintableFormChannelUrlTypes PrintableFormChannelUrlTypes { get; private set; }

        //public SysServiceCoverageTypes ServiceCoverageTypes { get; private set; }

        public SysProvisonTypes ProvisionTypes { get; private set; }

        public SysAreaInformationTypes AreaInformationTypes { get; private set; }

        public SysAreaTypes AreaTypes { get; private set; }

        public Language DefaultLanguage { get; set; }

        public Country DefaultCountry { get; set; }

        public List<Language> Languages { get; set; }

        public SysServiceChannelConnectionType ServiceChannelConnectionType { get; set; }
    }

    internal class SysPublishingStatuses
    {
        public PublishingStatusType PublishedStatusType { get; set; }

        public PublishingStatusType DraftStatusType { get; set; }

        public PublishingStatusType DeletedStatusType { get; set; }
    }

    internal class SysAddressCharacters
    {
        public AddressCharacter Postal { get; set; }
        public AddressCharacter Visiting { get; set; }
    }

    internal class SysAddressTypes
    {
        public AddressType Street { get; set; }
        public AddressType PostOfficeBox { get; set; }
        public AddressType Foreign { get; set; }
    }

    internal class SysWebpageTypes
    {
        public WebPageType Home { get; set; }
        public WebPageType Social { get; set; }
    }

/*ProducerInfo
    internal class SysRoleTypes
    {
        public RoleType Responsible { get; set; }
        public RoleType Producer { get; set; }
    }*/

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

    internal class SysProvisonTypes
    {
        public ProvisionType SelfProduced { get; set; }

        public ProvisionType PurchaseServices { get; set; }

        public ProvisionType Other { get; set; }
    }

    internal class SysAreaInformationTypes
    {
        public AreaInformationType WholeCountry { get; set; }

        public AreaInformationType WholeCountryExceptAlandIslands { get; set; }

        public AreaInformationType AreaType { get; set; }
    }

    internal class SysAreaTypes
    {
        public AreaType Municipality { get; set; }

        public AreaType Province { get; set; }

        public AreaType BusinessRegions { get; set; }

        public AreaType HospitalRegions { get; set; }
    }

    internal class SysServiceChannelConnectionType
    {
        public ServiceChannelConnectionType NotCommon { get; set; }
        public ServiceChannelConnectionType CommonForAll { get; set; }
        public ServiceChannelConnectionType CommonFor { get; set; }
    }
}
