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
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Utils
{
    public static class OrganizationFundingTypeChecker
    {
        private static readonly Dictionary<OrganizationTypeEnum, ServiceFundingTypeEnum[]> AllowedFundingTypes = new Dictionary<OrganizationTypeEnum, ServiceFundingTypeEnum[]>
        {
            {OrganizationTypeEnum.Company, new[] {ServiceFundingTypeEnum.MarketFunded} },
            {OrganizationTypeEnum.Municipality, new[] {ServiceFundingTypeEnum.PubliclyFunded} },
            {OrganizationTypeEnum.Organization, new[] {ServiceFundingTypeEnum.MarketFunded, ServiceFundingTypeEnum.PubliclyFunded} },
            {OrganizationTypeEnum.State, new[] {ServiceFundingTypeEnum.PubliclyFunded} },
            {OrganizationTypeEnum.RegionalOrganization, new[] {ServiceFundingTypeEnum.PubliclyFunded} }
        };

        public static bool IsAllowed(OrganizationTypeEnum organizationType, ServiceFundingTypeEnum fundingType)
        {
            return AllowedFundingTypes[organizationType].Contains(fundingType);
        }

        public static bool IsAllowed(string organizationType, string fundingType)
        {
            if (Enum.TryParse<OrganizationTypeEnum>(organizationType, out var organizationValue) &&
                Enum.TryParse<ServiceFundingTypeEnum>(fundingType, out var fundingValue))
            {
                return IsAllowed(organizationValue, fundingValue);
            }

            throw new KeyNotFoundException($"Keys {organizationType} or {fundingType} not found.");
        }

        public static bool CanBeChanged(OrganizationTypeEnum organizationType)
        {
            return AllowedFundingTypes[organizationType].Length > 1;
        }

        public static bool CanBeChanged(string organizationType)
        {
            if (Enum.TryParse<OrganizationTypeEnum>(organizationType, out var organizationValue))
            {
                return CanBeChanged(organizationValue);
            }

            throw new KeyNotFoundException($"Keys {organizationType} not found.");
        }

        public static ServiceFundingTypeEnum GetDefaultFundingTypeId(OrganizationTypeEnum organizationType)
        {
            return AllowedFundingTypes[organizationType].FirstOrDefault();
        }

        public static string GetDefaultFundingTypeId(string organizationType)
        {
            if (Enum.TryParse<OrganizationTypeEnum>(organizationType, out var organizationValue))
            {
                return GetDefaultFundingTypeId(organizationValue).ToString();
            }

            throw new KeyNotFoundException($"Keys {organizationType} not found.");
        }
    }
}
