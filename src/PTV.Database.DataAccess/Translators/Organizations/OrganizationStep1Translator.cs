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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<Organization, VmOrganizationStep1>), RegisterType.Transient)]
    internal class OrganizationStep1Translator : Translator<Organization, VmOrganizationStep1>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;

        public OrganizationStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmOrganizationStep1 TranslateEntityToVm(Organization entity)
        {
            var step = CreateEntityViewModelDefinition<VmOrganizationStep1>(entity)

                    .AddSimple(input => input.Id, output => output.Id)
                    .AddSimple(input => input.TypeId, output => output.OrganizationTypeId)
                    .AddSimple(input => input.ParentId, output => output.ParentId)
                    .AddNavigation(input => GetName(input, NameTypeEnum.Name), output => output.OrganizationName)
                    .AddNavigation(input => GetName(input, NameTypeEnum.AlternateName), output => output.OrganizationAlternateName)
                    .AddNavigation(input => input.Oid, output => output.OrganizationId)
                    .AddSimple(input => typesCache.Compare<NameType>(input.DisplayNameTypeId, NameTypeEnum.AlternateName.ToString()), output => output.IsAlternateNameUsedAsDisplayName)
                    .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description)
                    .AddNavigation(input => input.Municipality, output => output.Municipality)
                    .AddNavigation(input => input.Business, output => output.Business)
                    .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatus)

                    .AddCollection(input => input.OrganizationEmails.Select(x => x.Email), output => output.Emails)
                    .AddCollection(input => input.OrganizationPhones.Select(x => x.Phone), output => output.PhoneNumbers)
                    .AddCollection(input => input.OrganizationWebAddress.OrderBy(x => x.WebPage.OrderNumber), output => output.WebPages)

                    .AddCollection(input => input.OrganizationAddresses.Where(x => x.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Visiting.ToString())).Select(x => x.Address), output => output.VisitingAddresses)
                    .AddCollection(input => input.OrganizationAddresses.Where(x => x.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Postal.ToString())).Select(x => x.Address), output => output.PostalAddresses)

                    .GetFinal();

            step.ShowContacts = step.Emails.Count > 0 || step.PhoneNumbers.Count > 0 || step.WebPages.Count > 0 || step.PostalAddresses.Count > 0 || step.VisitingAddresses.Count > 0;
            step.ShowPostalAddress = step.PostalAddresses.Count > 0;
            step.ShowVisitingAddress = step.VisitingAddresses.Count > 0;


            return step;
        }

        private string GetName(Organization organization, NameTypeEnum type)
        {
            return organization.OrganizationNames.FirstOrDefault(x => x.TypeId == typesCache.Get<NameType>(type.ToString()))?.Name ?? string.Empty;
        }

        private string GetDescription(Organization organization, DescriptionTypeEnum type)
        {
            return languageCache.Filter(organization.OrganizationDescriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString())), RequestLanguageCode)?.Description ?? string.Empty;

        }

        public override Organization TranslateVmToEntity(VmOrganizationStep1 vModel)
        {
            throw new NotImplementedException();
        }
    }
}
