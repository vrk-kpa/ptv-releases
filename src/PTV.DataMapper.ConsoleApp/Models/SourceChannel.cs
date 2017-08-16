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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PTV.DataMapper.ConsoleApp.Models
{
    public class SourceChannel : SourceBase<V2VmOpenApiServiceLocationChannelIn>
    {
        public int id { get; set; }
        public int org_id { get; set; }
        public string dept_id { get; set; }
        public bool org_owned { get; set; }
        public string data_source_url { get; set; }
        public int provider_type { get; set; }
        public int[] service_ids { get; set; }
        public object[] sources { get; set; }
        public string name_fi { get; set; }
        public string name_sv { get; set; }
        public string name_en { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int northing_etrs_gk25 { get; set; }
        public int easting_etrs_gk25 { get; set; }
        public bool manual_coordinates { get; set; }
        public string street_address_fi { get; set; }
        public string street_address_sv { get; set; }
        public string street_address_en { get; set; }
        public string address_zip { get; set; }
        public string address_city_fi { get; set; }
        public string address_city_sv { get; set; }
        public string address_city_en { get; set; }
        public int citydistrict_id { get; set; }
        public string citydistrict_name { get; set; }
        public int subdistrict_id { get; set; }
        public string subdistrict_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string www_fi { get; set; }
        public string www_sv { get; set; }
        public string www_en { get; set; }
        public string address_postal_full { get; set; }
        public string address_postal_full_fi { get; set; }
        public string address_postal_full_sv { get; set; }
        public string address_postal_full_en { get; set; }
        public object[] connections { get; set; }
        public object[] events { get; set; }
        public object[] accessibility_sentences { get; set; }
        public object[] service_descriptions { get; set; }

        public override V2VmOpenApiServiceLocationChannelIn ConvertToVm(string orgId, string code, int id = 0)
        {
            var langFi = LanguageCode.fi.ToString();
            var vm =  new V2VmOpenApiServiceLocationChannelIn()
            {
                SourceId = this.id.ToString(),
                OrganizationId = orgId,
                ServiceChannelNames = new List<VmOpenApiLanguageItem>()
                {
                    new VmOpenApiLanguageItem() { Value = GetStringWithMaxLength(name_fi, 100), Language = langFi }
                },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem>()
                {
                    new VmOpenApiLocalizedListItem() { Value = name_fi, Language = langFi, Type = DescriptionTypeEnum.Description.ToString() },
                    new VmOpenApiLocalizedListItem() { Value = GetStringWithMaxLength(name_fi, 150), Language = langFi, Type = DescriptionTypeEnum.ShortDescription.ToString() },
                },
                ServiceAreaRestricted = true,
                ServiceAreas = new List<string>() { code },
                Languages = new List<string>() { langFi, LanguageCode.en.ToString(), LanguageCode.sv.ToString() },
                Addresses = new List<V2VmOpenApiAddressWithType>()
                {
                    new V2VmOpenApiAddressWithType()
                    {
                        Type = AddressTypeEnum.Visiting.ToString(),
                        PostalCode = address_zip,
                        StreetAddress = new List<VmOpenApiLanguageItem>() { new VmOpenApiLanguageItem() { Value = street_address_fi, Language = langFi } },
                        Country = "FI",
                    }
                },
                PublishingStatus = PublishingStatus.Draft.ToString()
            };

            // Email
            if (!string.IsNullOrEmpty(email))
            {
                var validEmail = GetValidEmail(email);
                if (!string.IsNullOrEmpty(validEmail))
                {
                    vm.SupportEmails = new List<VmOpenApiLanguageItem>() { new VmOpenApiLanguageItem() { Value = validEmail, Language = langFi } };
                }
            }

            // Phone number
            if (!string.IsNullOrEmpty(phone))
            {
                var validPhone = GetValidPhone(phone);
                if (!string.IsNullOrEmpty(validPhone))
                {
                    vm.PhoneNumbers = new List<VmOpenApiPhone>() { new VmOpenApiPhone() { Number = validPhone, Language = langFi } };
                }
            }

            if (!string.IsNullOrEmpty(www_fi))
            {
                vm.WebPages.Add(new VmOpenApiWebPage() {
                    Value = name_fi + " kotisivu",
                    Url = www_fi,
                    //Type = WebPageTypeEnum.HomePage.ToString(),
                    Language = langFi
                });
            }

            if (!string.IsNullOrEmpty(address_postal_full))
            {
                var address = address_postal_full.Split(',');
                if (address.Count() < 2)
                {
                    ErrorMsg = $"Postal address does not contain all needed elements { address_postal_full }";
                }
                else
                {
                    Regex regex = new Regex(@"(\d{5})", RegexOptions.Compiled | RegexOptions.CultureInvariant);
                    Match match = regex.Match(address[1]);
                    if (!match.Success)
                    {
                        ErrorMsg = $"Postal address does not contain all needed elements { address_postal_full }";
                    }
                    vm.Addresses.Add(new V2VmOpenApiAddressWithType()
                    {
                        Type = AddressTypeEnum.Postal.ToString(),
                        PostOfficeBox = address[0],
                        PostalCode = match.Groups[1].Value,
                        Country = "FI",
                    });
                }
            }
            return vm;
        }

        private string GetValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || email.Contains("@")) return email;
            email = email.Replace("(a)", "@");
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                ErrorMsg = $"Not a valid email address { email }.";
                return null;
            }
            return email;
        }

        private string GetValidPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) { return null; }

            // For testing purposes comment out
            //if (phone.Contains(",")) ErrorMsg = $"Phone number { phone }.";
            Regex regex = new Regex(@"^[+?\d\s]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            Match match = regex.Match(phone);
            if (match.Success)
            {
                return match.Value;
            }
            ErrorMsg = $"Could not get valid phone number from { phone }.";
            return null;
        }
    }
}
