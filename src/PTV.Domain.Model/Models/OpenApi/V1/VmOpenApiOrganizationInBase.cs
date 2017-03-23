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

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View Model of organization for IN api -base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiOrganizationInBase" />
    public class VmOpenApiOrganizationInBase : VmOpenApiOrganizationInVersionBase, IVmOpenApiOrganizationInBase
    {
        /// <summary>
        /// Organization OID. - must match the regex @"^[A-Za-z0-9.-]*$"
        /// </summary>
        [JsonProperty(Order = 2)]
        [RegularExpression(@"^[A-Za-z0-9.-]*$")]
        public new string Oid { get; set; }

        /// <summary>
        /// This property is not used in the API anymore.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 4)]
        public bool StreetAddressAsPostalAddress { get; set; }

        /// <summary>
        /// List of email addresses.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IList<VmOpenApiOrganizationEmailIn> EmailAddresses { get; set; } = new List<VmOpenApiOrganizationEmailIn>();

        /// <summary>
        /// List of phone numbers.
        /// </summary>
        [JsonProperty(Order = 21)]
        [ListWithEnum(typeof(PhoneNumberTypeEnum), "Type")]
        public new IList<VmOpenApiOrganizationPhoneIn> PhoneNumbers { get; set; }

        /// <summary>
        /// List of web pages.
        /// </summary>
        [JsonProperty(Order = 22)]
        [ListWithEnum(typeof(WebPageTypeEnum), "Type")]
        public new IList<VmOpenApiWebPageIn> WebPages { get; set; } = new List<VmOpenApiWebPageIn>();

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<VmOpenApiAddressWithType> Addresses { get; set; } = new List<VmOpenApiAddressWithType>();

        #region methods
        /// <summary>
        /// Gets the Version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 1;
        }

        /// <summary>
        /// Gets the Version base.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiOrganizationInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiOrganizationInVersionBase>();
            vm.Oid = this.Oid.SetStringValueLength(100);
            // convert emails
            if (this.EmailAddresses != null && this.EmailAddresses.Count > 0)
            {
                foreach (var emailAddress in this.EmailAddresses)
                {
                    var descriptions = emailAddress.Descriptions;
                    if (descriptions != null && descriptions.Count > 0)
                    {

                        var descriptionsByLanguage = descriptions
                            .GroupBy(d => d.Language)
                            .ToDictionary(g => g.Key, g => g.Select(d => new { d.Value }).ToList());

                        foreach (var languageDescription in descriptionsByLanguage)
                        {

                            var email = new V4VmOpenApiEmail
                            {
                                Id = emailAddress.Id,
                                OwnerReferenceId = emailAddress.OwnerReferenceId,
                                Value = emailAddress.Email.SetStringValueLength(100),
                                Language = languageDescription.Key
                            };

                            var description = languageDescription.Value.FirstOrDefault();
                            if (description != null)
                            {
                                email.Description = description.Value.SetStringValueLength(100);
                            }

                            vm.EmailAddresses.Add(email);
                        }
                    }
                    else
                    {
                        vm.EmailAddresses.Add(new V4VmOpenApiEmail
                        {
                            Id = emailAddress.Id,
                            OwnerReferenceId = emailAddress.OwnerReferenceId,
                            Value = emailAddress.Email.SetStringValueLength(100)
                        });
                    }
                }
            }

            // convert phone numbers
            if (this.PhoneNumbers != null && this.PhoneNumbers.Count > 0)
            {
                foreach (var phoneNumber in this.PhoneNumbers)
                {
                    var descriptions = phoneNumber.Descriptions;
                    if (descriptions != null && descriptions.Count > 0)
                    {

                        var descriptionsByLanguage = descriptions
                            .GroupBy(d => d.Language)
                            .ToDictionary(g => g.Key, g => g.Select(d => new { Value = d.Value, Type = d.Type }).ToList());

                        foreach (var languageDescription in descriptionsByLanguage)
                        {

                            var phone = new V4VmOpenApiPhone
                            {
                                Id = phoneNumber.Id,
                                OwnerReferenceId = phoneNumber.OwnerReferenceId,
                                PrefixNumber = phoneNumber.PrefixNumber,
                                Number = phoneNumber.Number,
                                ServiceChargeType = phoneNumber.ChargeType,
                                Language = languageDescription.Key
                            };

                            var chargeDescription = languageDescription.Value.FirstOrDefault(d => d.Type == PhoneDescriptionTypeEnum.ChargeDescription.ToString());
                            if (chargeDescription != null)
                            {
                                phone.ChargeDescription = chargeDescription.Value.SetStringValueLength(150);
                            }

                            var additionalInformation = languageDescription.Value.FirstOrDefault(d => d.Type == PhoneDescriptionTypeEnum.AdditionalInformation.ToString());
                            if (additionalInformation != null)
                            {
                                phone.AdditionalInformation = additionalInformation.Value.SetStringValueLength(150);
                            }

                            vm.PhoneNumbers.Add(phone);
                        }
                    }
                    else
                    {
                        vm.PhoneNumbers.Add(new V4VmOpenApiPhone
                        {
                            Id = phoneNumber.Id,
                            OwnerReferenceId = phoneNumber.OwnerReferenceId,
                            PrefixNumber = phoneNumber.PrefixNumber,
                            Number = phoneNumber.Number,
                            ServiceChargeType = phoneNumber.ChargeType
                        });
                    }
                }
            }

            var i = 1;
            WebPages.ForEach(w =>
            {
                var webPage = w.ConvertToWebPageWithOrderNumber();
                webPage.OrderNumber = (i++).ToString();
                vm.WebPages.Add(webPage);
            });

            vm.Addresses = new List<V4VmOpenApiAddressWithTypeIn>();
            this.Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion2()));
            return vm;
        }
        #endregion
    }
}
