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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Logic;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;
using PTV.Domain.Model.Models.V2.Organization;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IOrganizationService), RegisterType.Transient)]
    internal class OrganizationService : EntityServiceBase<OrganizationVersioned, Organization>, IOrganizationService
    {
        private IContextManager contextManager;
//        private readonly IUserIdentification userIdentification;
//        private ILogger logger;
//        private VmListItemLogic listItemLogic;
//        private readonly DataUtils dataUtils;
//        private VmOwnerReferenceLogic ownerReferenceLogic;
        //private IUrlService urlService;
        private ITypesCache typesCache;
//        private ILanguageCache languageCache;
        private IVersioningManager versionManager;

        public OrganizationService(
            IContextManager contextManager,
 //           IUserIdentification userIdentification,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
 //           ILogger<Services.ChannelService> logger,
            ICommonServiceInternal commonService,
 //           VmListItemLogic listItemLogic,
 //           DataUtils dataUtils,
 //           VmOwnerReferenceLogic ownerReferenceLogic,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versionManager,
            ServiceUtilities utilities,
            IValidationManager validationManager
            ) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, contextManager, utilities, commonService, validationManager)
        {
            this.contextManager = contextManager;
//            this.logger = logger;
//            this.userIdentification = userIdentification;
//            this.listItemLogic = listItemLogic;
//            this.dataUtils = dataUtils;
//            this.ownerReferenceLogic = ownerReferenceLogic;
            this.typesCache = cacheManager.TypesCache;
//            this.languageCache = cacheManager.LanguageCache;
            this.versionManager = versionManager;
        }
        
        public VmOrganizationHeader GetOrganizationHeader(Guid? organizationId)
        {
            return contextManager.ExecuteReader(unitOfWork => GetOrganizationHeader(organizationId, unitOfWork));
        }

        public VmOrganizationHeader GetOrganizationHeader(Guid? organizationId, IUnitOfWork unitOfWork)
        {
            var result = new VmOrganizationHeader();
            OrganizationVersioned entity;
            result = GetModel<OrganizationVersioned, VmOrganizationHeader>(entity = GetEntity<OrganizationVersioned>(organizationId, unitOfWork,
                q => q
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.OrganizationNames)
                    .Include(x => x.Versioning)
            ), unitOfWork);
            result.PreviousInfo = organizationId.HasValue ? Utilities.CheckIsEntityEditable<OrganizationVersioned, Organization>(organizationId.Value, unitOfWork) : null;
            return result;
        }

        public VmOrganizationHeader DeleteOrganization(Guid? organizationId)
        {
            VmOrganizationHeader result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var entity = CascadeDeleteOrganization(unitOfWork, organizationId);
                unitOfWork.Save();
                result = GetOrganizationHeader(organizationId, unitOfWork);
            });
            UnLockOrganization(result.Id.Value);
            return result;
        }

        public VmArchiveResult CheckDeleteOrganization(Guid? organizationId)
        {
            return contextManager.ExecuteWriter(unitOfWork => CascadeDeleteOrganization(unitOfWork, organizationId, true));            
        }

        private VmArchiveResult CascadeDeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id, bool checkDelete = false)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organizationUserMapRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
            var userMaps = organizationUserMapRep.All();
            var organizationIsUsed =
                organizationRep.All()
                    .Where(x => x.Id == id)
                    .Any(
                        i => userMaps.Any(k => k.OrganizationId == i.UnificRootId));
            if (organizationIsUsed)
            {
                throw new OrganizationNotDeleteInUserUseException();
            }

            var organization = organizationRep.All().SingleOrDefault(x => x.Id == id);

            if (checkDelete)
            {
                var organizations = organizationRep.All().Where(x => x.Id == id);
                var result = new VmArchiveResult
                {
                    Id = organization.Id,
                    PublishingStatusId = organization.PublishingStatusId,
                    ChannelsConnected = organizations.Any(i => i.UnificRoot.OrganizationServiceChannelsVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                    SubOrganizationsConnected = organizations.Any(i => i.UnificRoot.Children.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                    ServicesConnected = organizations.Any(i => i.UnificRoot.OrganizationServices.Any(j => j.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceVersioned.PublishingStatusId != psOldPublished))
                                        || organizations.Any(i => i.UnificRoot.ServiceProducerOrganizations.Any(j => j.ServiceProducer.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceProducer.ServiceVersioned.PublishingStatusId != psOldPublished))
                                        || organizations.Any(i => i.UnificRoot.OrganizationServicesVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished))

                };
                if (result.AnyConnected)
                {
                    return result;
                }
            }
            else
            {
                CommonService.ChangeEntityToDeleted<OrganizationVersioned>(unitOfWork, organization.Id);
                ArchiveConnectedChannels(unitOfWork, organization.UnificRootId);
                ArchiveConnectedServices(unitOfWork, organization.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, organization.UnificRootId);
            }
            return new VmArchiveResult { Id = organization.Id, PublishingStatusId = organization.PublishingStatusId };
        }

        private void ArchiveConnectedChannels(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            channelRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                .Where(x => x.OrganizationId == organizationId)
                .ForEach(x => x.SafeCall(i => i.PublishingStatusId = psDeleted));
        }

        private void ArchiveConnectedServices(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var services = serviceRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                .Where(x => x.OrganizationServices.Any(o => o.OrganizationId == organizationId) ||
                            x.ServiceProducers.SelectMany(sp => sp.Organizations).Any(o => o.OrganizationId == organizationId) ||
                            x.OrganizationId == organizationId)
                .Include(x => x.OrganizationServices).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions)
                .Include(x => x.ServiceProducers).ThenInclude(x => x.Organizations).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions)
                .ToList();

            foreach (var service in services)
            {
                var restOrganizations = service.OrganizationServices.Where(x => x.OrganizationId != organizationId && x.Organization.Versions.Any(y => y.PublishingStatusId != psDeleted && y.PublishingStatusId != psOldPublished)).ToList();
                service.OrganizationServices = restOrganizations.ToList();

                var producersToDelete = HandleServiceProducers(service.ServiceProducers, organizationId, psDeleted, psOldPublished);
                if (producersToDelete.Any())
                {
                    producersToDelete.ForEach(p => service.ServiceProducers.Remove(p));

                    var orderNumber = 1;
                    service.ServiceProducers.OrderBy(p => p.OrderNumber).ForEach(p => p.OrderNumber = orderNumber++);
                }
                if (service.OrganizationId == organizationId)
                {
                    service.SafeCall(i => i.PublishingStatusId = psDeleted);
                }
            }
        }

        private List<ServiceProducer> HandleServiceProducers(ICollection<ServiceProducer> serviceProducers, Guid organizationId, Guid psDeleted, Guid psOldPublished)
        {
            var producersToDelete = new List<ServiceProducer>();
            if (serviceProducers == null) return producersToDelete;

            foreach (var producer in serviceProducers)
            {
                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()))
                {
                    var producerOrganizations = producer.Organizations.Where(spo => spo.OrganizationId != organizationId && spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished)).ToList();
                    if (producerOrganizations.Any())
                    {
                        producer.Organizations = producerOrganizations;
                    }
                    else
                    {
                        producersToDelete.Add(producer);
                    }
                }

                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo == null)
                    {
                        producersToDelete.Add(producer);
                    }
                    else
                    {
                        if (spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                        {
                            producersToDelete.Add(producer);
                        }
                    }
                }

                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo != null && spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                    {
                        producersToDelete.Add(producer);
                    }
                }
            }

            return producersToDelete;
        }

        private void ArchiveSubOrganizations(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var subOrgs = organizationRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                .Where(x => x.ParentId == organizationId).ToList();

            foreach (var subOrg in subOrgs)
            {
                subOrg.PublishingStatusId = psDeleted;
                ArchiveConnectedChannels(unitOfWork, subOrg.UnificRootId);
                ArchiveConnectedServices(unitOfWork, subOrg.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, subOrg.UnificRootId);
            }

        }

        public VmOrganizationOutput GetOrganization(VmOrganizationBasic model)
        {
            VmOrganizationOutput result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetOrganization(model, unitOfWork);
            });

            return result;
        }

        private VmOrganizationOutput GetOrganization(VmOrganizationBasic model, IUnitOfWork unitOfWork)
        {
            VmOrganizationOutput result = null;
            SetTranslatorLanguage(model);
            result = GetModel<OrganizationVersioned, VmOrganizationOutput>(GetEntity<OrganizationVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.OrganizationNames)
                        .Include(x => x.OrganizationDescriptions)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.Business)
                        .Include(x => x.Municipality).ThenInclude(x => x.MunicipalityNames)
                        .Include(x => x.OrganizationEmails).ThenInclude(x => x.Email)
                        .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.OrganizationWebAddress).ThenInclude(x => x.WebPage)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressStreets).ThenInclude(x => x.StreetNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(x => x.PostOfficeBoxNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressForeigns).ThenInclude(x => x.ForeignTextNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressStreets).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.Coordinates)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.OrganizationAreas).ThenInclude(x => x.Area)
                        .Include(x => x.OrganizationAreaMunicipalities)
                        .Include(x => x.OrganizationDisplayNameTypes)
                        .Include(x => x.LanguageAvailabilities)
                        .Include(x => x.Versioning)
            ), unitOfWork);

            result.PreviousInfo = result.Id.HasValue ? Utilities.CheckIsEntityEditable<OrganizationVersioned, Organization>(result.Id.Value, unitOfWork) : null;
            return result;
        }

        public VmOrganizationOutput SaveOrganization(VmOrganizationInput model)
        {
            return ExecuteSave
            (
                unitOfWork => SaveOrganization(unitOfWork, model),
                (unitOfWork, entity) => GetOrganization(new VmOrganizationBasic() { Id = entity.Id }, unitOfWork)
            );
        }

        public OrganizationVersioned SaveOrganization(IUnitOfWorkWritable unitOfWork, VmOrganizationInput model)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var unificRootId = versionManager.GetUnificRootId<OrganizationVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue && IsCyclicDependency(unitOfWork, unificRootId.Value, model.ParentId))
            {
                throw new OrganizationCyclicDependencyException();
            }
            if (!string.IsNullOrEmpty(model.OrganizationId) && organizationRep.All().Any(x => (x.UnificRootId != unificRootId) && (x.Oid == model.OrganizationId)))
            {
                throw new PtvArgumentException("", model.OrganizationId);
            }

            if (typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT1.ToString()) == model.OrganizationType ||
                typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT2.ToString()) == model.OrganizationType)
            {
                if (organizationRep.All().Single(x => x.Id == model.Id).TypeId != model.OrganizationType)
                {
                    throw new PtvServiceArgumentException("Organization type is not allowed!", new List<string> { typesCache.GetByValue<OrganizationType>(model.OrganizationType.Value) });
                }

            }
            var entity = TranslationManagerToEntity.Translate<VmOrganizationInput, OrganizationVersioned>(model, unitOfWork);
            unitOfWork.Save();


            return entity;
        }

        public VmEntityHeaderBase PublishOrganization(IVmPublishingModel model)
        {
            return model.Id.IsAssigned() ? contextManager.ExecuteWriter(unitOfWork => PublishOrganization(unitOfWork, model)) : null;
        }

        private VmEntityHeaderBase PublishOrganization(IUnitOfWorkWritable unitOfWork, IVmPublishingModel model)
        {
            Guid? organizationId = model.Id;
            //Validate mandatory values
            var validationMessages = ValidationManager.CheckEntity<OrganizationVersioned>(organizationId.Value, unitOfWork, model);
            if (validationMessages.Any())
            {
                throw new PtvValidationException(validationMessages, null);
            }

            //Publishing
            var affected = CommonService.PublishEntity<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork, model);

            return GetOrganizationHeader(affected.Id, unitOfWork);
        }

        private bool IsCyclicDependency(IUnitOfWork unitOfWork, Guid unificRootId, Guid? parentId)
        {
            if (parentId == null) return false;
            if (!unificRootId.IsAssigned() || !parentId.IsAssigned()) return false;
            if (unificRootId == parentId) return true;
            var filteredOutStatuses = new List<Guid>()
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString())
            };
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => i.UnificRootId == parentId.Value && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var allTree = higherOrgs.ToList();
            CyclicCheck(unitOfWork, higherOrgs, ref allTree, filteredOutStatuses);
            return allTree.Contains(unificRootId);
        }


        private void CyclicCheck(IUnitOfWork unitOfWork, List<Guid> orgs, ref List<Guid> allTree, List<Guid> filteredOutStatuses)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => orgs.Contains(i.UnificRootId) && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var toCheck = higherOrgs.Except(allTree).ToList();
            allTree.AddRange(toCheck);
            if (toCheck.Any())
            {
                CyclicCheck(unitOfWork, toCheck, ref allTree, filteredOutStatuses);
            }
        }

        public IVmEntityBase LockOrganization(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<OrganizationVersioned, Organization>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockOrganization(Guid id)
        {
            return Utilities.UnLockEntityVersioned<OrganizationVersioned, Organization>(id);
        }

        public VmOrganizationHeader WithdrawOrganization(Guid organizationId)
        {

            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (GetOrganizationConnectedEntities(unitOfWork, organizationId).AnyConnected)
                {
                    throw new WithdrawConnectedExistsException();
                }
            });
            var result = CommonService.WithdrawEntity<OrganizationVersioned, OrganizationLanguageAvailability>(organizationId);
            UnLockOrganization(result.Id.Value);
            return GetOrganizationHeader(result.Id);
        }

        private VmArchiveResult GetOrganizationConnectedEntities(IUnitOfWorkWritable unitOfWork, Guid? organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organization = organizationRep.All().SingleOrDefault(x => x.Id == organizationId);
            var organizations = organizationRep.All().Where(x => x.Id == organizationId);
            return new VmArchiveResult
            {
                Id = organization.Id,
                PublishingStatusId = organization.PublishingStatusId,
                ChannelsConnected = organizations.Any(i => i.UnificRoot.OrganizationServiceChannelsVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                SubOrganizationsConnected = organizations.Any(i => i.UnificRoot.Children.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                ServicesConnected = organizations.Any(i => i.UnificRoot.OrganizationServices.Any(j => j.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.ServiceProducerOrganizations.Any(j => j.ServiceProducer.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceProducer.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.OrganizationServicesVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished))

            };
        }

        public VmOrganizationHeader RestoreOrganization(Guid organizationId)
        {
            var result = CommonService.RestoreEntity<OrganizationVersioned, OrganizationLanguageAvailability>(organizationId, (unitOfWork, ov) =>
            {
                if (IsCyclicDependency(unitOfWork, ov.UnificRootId, ov.ParentId))
                {
                    throw new OrganizationCyclicDependencyException();
                }
                return true;
            });
            UnLockOrganization(result.Id.Value);
            return GetOrganizationHeader(result.Id);
        }

        public VmOrganizationHeader ArchiveLanguage(VmEntityBasic model)
        {
            var entity = CommonService.ArchiveLanguage<OrganizationVersioned, OrganizationLanguageAvailability>(model);
            UnLockOrganization(entity.Id);
            return GetOrganizationHeader(entity.Id);
        }

        public VmOrganizationHeader RestoreLanguage(VmEntityBasic model)
        {
            var entity = CommonService.RestoreLanguage<OrganizationVersioned, OrganizationLanguageAvailability>(model);
            UnLockOrganization(entity.Id);
            return GetOrganizationHeader(entity.Id);
        }

        public VmOrganizationHeader WithdrawLanguage(VmEntityBasic model)
        {
            var entity = CommonService.WithdrawLanguage<OrganizationVersioned, OrganizationLanguageAvailability>(model);
            UnLockOrganization(entity.Id);
            return GetOrganizationHeader(entity.Id);
        }

        public VmOrganizationHeader GetValidatedEntity(VmEntityBasic model)
        {
            return ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<OrganizationVersioned, Organization>(model.Id.Value, true),
                (unitOfWork) => GetOrganizationHeader(model.Id, unitOfWork)
            );
        }

        public VmOrganizationOutput SaveAndValidateOrganization(VmOrganizationInput model)
        {
            var result = ExecuteSaveAndValidate
            (
                model.Id,
                unitOfWork => SaveOrganization(unitOfWork, model),
                (unitOfWork, entity) => GetOrganization(new VmOrganizationBasic() { Id = entity.Id }, unitOfWork)
            );

            return result;
        }
    }
}
