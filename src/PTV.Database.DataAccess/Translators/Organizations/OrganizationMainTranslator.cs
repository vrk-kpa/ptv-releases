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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<Organization, VmOrganizationModel>), RegisterType.Transient)]
    internal class OrganizationMainTranslator : Translator<Organization, VmOrganizationModel>
    {
        private ITypesCache typesCache;
        public OrganizationMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmOrganizationModel TranslateEntityToVm(Organization entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.PublishingStatusId, o => o.PublishingStatus)
                .AddNavigation(i => i, o => o.Step1Form)
                .GetFinal();
        }

        public override Organization TranslateVmToEntity(VmOrganizationModel vModel)
        {
            var transaltionDefinition = CreateViewModelEntityDefinition<Organization>(vModel)
            .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
            .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id);

            if (vModel.PublishingStatus != null)
            {
                transaltionDefinition.AddSimple(i => i.PublishingStatus, o => o.PublishingStatusId);
            }

            if (vModel.Step1Form != null)
            {
                SetStep1Translation(transaltionDefinition, vModel);
            }

            var entity = transaltionDefinition.GetFinal();

            return entity;
        }


        private void SetStep1Translation(ITranslationDefinitions<VmOrganizationModel, Organization> definition, VmOrganizationModel organization)
        {
            var model = organization.Step1Form;
            var names = new List<VmName>()
            {
                new VmName {Name = model.OrganizationName, TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()), OwnerReferenceId = organization.Id},
                new VmName {Name = model.OrganizationAlternateName, TypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()), OwnerReferenceId = organization.Id}
            };

            var descriptions = new List<VmDescription>()
            {
                new VmDescription { Description = model.Description, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()), OwnerReferenceId = organization.Id},
            };

            if (model.Business != null && !string.IsNullOrEmpty(model.Business.Code))
            {

                definition.AddNavigation(i => i.Step1Form.Business, o => o.Business);
            }

            definition
            .AddSimple(i => i.Step1Form.OrganizationTypeId.Value, o => o.TypeId)
            .AddSimple(i => i.Step1Form.ParentId, o => o.ParentId)
            .AddSimple(i => i.Step1Form.MunicipalityId, o => o.MunicipalityId)
            .AddSimple(i => i.Step1Form.DisplayNameId, o => o.DisplayNameTypeId)
            .AddNavigation(i => i.Step1Form.OrganizationId, o => o.Oid)
            .AddCollection(i => names, o => o.OrganizationNames)
            .AddCollection(i => descriptions, o => o.OrganizationDescriptions);

            //model.ShowContacts
            definition
            .AddCollection(i => i.Step1Form.Emails, o => o.OrganizationEmails)
            .AddCollection(i => i.Step1Form.PhoneNumbers, o => o.OrganizationPhones)
            .AddCollection(i => i.Step1Form.WebPages, o => o.OrganizationWebAddress);

            //model.ShowPostalAddress || model.ShowVisitingAddress)
            var addresses = new List<VmAddressSimple>();
            addresses = model.PostalAddresses.Any() ? addresses.Concat(model.PostalAddresses).ToList() : addresses;
            addresses = model.VisitingAddresses.Any() ? addresses.Concat(model.VisitingAddresses).ToList() : addresses;
            addresses.ForEach(x => x.OwnerReferenceId = organization.Id);
            definition.AddCollection(i => addresses, o => o.OrganizationAddresses);
        }
    }
};