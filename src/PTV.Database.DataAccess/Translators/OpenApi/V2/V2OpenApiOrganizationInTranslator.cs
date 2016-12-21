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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiOrganizationInBase, IVmOpenApiOrganizationInBase>), RegisterType.Transient)]
    internal class V2OpenApiOrganizationInTranslator : Translator<V2VmOpenApiOrganizationInBase, IVmOpenApiOrganizationInBase>
    {
        public V2OpenApiOrganizationInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IVmOpenApiOrganizationInBase TranslateEntityToVm(V2VmOpenApiOrganizationInBase entity)
        {
            throw new NotImplementedException("Translator V2VmOpenApiOrganizationInBase -> IVmOpenApiOrganizationInBase is not implemented.");
        }

        public override V2VmOpenApiOrganizationInBase TranslateVmToEntity(IVmOpenApiOrganizationInBase vModel)
        {

            var vm = CreateViewModelEntityDefinition(vModel)
                .AddNavigation(i => i.SourceId, o => o.SourceId)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Oid, o => o.Oid)
                .AddNavigation(i => i.ParentOrganizationId, o => o.ParentOrganizationId)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddNavigation(i => i.OrganizationType, o => o.OrganizationType)
                .AddNavigation(i => i.BusinessCode, o => o.BusinessCode)
                .AddNavigation(i => i.BusinessName, o => o.BusinessName)
                .AddNavigation(i => i.DisplayNameType, o => o.DisplayNameType)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .AddSimple(i => i.BusinessId, o => o.BusinessId)
                .AddSimple(i => i.DeleteAllEmails, o => o.DeleteAllEmails)
                .AddSimple(i => i.DeleteAllPhones, o => o.DeleteAllPhones)
                .AddSimple(i => i.DeleteAllWebPages, o => o.DeleteAllWebPages)
                .AddSimple(i => i.DeleteAllAddresses, o => o.DeleteAllAddresses)
                .GetFinal();

            // translate emails
            if (vModel.EmailAddresses != null && vModel.EmailAddresses.Count > 0)
            {
                foreach (var emailAddress in vModel.EmailAddresses)
                {
                    var descriptions = emailAddress.Descriptions;
                    if (descriptions != null && descriptions.Count > 0)
                    {

                        var descriptionsByLanguage = descriptions
                            .GroupBy(d => d.Language)
                            .ToDictionary(g => g.Key, g => g.Select(d => new {d.Value}).ToList());

                        foreach (var languageDescription in descriptionsByLanguage)
                        {

                            var email = new VmOpenApiEmail
                            {
                                Id = emailAddress.Id,
                                OwnerReferenceId = emailAddress.OwnerReferenceId,
                                Value = emailAddress.Email,
                                Language = languageDescription.Key
                            };

                            var description = languageDescription.Value.FirstOrDefault();
                            if (description != null)
                            {
                                email.Description = description.Value;
                            }

                            vm.EmailAddresses.Add(email);
                        }
                    }
                    else
                    {
                        vm.EmailAddresses.Add(new VmOpenApiEmail
                        {
                            Id = emailAddress.Id,
                            OwnerReferenceId = emailAddress.OwnerReferenceId,
                            Value = emailAddress.Email
                        });
                    }
                }
            }

            // translate phone numbers
            if (vModel.PhoneNumbers != null && vModel.PhoneNumbers.Count > 0)
            {
                foreach (var phoneNumber in vModel.PhoneNumbers)
                {
                    var descriptions = phoneNumber.Descriptions;
                    if (descriptions != null && descriptions.Count > 0)
                    {

                        var descriptionsByLanguage = descriptions
                            .GroupBy(d => d.Language)
                            .ToDictionary(g => g.Key, g => g.Select(d => new { Value = d.Value, Type = d.Type }).ToList());

                        foreach (var languageDescription in descriptionsByLanguage)
                        {

                            var phone = new VmOpenApiPhone
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
                                phone.ChargeDescription = chargeDescription.Value;
                            }

                            var additionalInformation = languageDescription.Value.FirstOrDefault(d => d.Type == PhoneDescriptionTypeEnum.AdditionalInformation.ToString());
                            if (additionalInformation != null)
                            {
                                phone.AdditionalInformation = additionalInformation.Value;
                            }

                            vm.PhoneNumbers.Add(phone);
                        }
                    }
                    else
                    {
                        vm.PhoneNumbers.Add(new VmOpenApiPhone
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
            vm.OrganizationNames = vModel.OrganizationNames;
            vm.OrganizationDescriptions = vModel.OrganizationDescriptions;
            vm.WebPages = vModel.WebPages;
            vm.Addresses = new List<V2VmOpenApiAddressWithType>();
            vModel.Addresses.ForEach(a => vm.Addresses.Add(a));
            return vm;
        }
    }
}
