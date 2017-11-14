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
* THE SOFTWARE.C:\Projects\PTV_TEST\src\PTV.Database.DataAccess\Services\Security\
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(IBaseValidationChecker<OrganizationVersioned>), RegisterType.Transient)]
    internal class OrganizationValidationChecker : BaseValidationChecker<OrganizationVersioned>
    {
        private readonly ITypesCache typesCache;

        public OrganizationValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override OrganizationVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            return GetEntity<OrganizationVersioned>(id, unitOfWork,
                q => q.Include(i => i.OrganizationNames)
                      .Include(i => i.OrganizationDescriptions)
                      .Include(i => i.LanguageAvailabilities)
                      .Include(i => i.OrganizationAreas)
                      .Include(i => i.OrganizationAreaMunicipalities)
                      .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                      .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                      .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                      .Include(i => i.OrganizationPhones).ThenInclude(i => i.Phone)
                      .Include(i => i.OrganizationWebAddress).ThenInclude(i => i.WebPage)
                      .Include(i => i.OrganizationEmails).ThenInclude(i => i.Email)
                );
        }

        public override Dictionary<Guid, List<ValidationMessage>> ValidateEntity(Guid? language)
        {
            foreach (var entityLanguageId in entityOrPublishedLanguagesAvailabilityIds)
            {
                SetValidationLanguage(entityLanguageId);

                NotEmptyString("name",
                    x => x.OrganizationNames
                        .Where(y => y.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) && y.LocalizationId == entityLanguageId)
                        .Select(y => y.Name).FirstOrDefault());

                NotEmptyString("shortDescription",
                    x => x.OrganizationDescriptions
                        .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) && y.LocalizationId == entityLanguageId)
                        .Select(y => y.Description)
                        .FirstOrDefault());

                if (!NotEmptyGuid("organizationType", x => x.TypeId))
                {
                    CheckBySelectedOrganizationType(entity.TypeId.Value);
                }

                //emails
                foreach (var email in entity.OrganizationEmails.Where(x => x.Email?.LocalizationId == entityLanguageId).Select(x => x.Email))
                {
                    CheckEntityWithMergeResult(email);
                }

                //phones
                foreach (var phone in entity.OrganizationPhones.Where(x => x.Phone?.LocalizationId == entityLanguageId).Select(x => x.Phone))
                {
                    CheckEntityWithMergeResult(phone);
                }

                //websites
                foreach (var webAddress in entity.OrganizationWebAddress.Where(x => x.WebPage?.LocalizationId == entityLanguageId).Select(x => x.WebPage))
                {
                    CheckEntityWithMergeResult(webAddress);
                }

                //visiting Addresses
                foreach (var visitingAddress in entity.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString())).Select(x => x.Address))
                {
                    CheckEntityWithMergeResult(visitingAddress);
                }

                //postal Addresses
                foreach (var postalAddress in entity.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())).Select(x => x.Address))
                {
                    CheckEntityWithMergeResult(postalAddress);
                }

            }

            return validationMessagesDictionary;
        }

        private void CheckBySelectedOrganizationType(Guid typeId)
        {
            var organizationType = typesCache.GetByValue<OrganizationType>(typeId);
            var organizationTypeEnum = Enum.Parse(typeof(OrganizationTypeEnum), organizationType);

            switch (organizationTypeEnum)
            {
                case OrganizationTypeEnum.RegionalOrganization:
                    NotBeTrue("areaType", x => !(x.OrganizationAreas.Any() || x.OrganizationAreaMunicipalities.Any()));
                    break;
                case OrganizationTypeEnum.Municipality:
                    NotEmptyGuid("municipality", x => x.MunicipalityId);
                    break;
                case OrganizationTypeEnum.State:
                case OrganizationTypeEnum.Company:
                case OrganizationTypeEnum.Organization:
                    if (!NotEmptyGuid("areaInformationType", x => x.AreaInformationTypeId))
                    {
                        if (entity.AreaInformationTypeId ==
                            typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString()))
                        {
                            NotBeTrue("areaType", x => !(x.OrganizationAreas.Any() || x.OrganizationAreaMunicipalities.Any()));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
