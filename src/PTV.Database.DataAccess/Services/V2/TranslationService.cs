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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Domain.Model.Models.V2.TranslationOrder.Json;
using PTV.Domain.Model.Models.V2.TranslationOrder.Soap;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Framework.Exceptions.DataAccess;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(ITranslationService), RegisterType.Transient)]
    [Framework.RegisterService(typeof(ITranslationServiceInternal), RegisterType.Transient)]
    internal class TranslationService : ServiceBase, ITranslationService, ITranslationServiceInternal
    {
        private readonly IContextManager contextManager;
        private readonly ILogger logger;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private Guid nameTypeId;
        private readonly IValidationManager validationManager;
        private IEntityTreesCache entityTreesCache;
        private ITextManager textManager;

        public TranslationService(
            IContextManager contextManager,
            IPublishingStatusCache publishingStatusCache,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ICacheManager cacheManager,
            IVersioningManager versioningManager,
            IUserOrganizationChecker userOrganizationChecker,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            ITextManager textManager,
            ILogger<TranslationService> logger,
            IValidationManager validationManager) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.contextManager = contextManager;
            this.utilities = utilities;
            this.commonService = commonService;
            this.logger = logger;
            this.validationManager = validationManager;
            this.textManager = textManager;
            entityTreesCache = cacheManager.EntityTreesCache;
            nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
        }

        #region Service

        public void CheckServiceOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork)
        {
            var rootEntityId = VersioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, model.EntityId);
            if (rootEntityId.HasValue)
            {
                foreach (var targetLanguage in model.RequiredLanguages)
                {
                    var isAllowedState = IsServiceLanguageVersionInState(unitOfWork, rootEntityId.Value, targetLanguage, GetAfterTranslationStateList());
                    if (isAllowedState.TranslationOrderId.IsAssigned() && !isAllowedState.Result)  //Check update posibility
                    {
                        throw new PtvTranslationException();
                    }
                }
            }
        }

        public Guid SaveServiceTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model)
        {
            var lastOrderIdentifier = GetLastOrderIdentifier(unitOfWork);
            var previousTranslationLanguageIds = new List<Guid>();
            var stateTypeArrivedId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
            var serviceVersioned = GetEntity<ServiceVersioned>(model.EntityId, unitOfWork,
                q => q.Include(x => x.ServiceNames)
                    .Include(x => x.Organization)
                    .Include(x => x.LanguageAvailabilities)
            );

            if (serviceVersioned == null)
            {
                throw new PtvTranslationException($"ServiceVersioned cannot be null with id: {model.EntityId}");
            }

            foreach (var targetLangaugeId in model.RequiredLanguages)
            {
                SetServiceModelValues(unitOfWork, serviceVersioned, model, targetLangaugeId, ++lastOrderIdentifier);

                var languageVersionInArrivedOrCanceledState = IsServiceLanguageVersionInState(unitOfWork, model.EntityRootId, targetLangaugeId, GetAfterTranslationStateList());
                UpdateLastStateAndPreviousTranslationOrder(unitOfWork, model, targetLangaugeId, languageVersionInArrivedOrCanceledState, stateTypeArrivedId, previousTranslationLanguageIds);

                var data = TranslationManagerToEntity.Translate<VmTranslationOrderInput, ServiceTranslationOrder>(model, unitOfWork);

                var sourceJsonData = GenerateServiceTranslationJson(unitOfWork,
                    new VmJsonTranslationInput
                    {
                        Id = model.EntityId,
                        SourceLanguage = model.SourceLanguage
                    });

                if (!string.IsNullOrEmpty(sourceJsonData.Json))
                {
                    data.TranslationOrder.SourceLanguageCharAmount = sourceJsonData.JsonTextFieldsCharAmount;
                    data.TranslationOrder.SourceLanguageData = sourceJsonData.Json;
                    data.TranslationOrder.SourceLanguageDataHash = sourceJsonData.Json?.GetHashCode().ToString();
                }
            }

            return AddMissingLanguagesAvailabilityToEntity<ServiceVersioned, ServiceLanguageAvailability>(
                unitOfWork,
                serviceVersioned,
                model.RequiredLanguages,
                model.SourceLanguage,
                serviceVersioned.ServiceNames.FirstOrDefault(x => x.LocalizationId == model.SourceLanguage && x.TypeId == nameTypeId)?.Name,
                previousTranslationLanguageIds);
        }

        private void UpdateLastStateAndPreviousTranslationOrder(IUnitOfWorkWritable unitOfWork,
            VmTranslationOrderInput model,
            Guid targetLangaugeId,
            VmTranslationOrderDataResult languageVersionInArrivedOrCanceledState,
            Guid stateTypeArrivedId,
            ICollection<Guid> previousTranslationLanguageIds)
        {
            if (languageVersionInArrivedOrCanceledState != null && languageVersionInArrivedOrCanceledState.Result && languageVersionInArrivedOrCanceledState.TranslationOrderId.HasValue)
            {
                UpdatePreviousLastTranslationOrderState(unitOfWork, languageVersionInArrivedOrCanceledState.TranslationOrderId.Value); //Update Last state to false

                if (languageVersionInArrivedOrCanceledState.OrderStateTypeId == stateTypeArrivedId) //if Arrived set previous translation order(Reorder)
                {
                    model.PreviousTranslationOrderId = languageVersionInArrivedOrCanceledState.TranslationOrderId;
                    previousTranslationLanguageIds.Add(targetLangaugeId);
                }
            }
        }

        private void SetServiceModelValues(IUnitOfWorkWritable unitOfWork, ServiceVersioned serviceVersioned, VmTranslationOrderInput model, Guid targetLangaugeId, long lastOrderIdentifier)
        {
            model.OrderIdentifier = lastOrderIdentifier;
            model.TargetLanguage = targetLangaugeId;
            model.SourceEntityName = serviceVersioned?.ServiceNames.FirstOrDefault(x => x.LocalizationId == model.SourceLanguage && x.TypeId == nameTypeId)?.Name;

            if (serviceVersioned != null)
            {
                model.EntityRootId = serviceVersioned.UnificRootId;
                model.EntityId = serviceVersioned.Id;
                SetOrganizationModelValues(unitOfWork, model, serviceVersioned.OrganizationId);
            }
        }

        private void SetOrganizationModelValues(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model, Guid organizationId)
        {
            var organizationVersionInfo =
                VersioningManager.GetLastPublishedModifiedDraftVersion<OrganizationVersioned>(unitOfWork, organizationId);

            var organizationVersioned = GetEntity<OrganizationVersioned>(organizationVersionInfo?.EntityId, unitOfWork,
                q => q.Include(x => x.OrganizationNames)
                    .Include(x => x.Business));

            model.OrganizationIdentifier = organizationVersioned?.UnificRootId;
            model.OrganizationName = organizationVersioned?.OrganizationNames
                .Where(x => x.LocalizationId == model.SourceLanguage).Select(x => x.Name).FirstOrDefault();
            model.OrganizationBusinessCode = organizationVersioned?.Business?.Code;
        }

        public VmTranslationOrderStateOutputs GetServiceTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var unificRootId = VersioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, entityId);

            var entityOrderStates = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder).ThenInclude(i => i.ServiceTranslationOrders).ThenInclude(i => i.Service)
                .Include(i => i.TranslationOrder).ThenInclude(i => i.TranslationCompany)
                .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y => y.ServiceId == unificRootId))
                .OrderByDescending(x => x.SendAt)
                .ToList();

            return GetVmTranslationOrderStateOutputs(unitOfWork, entityId, languageId, entityOrderStates);
        }

        private VmJsonTranslationData GenerateServiceTranslationJson(IUnitOfWork unitOfWork, VmJsonTranslationInput model)
        {
            var service = GetEntity<ServiceVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.ServiceNames)
                    .Include(x => x.ServiceDescriptions)
                    .Include(x => x.ServiceRequirements));

            TranslationManagerToVm.SetLanguage(model.SourceLanguage);
            var jsonModel = TranslationManagerToVm.Translate<ServiceVersioned, VmJsonServiceTranslation>(service);

            return new VmJsonTranslationData
            {
                Json = JsonConvert.SerializeObject(jsonModel, Formatting.Indented),
                JsonTextFieldsCharAmount = GetCharacterCountOfVmJsonTranslationTexts(jsonModel)
            };
        }

        public VmTranslationOrderDataResult IsServiceLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep
                .All()
                .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y => y.ServiceId == rootId) &&
                                     x.TranslationOrder.TargetLanguageId == languageId &&
                                     x.Last)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        private VmTranslationOrderDataResult IsServiceLanguageVersionInStateByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates, bool confirm = false)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep
                .All()
                .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y => y.ServiceId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Checked == confirm &&
                            x.Last)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        public VmTranslationOrderDataResult IsServiceLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsServiceLanguageVersionInState(unitOfWork, rootId, languageId, new List<TranslationStateTypeEnum> {TranslationStateTypeEnum.Arrived});
        }

        private VmTranslationOrderDataResult IsServiceLanguageVersionDeliveredByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, bool confirm)
        {
            return IsServiceLanguageVersionInStateByUserConfirm(unitOfWork, rootId, languageId,
                new List<TranslationStateTypeEnum> { TranslationStateTypeEnum.Arrived}, confirm: confirm);
        }

        public bool IsServiceLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsServiceLanguageVersionInState(unitOfWork, rootId, languageId, GetInTranslationStateList()).Result;
        }

        public void ConfirmServiceDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId, bool allowRemoveTrackingOrders = false)
        {
            var unificRootId = VersioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, versionId);
            if (unificRootId != null)
            {
                if (allowRemoveTrackingOrders)
                {
                    var deliveredData = IsServiceLanguageVersionDelivered(unitOfWork, unificRootId.Value, languageId);
                    RemoveTrackingTranslationOrders(unitOfWork, deliveredData);
                }

                var isDeliveredDataWithoutUserConfirm = IsServiceLanguageVersionDeliveredByUserConfirm(unitOfWork, unificRootId.Value, languageId, false);
                if (isDeliveredDataWithoutUserConfirm.Result && isDeliveredDataWithoutUserConfirm.OrderStatusId.HasValue)
                {
                    UpdateTranslationOrderState(unitOfWork, isDeliveredDataWithoutUserConfirm.OrderStatusId.Value, true);
                }
            }
        }

        #endregion Service

        #region General description

        public void CheckGeneralDescriptionOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork)
        {
            var rootEntityId = VersioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, model.EntityId);
            if (rootEntityId.HasValue)
            {
                foreach (var targetLanguage in model.RequiredLanguages)
                {
                    var isAllowedState = IsGeneralDescriptionLanguageVersionInState(unitOfWork, rootEntityId.Value, targetLanguage, GetAfterTranslationStateList());
                    if (isAllowedState.TranslationOrderId.IsAssigned() && !isAllowedState.Result)  //Check update posibility
                    {
                        throw new PtvTranslationException();
                    }
                }
            }
        }

        public Guid SaveGeneralDescriptionTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model)
        {
            var lastOrderIdentifier = GetLastOrderIdentifier(unitOfWork);
            var previousTranslationLanguageIds = new List<Guid>();
            var stateTypeArrivedId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
            var generalDescriptionVersioned = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(model.EntityId, unitOfWork,
                q => q.Include(x => x.Names)
                    .Include(x => x.LanguageAvailabilities)
            );

            if (generalDescriptionVersioned == null)
            {
                throw new PtvTranslationException($"General description cannot be null with id: {model.EntityId}");
            }

            foreach (var targetLangaugeId in model.RequiredLanguages)
            {
                SetGeneralDescriptionModelValues(generalDescriptionVersioned, model, targetLangaugeId, ++lastOrderIdentifier);

                var languageVersionInArrivedOrCanceledState = IsGeneralDescriptionLanguageVersionInState(unitOfWork, model.EntityRootId, targetLangaugeId, GetAfterTranslationStateList());
                UpdateLastStateAndPreviousTranslationOrder(unitOfWork, model, targetLangaugeId, languageVersionInArrivedOrCanceledState, stateTypeArrivedId, previousTranslationLanguageIds);

                var data = TranslationManagerToEntity.Translate<VmTranslationOrderInput, GeneralDescriptionTranslationOrder>(model, unitOfWork);

                var sourceJsonData = GenerateGeneralDescriptionTranslationJson(unitOfWork,
                    new VmJsonTranslationInput
                    {
                        Id = model.EntityId,
                        SourceLanguage = model.SourceLanguage
                    });

                if (!string.IsNullOrEmpty(sourceJsonData.Json))
                {
                    data.TranslationOrder.SourceLanguageCharAmount = sourceJsonData.JsonTextFieldsCharAmount;
                    data.TranslationOrder.SourceLanguageData = sourceJsonData.Json;
                    data.TranslationOrder.SourceLanguageDataHash = sourceJsonData.Json?.GetHashCode().ToString();
                }
            }

            return AddMissingLanguagesAvailabilityToEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                unitOfWork,
                generalDescriptionVersioned,
                model.RequiredLanguages,
                model.SourceLanguage,
                generalDescriptionVersioned.Names.FirstOrDefault(x => x.LocalizationId == model.SourceLanguage && x.TypeId == nameTypeId)?.Name,
                previousTranslationLanguageIds);
        }

        private void SetGeneralDescriptionModelValues(StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned, VmTranslationOrderInput model, Guid targetLangaugeId, long lastOrderIdentifier)
        {
            model.OrderIdentifier = lastOrderIdentifier;
            model.TargetLanguage = targetLangaugeId;
            model.SourceEntityName = generalDescriptionVersioned?.Names.FirstOrDefault(x => x.LocalizationId == model.SourceLanguage && x.TypeId == nameTypeId)?.Name;

            if (generalDescriptionVersioned != null)
            {
                model.EntityRootId = generalDescriptionVersioned.UnificRootId;
                model.EntityId = generalDescriptionVersioned.Id;
            }
        }

        public VmTranslationOrderStateOutputs GetGeneralDescriptionTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            var unificRootId = VersioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, entityId);
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();

            var entityOrderStates = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder).ThenInclude(i => i.GeneralDescriptionTranslationOrders).ThenInclude(i => i.StatutoryServiceGeneralDescription)
                .Include(i => i.TranslationOrder).ThenInclude(i => i.TranslationCompany)
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any(y => y.StatutoryServiceGeneralDescriptionId == unificRootId))
                .OrderByDescending(x => x.SendAt)
                .ToList();

            return GetVmTranslationOrderStateOutputs(unitOfWork, entityId, languageId, entityOrderStates);
        }

        private VmJsonTranslationData GenerateGeneralDescriptionTranslationJson(IUnitOfWork unitOfWork, VmJsonTranslationInput model)
        {
            var generalDescriptionVersioned = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.Names)
                    .Include(x => x.Descriptions)
                    .Include(x => x.StatutoryServiceRequirements));

            TranslationManagerToVm.SetLanguage(model.SourceLanguage);
            var jsonModel = TranslationManagerToVm.Translate<StatutoryServiceGeneralDescriptionVersioned, VmJsonGeneralDescriptionTranslation>(generalDescriptionVersioned);

            return new VmJsonTranslationData
            {
                Json = JsonConvert.SerializeObject(jsonModel, Formatting.Indented),
                JsonTextFieldsCharAmount = GetCharacterCountOfVmJsonTranslationTexts(jsonModel)
            };
        }

        public VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep
                .All()
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any(y => y.StatutoryServiceGeneralDescriptionId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Last)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        private VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionInStateByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates, bool confirm = false)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep
                .All()
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any(y => y.StatutoryServiceGeneralDescriptionId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Checked == confirm &&
                            x.Last)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        public VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsGeneralDescriptionLanguageVersionInState(unitOfWork, rootId, languageId, new List<TranslationStateTypeEnum> { TranslationStateTypeEnum.Arrived });
        }

        private VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionDeliveredByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, bool confirm)
        {
            return IsGeneralDescriptionLanguageVersionInStateByUserConfirm(unitOfWork, rootId, languageId,
                new List<TranslationStateTypeEnum> { TranslationStateTypeEnum.Arrived }, confirm: confirm);
        }

        public bool IsGeneralDescriptionLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsGeneralDescriptionLanguageVersionInState(unitOfWork, rootId, languageId, GetInTranslationStateList()).Result;
        }

        public void ConfirmGeneralDescriptionDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId, bool allowRemoveTrackingOrders = false)
        {
            var unificRootId = VersioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, versionId);
            if (unificRootId != null)
            {
                if (allowRemoveTrackingOrders)
                {
                    var deliveredData = IsGeneralDescriptionLanguageVersionDelivered(unitOfWork, unificRootId.Value, languageId);
                    RemoveTrackingTranslationOrders(unitOfWork, deliveredData);
                }

                var isDeliveredDataWithoutUserConfirm = IsGeneralDescriptionLanguageVersionDeliveredByUserConfirm(unitOfWork, unificRootId.Value, languageId, false);
                if (isDeliveredDataWithoutUserConfirm.Result && isDeliveredDataWithoutUserConfirm.OrderStatusId.HasValue)
                {
                    UpdateTranslationOrderState(unitOfWork, isDeliveredDataWithoutUserConfirm.OrderStatusId.Value, true);
                }
            }
        }

        #endregion  General description

        #region Channels

        public void CheckChannelOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork)
        {
            var rootEntityId = VersioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, model.EntityId);
            if (rootEntityId.HasValue)
            {
                foreach (var targetLanguage in model.RequiredLanguages)
                {
                    var isAllowedState = IsChannelLanguageVersionInState(unitOfWork, rootEntityId.Value, targetLanguage, GetAfterTranslationStateList());
                    if (isAllowedState.TranslationOrderId.IsAssigned() && !isAllowedState.Result)  //Check update posibility
                    {
                        throw new PtvTranslationException();
                    }
                }
            }
        }

        public VmTranslationOrderStateOutputs GetChannelTranslationData(VmTranslationDataInput model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetChannelTranslationOrderStates(unitOfWork, model.EntityId, model.SourceLanguage);
            });
        }

        public Guid SaveChannelTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model)
        {
            var lastOrderIdentifier = GetLastOrderIdentifier(unitOfWork);
            var previousTranslationLanguageIds = new List<Guid>();
            var stateTypeArrivedId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());

            var channelVersioned = GetEntity<ServiceChannelVersioned>(model.EntityId, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames)
                    .Include(x => x.Organization)
                    .Include(x => x.LanguageAvailabilities)
                );
            if (channelVersioned == null)
            {
                throw new PtvTranslationException($"ServiceVersioned cannot be null with id: {model.EntityId}");
            }

            foreach (var targetLangaugeId in model.RequiredLanguages)
            {
                SetChannelModelValues(unitOfWork, channelVersioned, model, targetLangaugeId, ++lastOrderIdentifier);

                var languageVersionInArrivedOrCanceledState = IsChannelLanguageVersionInState(unitOfWork, model.EntityRootId, targetLangaugeId, GetAfterTranslationStateList());
                UpdateLastStateAndPreviousTranslationOrder(unitOfWork, model, targetLangaugeId, languageVersionInArrivedOrCanceledState, stateTypeArrivedId, previousTranslationLanguageIds);

                var data = TranslationManagerToEntity.Translate<VmTranslationOrderInput, ServiceChannelTranslationOrder>(model, unitOfWork);

                var sourceJsonData = GenerateChannelTranslationJson(unitOfWork,
                    new VmJsonTranslationInput
                    {
                        Id = model.EntityId,
                        SourceLanguage = model.SourceLanguage
                    });

                if (!string.IsNullOrEmpty(sourceJsonData.Json))
                {
                    data.TranslationOrder.SourceLanguageCharAmount = sourceJsonData.JsonTextFieldsCharAmount;
                    data.TranslationOrder.SourceLanguageData = sourceJsonData.Json;
                    data.TranslationOrder.SourceLanguageDataHash = sourceJsonData.Json?.GetHashCode().ToString();
                }
            }

            return AddMissingLanguagesAvailabilityToEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                unitOfWork,
                channelVersioned,
                model.RequiredLanguages,
                model.SourceLanguage,
                channelVersioned.ServiceChannelNames.FirstOrDefault(x => x.LocalizationId == model.SourceLanguage && x.TypeId == nameTypeId)?.Name,
                previousTranslationLanguageIds);
        }

        private void SetChannelModelValues(IUnitOfWorkWritable unitOfWork, ServiceChannelVersioned channelVersioned, VmTranslationOrderInput model, Guid targetLangaugeId, long lastOrderIdentifier)
        {
            model.OrderIdentifier = lastOrderIdentifier;
            model.TargetLanguage = targetLangaugeId;
            model.SourceEntityName = channelVersioned?.ServiceChannelNames.FirstOrDefault(x => x.LocalizationId == model.SourceLanguage && x.TypeId == nameTypeId)?.Name;

            if (channelVersioned != null)
            {
                model.EntityRootId = channelVersioned.UnificRootId;
                model.EntityId = channelVersioned.Id;
                SetOrganizationModelValues(unitOfWork, model, channelVersioned.OrganizationId);
            }
        }

        public VmTranslationOrderStateOutputs GetChannelTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            var unificRootId = VersioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, entityId);
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();

            var entityOrderStates = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder).ThenInclude(i => i.ServiceChannelTranslationOrders).ThenInclude(i => i.ServiceChannel)
                .Include(i => i.TranslationOrder).ThenInclude(i => i.TranslationCompany)
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any(y => y.ServiceChannelId == unificRootId))
                .OrderByDescending(x => x.SendAt)
                .ToList();

            return GetVmTranslationOrderStateOutputs(unitOfWork, entityId, languageId, entityOrderStates);
        }

        private VmJsonTranslationData GenerateChannelTranslationJson(IUnitOfWork unitOfWork, VmJsonTranslationInput model)
        {
            var entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames)
                    .Include(x => x.ServiceChannelDescriptions));

            TranslationManagerToVm.SetLanguage(model.SourceLanguage);
            var jsonModel = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmJsonChannelTranslation>(entity);

            return new VmJsonTranslationData
            {
                Json = JsonConvert.SerializeObject(jsonModel, Formatting.Indented),
                JsonTextFieldsCharAmount = GetCharacterCountOfVmJsonTranslationTexts(jsonModel)
            };
        }

        public VmTranslationOrderDataResult IsChannelLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep
                .All()
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any(y => y.ServiceChannelId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Last)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        private VmTranslationOrderDataResult IsChannelLanguageLanguageVersionInStateByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates, bool confirm = false)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep
                .All()
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any(y => y.ServiceChannelId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Checked == confirm &&
                            x.Last)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        public VmTranslationOrderDataResult IsChannelLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsChannelLanguageVersionInState(unitOfWork, rootId, languageId, new List<TranslationStateTypeEnum> { TranslationStateTypeEnum.Arrived });
        }

        private VmTranslationOrderDataResult IsChannelLanguageVersionDeliveredByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, bool confirm)
        {
            return IsChannelLanguageLanguageVersionInStateByUserConfirm(unitOfWork, rootId, languageId,
                new List<TranslationStateTypeEnum> { TranslationStateTypeEnum.Arrived }, confirm: confirm);
        }

        public bool IsChannelLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsChannelLanguageVersionInState(unitOfWork, rootId, languageId, GetInTranslationStateList()).Result;
        }

        public void ConfirmChannelDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId, bool allowRemoveTrackingOrders = false)
        {
            var unificRootId = VersioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, versionId);

            if (unificRootId != null)
            {
                if (allowRemoveTrackingOrders)
                {
                    var deliveredData = IsChannelLanguageVersionDelivered(unitOfWork, unificRootId.Value, languageId);
                    RemoveTrackingTranslationOrders(unitOfWork, deliveredData);
                }

                var isDeliveredDataWithoutUserConfirm = IsChannelLanguageVersionDeliveredByUserConfirm(unitOfWork, unificRootId.Value, languageId, false);
                if (isDeliveredDataWithoutUserConfirm.Result && isDeliveredDataWithoutUserConfirm.OrderStatusId.HasValue)
                {
                    UpdateTranslationOrderState(unitOfWork, isDeliveredDataWithoutUserConfirm.OrderStatusId.Value, true);
                }
            }
        }

        #endregion Channels

        public long GetLastOrderIdentifier(IUnitOfWorkWritable unitOfWork)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();

            return translationOrderRep.All()
                .OrderByDescending(x => x.Created)
                .ThenByDescending(x => x.OrderIdentifier)
                .Select(x => x.OrderIdentifier)
                .FirstOrDefault();
        }

        public string GetTranslationDataJson(Guid translationOrderId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var translationOrder = GetEntity<TranslationOrder>(translationOrderId, unitOfWork, i => i);

                return translationOrder?.SourceLanguageData ?? string.Empty;
            });
        }

        public Guid AddMissingLanguagesAvailabilityToEntity<T, TLanguageAvail>(
            IUnitOfWorkWritable unitOfWork,
            T entity,
            List<Guid> targetLanguageIds,
            Guid sourceLanguageId,
            string sourceName,
            List<Guid> previousTranslationLanguageIds)
            where T : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var entityLanguagesList = entity.LanguageAvailabilitiesReference.Select(x => x.LanguageId).ToList();
            var targetNonExistingLanguages = targetLanguageIds.Except(entityLanguagesList).ToList();
            var languageAvailabilities = new List<VmLanguageAvailabilityInfo>();
            var keepInPreviousState = false;
            var historyAction = previousTranslationLanguageIds.Any(previousId => targetLanguageIds.Contains(previousId))
                ? HistoryAction.TranslationReordered
                : HistoryAction.TranslationOrdered;

            languageAvailabilities.AddRange(entity.LanguageAvailabilities
                .Select(language => new VmLanguageAvailabilityInfo
                {
                    LanguageId = language.LanguageId,
                    StatusId = language.StatusId
                }).ToList());

            if (targetNonExistingLanguages.Any()) //language version doesnt exist yet
            {
                logger.LogInformation($"Translation - for entity with id:{entity.Id} will added this non-exist languages:{string.Join(",", targetNonExistingLanguages)}");
                languageAvailabilities.AddRange(targetNonExistingLanguages.Select(targetLanguage => new VmLanguageAvailabilityInfo { LanguageId = targetLanguage, IsNew = true}).ToList());
            }
            else
            {
                //change version, previous state will same
                keepInPreviousState = true;
            }

            //Add new version with missing language versions
            var result = TranslationManagerToEntity.Translate<VmTranslationOrderEntityBase, T>(
                new VmTranslationOrderEntityBase
                {
                    Id = entity.Id,
                    LanguagesAvailabilities = languageAvailabilities,
                    SourceName = sourceName,
                    KeepInPreviousState = keepInPreviousState
                }, unitOfWork);

            var translationOrderDetails = new VmTranslationOrderEntityTargetLanguages
            {
                SourceLanguage = sourceLanguageId,
                TargetLanguages = targetLanguageIds,
                EntityRootId = entity.UnificRootId
            };

            commonService.CreateHistoryMetaData<T, TLanguageAvail>(result, translationOrderDetails: translationOrderDetails, action: historyAction);
            return result?.Id ?? entity.Id;
        }

        private List<TranslationStateTypeEnum> GetAfterTranslationStateList()
        {
            var translationList = new List<TranslationStateTypeEnum>
            {
                TranslationStateTypeEnum.Arrived,
                TranslationStateTypeEnum.Canceled
            };
            return translationList;
        }

        private List<TranslationStateTypeEnum> GetInTranslationStateList()
        {
            var translationList = new List<TranslationStateTypeEnum>
            {
                TranslationStateTypeEnum.ReadyToSend,
                TranslationStateTypeEnum.Sent,
                TranslationStateTypeEnum.InProgress,
                TranslationStateTypeEnum.FileError,
                TranslationStateTypeEnum.SendError,
                TranslationStateTypeEnum.DeliveredFileError,
                TranslationStateTypeEnum.FailForInvestigation,
                TranslationStateTypeEnum.RequestForRepetition,
                TranslationStateTypeEnum.RequestForCancel
            };
            return translationList;
        }

        private Guid GetTranslationOrderParentBaseId(IUnitOfWork unitOfWork, Guid translationOrderId)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var translationOrders = translationOrderRep.All();
            var data = translationOrders.Single(x => x.Id == translationOrderId);

            if (data.PreviousTranslationOrderId.HasValue)
            {
                return GetTranslationOrderParentBaseId(unitOfWork, data.PreviousTranslationOrderId.Value);
            }

            return data.Id;
        }

        private int GetNumberOfTranslationOrderByState(IUnitOfWork unitOfWork, Guid translationOrderId, Guid stateId)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            return translationOrderStateRep
                .All()
                .Count(x => x.TranslationStateId == stateId && x.TranslationOrderId == translationOrderId);
        }

        private List<TranslationOrder> GetTranslationOrdersByStateList(IUnitOfWork unitOfWork, List<Guid> states)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var results = new List<TranslationOrder>();
            foreach (var state in states)
            {
                results.AddRange(translationOrderRep.All()
                    .Include(i => i.TranslationOrderStates)
                    .Where(x => state == x.TranslationOrderStates.Where(y => y.Last)
                                    .OrderByDescending(z => z.SendAt)
                                    .FirstOrDefault().TranslationStateId)
                    .ToList());
            }

            return results;
        }

        private VmTranslationOrderStateOutputs GetVmTranslationOrderStateOutputs(IUnitOfWork unitOfWork, Guid entityId, Guid languageId, ICollection<TranslationOrderState> entityOrderStates)
        {
            var stateTypeArrivedId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
            var stateTypeCanceledId = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Canceled.ToString());

            //Select all languages throught all entity content of entity(source and target languages) without arrived and canceled
            var entityLastOrderStates = entityOrderStates
                .Where(x => x.Last &&
                            x.TranslationStateId != stateTypeArrivedId &&
                            x.TranslationStateId != stateTypeCanceledId)
                .ToList();

            var entitySourceLanguagesInUse = entityLastOrderStates
                .Select(x => x.TranslationOrder.SourceLanguageId)
                .Distinct().ToList();

            var entityTargetLanguagesInUse = entityLastOrderStates
                .Select(x => x.TranslationOrder.TargetLanguageId)
                .Distinct().ToList();

            var entityAllLanguagesInUse = entityTargetLanguagesInUse.Contains(languageId)
                ? languageCache.AllowedLanguageIds //If language use as target, cannot do other translation for other languages
                : entitySourceLanguagesInUse.Union(entityTargetLanguagesInUse).Distinct().ToList();

            //Select all target languages in use by source language without arrived and canceled
            var sourceLanguageOrderStates = entityOrderStates
                .Where(x => x.TranslationOrder.SourceLanguageId == languageId)
                .ToList();

            var sourceTargetLanguagesLastAlreadyDelivered = sourceLanguageOrderStates
                .Where(x => x.Last && x.TranslationStateId == stateTypeArrivedId)
                .Select(x => x.TranslationOrder.TargetLanguageId)
                .Distinct().ToList();

            var sourceTargetLanguagesInUse = sourceLanguageOrderStates
                .Where(x => x.Last &&
                            x.TranslationStateId != stateTypeArrivedId &&
                            x.TranslationStateId != stateTypeCanceledId)
                .Select(x => x.TranslationOrder.TargetLanguageId)
                .Distinct().ToList();

            var vmEntityOrderStates = TranslationManagerToVm.TranslateAll<TranslationOrderState, VmTranslationOrderStateOutput>(entityOrderStates);
            foreach (var orderState in vmEntityOrderStates) //Content Id
            {
                orderState.TranslationOrder.ContentId = GetTranslationOrderParentBaseId(unitOfWork, orderState.TranslationOrder.Id);
            }

            return new VmTranslationOrderStateOutputs
            {
                Id = entityId,
                TranslationOrderStates = vmEntityOrderStates,
                TargetLanguagesAllInUse = entityAllLanguagesInUse, //Disabled items
                TargetLanguagesSelectedInUse = sourceTargetLanguagesInUse, //Selected items
                TargetLanguagesLastAlreadyDelivered = sourceTargetLanguagesLastAlreadyDelivered
            };
        }

        //SOAP VM
        public void TranslationOrderSentSuccess(Guid translationOrderId, string companyOrderIdentifier)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
                var translationOrder = translationOrderRep.All().FirstOrDefault(x => x.Id == translationOrderId);

                if (translationOrder == null)
                {
                    throw new PtvTranslationException($"Cannot find order with Id:{translationOrderId}");
                }

                translationOrder.TranslationCompanyOrderIdentifier = companyOrderIdentifier;

                try
                {
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
                catch (Exception e)
                {
                    throw new PtvTranslationException(e.Message, e);
                }
            });
        }

        public void TranslationOrderSentFailure(Guid translationOrderId)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                AddTranslationOrderState(unitOfWork, translationOrderId, typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.SendError.ToString()));
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        private void UpdateTranslationOrderState(IUnitOfWorkWritable unitOfWork, Guid orderStateId, bool confirm)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep.All().FirstOrDefault(x => x.Id == orderStateId);

            if (translationOrderState == null)
            {
                throw new PtvTranslationException($"Cannot find order state with Id:{orderStateId}");
            }

            translationOrderState.Checked = confirm;

            try
            {
                unitOfWork.Save(SaveMode.AllowAnonymous);
            }
            catch (Exception e)
            {
                throw new PtvTranslationException(e.Message, e);
            }
        }

        public void AddTranslationErrorStateWithRepetitionCheck(Guid translationOrderId, int maxRepetitionNumber, TranslationStateTypeEnum? translationStateType, string infoDetails = null)
        {
            if (!translationStateType.HasValue) return;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var stateTypeId = typesCache.Get<TranslationStateType>(translationStateType.ToString());
                var translationOrderInStateNumber = GetNumberOfTranslationOrderByState(unitOfWork, translationOrderId, stateTypeId);

                AddTranslationOrderState(unitOfWork, translationOrderId, stateTypeId, infoDetails);

                if (translationOrderInStateNumber > maxRepetitionNumber)
                {
                    AddTranslationOrderState(unitOfWork, translationOrderId, typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.FailForInvestigation.ToString()), infoDetails);
                }

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        public void AddTranslationOrderState(Guid translationOrderId, TranslationStateTypeEnum? translationStateType, string infoDetails = null, DateTime? deliverAt = null)
        {
            if (!translationStateType.HasValue) return;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var stateTypeId = typesCache.Get<TranslationStateType>(translationStateType.ToString());
                AddTranslationOrderState(unitOfWork, translationOrderId, stateTypeId, infoDetails, deliverAt);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        public void AddTranslationOrderState(IUnitOfWorkWritable unitOfWork, Guid translationOrderId, Guid stateId, string infoDetails = null, DateTime? deliverAt = null)
        {
            UpdatePreviousLastTranslationOrderState(unitOfWork, translationOrderId);
            var vm = new VmTranslationOrderStateInput { TranslationOrderId = translationOrderId, TranslationStateId = stateId, InfoDetails = infoDetails, Last = true };
            TranslationManagerToEntity.Translate<VmTranslationOrderStateInput, TranslationOrderState>(vm, unitOfWork);
            UpdateTranslationOrderDeliverAt(unitOfWork, translationOrderId, deliverAt);
        }

        private void UpdatePreviousLastTranslationOrderState(IUnitOfWork unitOfWork, Guid translationOrderId)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            translationOrderStateRep.All().Where(x => x.TranslationOrderId == translationOrderId && x.Last).ToList().ForEach(x => x.Last = false);
            translationOrderStateRep.Known(EntityState.Added).Where(x => x.TranslationOrderId == translationOrderId && x.Last).ForEach(i => i.Last = false);
        }

        private void UpdateTranslationOrderDeliverAt(IUnitOfWorkWritable unitOfWork, Guid translationOrderId, DateTime? deliverAt)
        {
            if (deliverAt.HasValue)
            {
                var translationOrdeRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
                var translationOrder = translationOrdeRep.All().FirstOrDefault(x => x.Id == translationOrderId);

                if (translationOrder == null)
                {
                    throw new PtvTranslationException($"Cannot find order with Id:{translationOrderId}");
                }

                translationOrder.DeliverAt = deliverAt;
            }
        }

        private int GetCharacterCountOfVmJsonTranslationTexts<T>(T model)
        {
            var result = 0;
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                if (p.PropertyType != typeof(VmJsonTranslationText)) { continue; }

                var textValue = ((VmJsonTranslationText)p.GetValue(model, null))?.Text;
                if (!string.IsNullOrEmpty(textValue))
                {
                    result += textValue.Count(char.IsLetter);
                }
            }
            return result;
        }

        private VmTranslationOrderDataResult GetTranslationOrderIsInStateResult(TranslationOrderState translationOrderState, List<TranslationStateTypeEnum> translationStateTypeEnums)
        {
            var translationStates = translationStateTypeEnums.Select(translationState => typesCache.Get<TranslationStateType>(translationState.ToString())).ToList();
            var result = translationStates.Contains(translationOrderState?.TranslationStateId ?? Guid.Empty);
            return new VmTranslationOrderDataResult
            {
                Result = result,
                TranslationOrderId = translationOrderState?.TranslationOrderId,
                OrderStateTypeId = translationOrderState?.TranslationStateId,
                OrderStatusId = result ? translationOrderState?.Id : (Guid?)null,
            };
        }

        private void RemoveTrackingTranslationOrders(IUnitOfWorkWritable unitOfWork, VmTranslationOrderDataResult deliveredData)
        {
            var trackingTransaltionOrderRep = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            if (deliveredData != null && deliveredData.Result &&
                trackingTransaltionOrderRep.All().Any(x => x.TranslationOrderId == deliveredData.TranslationOrderId))
            {
                trackingTransaltionOrderRep.BatchDelete(x => x.TranslationOrderId, deliveredData.TranslationOrderId);
                try
                {
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
                catch (Exception e)
                {
                    throw new PtvTranslationException(e.Message, e);
                }
            }
        }

        public IReadOnlyList<VmSoapTranslationOrder> GetTranslationOrdersByStateTypes(IReadOnlyCollection<TranslationStateTypeEnum> stateTypes)
        {
            var stateList = stateTypes.Select(stateEnum => typesCache.Get<TranslationStateType>(stateEnum.ToString())).ToList();

            return contextManager.ExecuteReader(unitOfWork =>
            {
                var translationOrders = GetTranslationOrdersByStateList(unitOfWork, stateList);
                var vmSoapTranslationsOrders = TranslationManagerToVm.TranslateAll<TranslationOrder, VmSoapTranslationOrder>(translationOrders);

                foreach (var order in vmSoapTranslationsOrders)
                {
                    order.ContentId = GetTranslationOrderParentBaseId(unitOfWork, order.Id);
                }

                return vmSoapTranslationsOrders;
            });
        }

        public void CheckAndSetLastErrorState(IReadOnlyList<VmSoapTranslationOrder> translationOrders)
        {
            var errorStateIds = new List<Guid>
            {
                typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.SendError.ToString()),
                typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.FileError.ToString()),
                typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.DeliveredFileError.ToString())
            };

            contextManager.ExecuteReader(unitOfWork =>
            {
                var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();

                foreach (var translationOrder in translationOrders)
                {
                    var lastErrorOrderStates = translationOrderStateRep.All()
                        .Where(x => x.TranslationOrderId == translationOrder.Id)
                        .OrderByDescending(y => y.SendAt)
                        .Take(3)
                        .ToList();

                    var errorState = lastErrorOrderStates.LastOrDefault();
                    var errorStateTypeId = errorState?.TranslationStateId;

                    translationOrder.OrderStateType = errorStateTypeId.HasValue && errorStateIds.Contains(errorStateTypeId.Value)
                            ? (TranslationStateTypeEnum?) ParseEnum<TranslationStateTypeEnum>(typesCache.GetByValue<TranslationStateType>(errorStateTypeId.Value))
                            : null;
                }
            });
        }

        private T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        #region Processing download file

        public void ProcessTranslatedFile(Guid translationOrderId, string targetFileJson)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (!IsValidJson(targetFileJson.Trim()))
                {
                    throw new PtvTranslationException("Json validation error of translated file.");
                }

                var updateFile = UpdateTargetLanguageFileInTranslationOrder(unitOfWork, translationOrderId, targetFileJson);
                if (!updateFile)
                {
                    throw new PtvTranslationException("Error of update target language data by translated file.");
                }

                UpdateEntityByNewTranslation(unitOfWork, translationOrderId, targetFileJson);

                try
                {
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
                catch (Exception e)
                {
                    throw new PtvTranslationException($"Error of saving translated file to translation order({translationOrderId}). {e.FlattenWithInnerExceptions()}");
                }
            });
        }

        private bool UpdateTargetLanguageFileInTranslationOrder(IUnitOfWorkWritable unitOfWork, Guid translationOrderId, string targetFileJson)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var translationOrder = translationOrderRep.All().FirstOrDefault(x => x.Id == translationOrderId);
            if (translationOrder != null)
            {
                translationOrder.TargetLanguageData = targetFileJson;
                return true;
            }
            return false;
        }

        private void UpdateEntityByNewTranslation(IUnitOfWorkWritable unitOfWork, Guid translationOrderId, string targetFileJson)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var translationOrder = translationOrderRep.All()
                .Include(i => i.ServiceTranslationOrders)
                .Include(i => i.GeneralDescriptionTranslationOrders)
                .Include(i => i.ServiceChannelTranslationOrders)
                .FirstOrDefault(x => x.Id == translationOrderId);

            if (translationOrder != null)
            {
                TranslationManagerToEntity.SetLanguage(translationOrder.TargetLanguageId); //Set target langauge

                var translationOrderDetails = new VmTranslationOrderEntityTargetLanguages
                {
                    SourceLanguage = translationOrder.SourceLanguageId,
                    TargetLanguages = new List<Guid> {translationOrder.TargetLanguageId}
                };

                try
                {
                    if (translationOrder.ServiceTranslationOrders.Any())
                    {
                        var modelData = JsonConvert.DeserializeObject<VmJsonServiceTranslation>(targetFileJson);
                        modelData.Id = GetLastVersionOfEntityId<ServiceVersioned>(unitOfWork, translationOrder.ServiceTranslationOrders.Select(x => x.ServiceId).FirstOrDefault());
                        var service = TranslationManagerToEntity.Translate<VmJsonServiceTranslation, ServiceVersioned>(modelData, unitOfWork);
                        AdditionalTranslationOfEntity(unitOfWork, modelData, service, translationOrder.TargetLanguageId); 
                        commonService.CreateHistoryMetaData<ServiceVersioned, ServiceLanguageAvailability>(service, translationOrderDetails: translationOrderDetails, action: HistoryAction.TranslationReceived);
                        TrackTranslationOrder(unitOfWork, translationOrderId, service.OrganizationId);
                    }
                    else if (translationOrder.ServiceChannelTranslationOrders.Any())
                    {
                        var modelData = JsonConvert.DeserializeObject<VmJsonChannelTranslation>(targetFileJson);
                        modelData.Id = GetLastVersionOfEntityId<ServiceChannelVersioned>(unitOfWork, translationOrder.ServiceChannelTranslationOrders.Select(x => x.ServiceChannelId).FirstOrDefault());
                        var serviceChannel = TranslationManagerToEntity.Translate<VmJsonChannelTranslation, ServiceChannelVersioned>(modelData, unitOfWork);
                        commonService.CreateHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel, translationOrderDetails: translationOrderDetails, action: HistoryAction.TranslationReceived);
                        TrackTranslationOrder(unitOfWork, translationOrderId, serviceChannel.OrganizationId);
                    }
                    else if (translationOrder.GeneralDescriptionTranslationOrders.Any())
                    {
                        var modelData = JsonConvert.DeserializeObject<VmJsonGeneralDescriptionTranslation>(targetFileJson);
                        modelData.Id = GetLastVersionOfEntityId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, translationOrder.GeneralDescriptionTranslationOrders.Select(x => x.StatutoryServiceGeneralDescriptionId).FirstOrDefault());
                        var generalDescription = TranslationManagerToEntity.Translate<VmJsonGeneralDescriptionTranslation, StatutoryServiceGeneralDescriptionVersioned>(modelData, unitOfWork);
                        AdditionalTranslationOfEntity(unitOfWork, modelData, generalDescription, translationOrder.TargetLanguageId); 
                        commonService.CreateHistoryMetaData<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription, translationOrderDetails: translationOrderDetails, action: HistoryAction.TranslationReceived);
                    }
                }
                catch (JsonSerializationException ex)
                {
                    throw new PtvTranslationException($"Deserialize of json translated data failed. json:{targetFileJson}, exception:{ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new PtvTranslationException($"Update entity by new translation data failed. exception: {ex.Message}", ex);
                }
            }
        }

        private void AdditionalTranslationOfEntity(IUnitOfWorkWritable unitOfWork, VmJsonServiceTranslation modelData, ServiceVersioned entity, Guid targetLanguageId)
        {
            if (!string.IsNullOrEmpty(modelData.ConditionsAndCriteria?.Text))
            {
                if (entity.ServiceRequirements.Any(x => x.LocalizationId == targetLanguageId))
                {
                    var serviceRequirementUpdated = entity.ServiceRequirements.FirstOrDefault(x => x.LocalizationId == targetLanguageId);
                    if (serviceRequirementUpdated != null)
                    {
                        serviceRequirementUpdated.Requirement = textManager.ConvertMarkdownToJson(modelData.ConditionsAndCriteria?.Text);
                    }
                }
                else
                {
                    var serviceRequirementVm = new VmServiceRequirement
                    {
                        Id = entity.Id,
                        Requirement = textManager.ConvertMarkdownToJson(modelData.ConditionsAndCriteria?.Text),
                        LocalizationId = targetLanguageId
                    };

                    var newServiceRequirement = TranslationManagerToEntity.Translate<VmServiceRequirement, ServiceRequirement>(serviceRequirementVm, unitOfWork);
                    newServiceRequirement.ServiceVersioned = entity;
                    entity.ServiceRequirements.Add(newServiceRequirement);
                }
            }
        }
        
        private void AdditionalTranslationOfEntity(IUnitOfWorkWritable unitOfWork, VmJsonGeneralDescriptionTranslation modelData, StatutoryServiceGeneralDescriptionVersioned entity, Guid targetLanguageId)
        {
            if (!string.IsNullOrEmpty(modelData.ConditionsAndCriteria?.Text))
            {
                if (entity.StatutoryServiceRequirements.Any(x => x.LocalizationId == targetLanguageId))
                {
                    var serviceRequirementUpdated = entity.StatutoryServiceRequirements.FirstOrDefault(x => x.LocalizationId == targetLanguageId);
                    if (serviceRequirementUpdated != null)
                    {
                        serviceRequirementUpdated.Requirement = textManager.ConvertMarkdownToJson(modelData.ConditionsAndCriteria?.Text);
                    }
                }
                else
                {
                    var serviceRequirementVm = new VmServiceRequirement
                    {
                        Id = entity.Id,
                        Requirement = textManager.ConvertMarkdownToJson(modelData.ConditionsAndCriteria?.Text),
                        LocalizationId = targetLanguageId
                    };

                    var newServiceRequirement = TranslationManagerToEntity.Translate<VmServiceRequirement, StatutoryServiceRequirement>(serviceRequirementVm, unitOfWork);
                    newServiceRequirement.StatutoryServiceGeneralDescriptionVersioned = entity;
                    entity.StatutoryServiceRequirements.Add(newServiceRequirement);
                }
            }
        }

        private void TrackTranslationOrder(IUnitOfWorkWritable unitOfWork, Guid translationOrder, Guid organizationId)
        {
            var trackRepository = unitOfWork.CreateRepository<ITrackingTranslationOrderRepository>();
            trackRepository.Add(new TrackingTranslationOrder
            {
                OperationType = TranslationStateTypeEnum.Arrived.ToString(),
                OrganizationId = organizationId,
                TranslationOrderId = translationOrder
            });
        }

        private bool IsValidJson(string json)
        {
            return (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{") && json.EndsWith("}")) || (!string.IsNullOrWhiteSpace(json) && json.StartsWith("[") && json.EndsWith("]"));
        }

        private Guid GetLastVersionOfEntityId<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid? unificRootId) where TEntityType : class, IVersionedVolume
        {
            if (unificRootId == null)
            {
                throw new PtvTranslationException($"Unific root id of entity {typeof(TEntityType)} cannot be null.");
            }

            var lastPublishedModifiedDraftVersion = VersioningManager.GetLastPublishedModifiedDraftVersion<TEntityType>(unitOfWork, unificRootId.Value);
            if (lastPublishedModifiedDraftVersion?.EntityId == null)
            {
                throw new Exception($"Cannot find last published modified or draft version for unificRootId: {unificRootId.Value}");
            }

            return lastPublishedModifiedDraftVersion.EntityId;
        }

        #endregion

        #region Translation availability
        public Dictionary<string, VmTranslationOrderAvailability> GetServiceTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId)
        {
            var canBeTranslated = CanBeTranslated<ServiceVersioned>(new List<Guid> { entityVersionedId }, unitOfWork, entityVersionedId);

            return commonService.GetTranslationLanguageList()
                .ToDictionary(x => x, y => new VmTranslationOrderAvailability
                {
                    IsInTranslation = IsServiceLanguageVersionInTranslation(unitOfWork, entityRootId, languageCache.Get(y)),
                    IsTranslationDelivered = IsServiceLanguageVersionDeliveredByUserConfirm(unitOfWork, entityRootId, languageCache.Get(y), false).Result,
                    CanBeTranslated = canBeTranslated.ContainsKey(y) && canBeTranslated[y]
                });
        }

        public Dictionary<string, VmTranslationOrderAvailability> GetChannelTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId, List<VmChannelConnectionOutput> connections)
        {
            var connectionsIds = connections == null ? commonService.GetServiceChannelRelationIds(unitOfWork, entityRootId) : connections.Select(x => x.Id).ToList();
            var canBeTranslated = CanBeTranslated<ServiceChannelVersioned>(connectionsIds, unitOfWork, entityVersionedId);
            return commonService.GetTranslationLanguageList()
                .ToDictionary(x => x, y => new VmTranslationOrderAvailability
                {
                    IsInTranslation = IsChannelLanguageVersionInTranslation(unitOfWork, entityRootId, languageCache.Get(y)),
                    IsTranslationDelivered = IsChannelLanguageVersionDeliveredByUserConfirm(unitOfWork, entityRootId, languageCache.Get(y), false).Result,
                    CanBeTranslated = canBeTranslated.ContainsKey(y) && canBeTranslated[y]
                });
        }

        public Dictionary<string, VmTranslationOrderAvailability> GetGeneralDescriptionTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId)
        {
            var canBeTranslated = CanBeGeneralDescriptionTranslated(unitOfWork, entityVersionedId); ;

            return commonService.GetTranslationLanguageList()
                .ToDictionary(x => x, y => new VmTranslationOrderAvailability
                {
                    IsInTranslation = IsGeneralDescriptionLanguageVersionInTranslation(unitOfWork, entityRootId, languageCache.Get(y)),
                    IsTranslationDelivered = IsGeneralDescriptionLanguageVersionDeliveredByUserConfirm(unitOfWork, entityRootId, languageCache.Get(y), false).Result,
                    CanBeTranslated = canBeTranslated.ContainsKey(y) && canBeTranslated[y]
                });
        }
        #endregion

        #region OpenApi

        public IVmOpenApiModelWithPagingBase<VmOpenApiTranslationItem> GetTranslationItems(DateTime? date, DateTime? dateBefore, int pageNumber, int pageSize)
        {
            var handler = new TranslationsPagingHandler(languageCache, typesCache, date, dateBefore, pageNumber, pageSize);
            contextManager.ExecuteReader(unitOfWork =>
            {
                var totalCount = handler.Search(unitOfWork);
            });

            return handler.GetModel();
        }

        #endregion

        #region DataImportConsole

        public void UpdateAllMissingLanguageVersionInTranslation(IUnitOfWorkWritable unitOfWork)
        {
            var inTranslationStatesList = GetInTranslationStateList().Select(stateEnum => typesCache.Get<TranslationStateType>(stateEnum.ToString())).ToList();
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var translationOrdersInTranslationList = new List<TranslationOrder>();
            foreach (var state in inTranslationStatesList)
            {
                translationOrdersInTranslationList.AddRange(translationOrderRep.All()
                    .Include(i => i.TranslationOrderStates)
                    .Include(i => i.ServiceTranslationOrders)
                    .Include(i => i.ServiceChannelTranslationOrders)
                    .Include(i => i.GeneralDescriptionTranslationOrders)
                    .Where(x => state == x.TranslationOrderStates.FirstOrDefault(y => y.Last).TranslationStateId)
                    .ToList());
            }

            //Services
            var serviceTranslationOrders = translationOrdersInTranslationList.SelectMany(x => x.ServiceTranslationOrders).ToList();
            var translateServicesWithRequiredLanguages = serviceTranslationOrders.GroupBy(x => new { x.ServiceId, x.TranslationOrder.SourceLanguageId } )
                .Select(x => new VmTranslationOrderEntityTargetLanguages
                {
                    EntityRootId = x.Key.ServiceId,
                    TargetLanguages = x.Select(y => y.TranslationOrder.TargetLanguageId).ToList(),
                    SourceLanguage = x.Key.SourceLanguageId
                })
                .ToList();
            UpdateEntityWithMissingLanguageAvailabilities<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,translateServicesWithRequiredLanguages);

            //General description
            var generalDescriptionTranslationOrders = translationOrdersInTranslationList.SelectMany(x => x.GeneralDescriptionTranslationOrders).ToList();
            var translateGeneralDescriptionWithRequiredLanguages = generalDescriptionTranslationOrders.GroupBy(x => new { x.StatutoryServiceGeneralDescriptionId, x.TranslationOrder.SourceLanguageId } )
                .Select(x => new VmTranslationOrderEntityTargetLanguages
                {
                    EntityRootId = x.Key.StatutoryServiceGeneralDescriptionId,
                    TargetLanguages = x.Select(y => y.TranslationOrder.TargetLanguageId).ToList(),
                    SourceLanguage = x.Key.SourceLanguageId
                })
                .ToList();
            UpdateEntityWithMissingLanguageAvailabilities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, translateGeneralDescriptionWithRequiredLanguages);

            //Channel
            var channelTranslationOrders = translationOrdersInTranslationList.SelectMany(x => x.ServiceChannelTranslationOrders).ToList();
            var translateChannelWithRequiredLanguages = channelTranslationOrders.GroupBy(x => new { x.ServiceChannelId, x.TranslationOrder.SourceLanguageId } )
                .Select(x => new VmTranslationOrderEntityTargetLanguages
                {
                    EntityRootId = x.Key.ServiceChannelId,
                    TargetLanguages = x.Select(y => y.TranslationOrder.TargetLanguageId).ToList(),
                    SourceLanguage = x.Key.SourceLanguageId
                })
                .ToList();
            UpdateEntityWithMissingLanguageAvailabilities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, translateChannelWithRequiredLanguages);

        }

        private void UpdateEntityWithMissingLanguageAvailabilities<T, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, List<VmTranslationOrderEntityTargetLanguages> translateServicesWithRequiredLanguages)
            where T : class, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            foreach (var translatedEntity in translateServicesWithRequiredLanguages)
            {
                var entityVersionedId = VersioningManager
                    .GetLastPublishedModifiedDraftVersion<T>(unitOfWork, translatedEntity.EntityRootId)
                    .EntityId;

                var entityVersioned = GetEntity<T>(entityVersionedId, unitOfWork,
                    q => q.Include(x => x.LanguageAvailabilities));

                AddMissingLanguagesAvailabilityToEntity<T, TLanguageAvail>(
                    unitOfWork,
                    entityVersioned,
                    translatedEntity.TargetLanguages,
                    translatedEntity.SourceLanguage,
                    null,
                    new List<Guid>());
            }
        }

        public void AddAllMissingEntityNamesAfterTranslationOrder(IUnitOfWorkWritable unitOfWork)
        {
            int counter = 0;
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var serviceNameRep = unitOfWork.CreateRepository<IServiceNameRepository>();
            var serviceChannelNameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
            var generalDescriptionNameRep = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();
            var translationOrders = translationOrderRep.All()
                .Include(i => i.ServiceTranslationOrders)
                .Include(i => i.ServiceChannelTranslationOrders)
                .Include(i => i.GeneralDescriptionTranslationOrders)
                .ToList();

            //Services
            var serviceTranslationOrders = translationOrders.SelectMany(x => x.ServiceTranslationOrders).ToList();
            var translationServiceRootIds = serviceTranslationOrders.Select(x => x.ServiceId).Distinct().ToList();

            foreach (var serviceRootId in translationServiceRootIds)
            {
                var entityVersionedId = VersioningManager.GetLastPublishedModifiedDraftVersion<ServiceVersioned>(unitOfWork, serviceRootId)?.EntityId;
                var entityVersioned = GetEntity<ServiceVersioned>(entityVersionedId, unitOfWork, q => q.Include(x => x.LanguageAvailabilities).Include(x => x.ServiceNames));

                if (entityVersioned == null) continue;

                foreach (var languageAvalability in entityVersioned.LanguageAvailabilities)
                {
                    var entityLanguageId = languageAvalability.LanguageId;

                    if (entityVersioned.ServiceNames.Where(x => x.TypeId == nameTypeId).All(x => x.LocalizationId != entityLanguageId)) //unexist name for language avilability
                    {
                        counter++;
                        string previusName = null;
                        var sourceLanguageData = serviceTranslationOrders
                            .FirstOrDefault(x => x.ServiceId == serviceRootId &&
                                                 x.TranslationOrder.SourceLanguageId == entityLanguageId)
                                   ?.TranslationOrder?.SourceLanguageData;

                        if (!string.IsNullOrEmpty(sourceLanguageData))
                        {
                            var modelData = JsonConvert.DeserializeObject<VmJsonServiceTranslation>(sourceLanguageData);
                            previusName = modelData.Name?.Text;
                        }

                        if (!string.IsNullOrEmpty(previusName))
                        {
                            serviceNameRep.Add(new ServiceName
                            {
                                ServiceVersionedId = entityVersioned.Id,
                                TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                                Name = previusName,
                                LocalizationId = entityLanguageId
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"Added {counter} service names.");

            //Channels
            counter = 0;
            var channelTranslationOrders = translationOrders.SelectMany(x => x.ServiceChannelTranslationOrders).ToList();
            var translationChannelRootIds = channelTranslationOrders.Select(x => x.ServiceChannelId).Distinct().ToList();

            foreach (var channelRootId in translationChannelRootIds)
            {
                var entityVersionedId = VersioningManager.GetLastPublishedModifiedDraftVersion<ServiceChannelVersioned>(unitOfWork, channelRootId)?.EntityId;
                var entityVersioned = GetEntity<ServiceChannelVersioned>(entityVersionedId, unitOfWork, q => q.Include(x => x.LanguageAvailabilities).Include(x => x.ServiceChannelNames));

                if (entityVersioned == null) continue;

                foreach (var languageAvalability in entityVersioned.LanguageAvailabilities)
                {
                    var entityLanguageId = languageAvalability.LanguageId;

                    if (entityVersioned.ServiceChannelNames.Where(x => x.TypeId == nameTypeId).All(x => x.LocalizationId != entityLanguageId)) //unexist name for language avilability
                    {
                        counter++;
                        string previusName = null;
                        var sourceLanguageData = channelTranslationOrders
                            .FirstOrDefault(x => x.ServiceChannelId == channelRootId &&
                                                 x.TranslationOrder.SourceLanguageId == entityLanguageId)
                            ?.TranslationOrder?.SourceLanguageData;

                        if (!string.IsNullOrEmpty(sourceLanguageData))
                        {
                            var modelData = JsonConvert.DeserializeObject<VmJsonChannelTranslation>(sourceLanguageData);
                            previusName = modelData.Name?.Text;
                        }

                        if (!string.IsNullOrEmpty(previusName))
                        {
                            serviceChannelNameRep.Add(new ServiceChannelName
                            {
                                ServiceChannelVersionedId = entityVersioned.Id,
                                TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                                Name = previusName,
                                LocalizationId = entityLanguageId
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"Added {counter} channel names.");

            //General descriptions
            counter = 0;
            var generalDescriptionTranslationOrders = translationOrders.SelectMany(x => x.GeneralDescriptionTranslationOrders).ToList();
            var translationGeneralDescriptionRootIds = generalDescriptionTranslationOrders.Select(x => x.StatutoryServiceGeneralDescriptionId).Distinct().ToList();

            foreach (var generalDescriptionRootId in translationGeneralDescriptionRootIds)
            {
                var entityVersionedId = VersioningManager.GetLastPublishedModifiedDraftVersion<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, generalDescriptionRootId)?.EntityId;
                var entityVersioned = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(entityVersionedId, unitOfWork, q => q.Include(x => x.LanguageAvailabilities).Include(x => x.Names));

                if (entityVersioned == null) continue;

                foreach (var languageAvalability in entityVersioned.LanguageAvailabilities)
                {
                    var entityLanguageId = languageAvalability.LanguageId;

                    if (entityVersioned.Names.Where(x => x.TypeId == nameTypeId).All(x => x.LocalizationId != entityLanguageId)) //unexist name for language availability
                    {
                        counter++;
                        string previusName = null;

                        var sourceLanguageData = generalDescriptionTranslationOrders
                            .FirstOrDefault(x => x.StatutoryServiceGeneralDescriptionId == generalDescriptionRootId &&
                                                 x.TranslationOrder.SourceLanguageId == entityLanguageId)
                                   ?.TranslationOrder?.SourceLanguageData;

                        if (!string.IsNullOrEmpty(sourceLanguageData))
                        {
                            var modelData = JsonConvert.DeserializeObject<VmJsonGeneralDescriptionTranslation>(sourceLanguageData);
                            previusName = modelData.Name?.Text;
                        }

                        //special case
                        if (string.IsNullOrEmpty(previusName))
                        {
                            var targetLanguageData = generalDescriptionTranslationOrders
                                .FirstOrDefault(
                                    x => x.StatutoryServiceGeneralDescriptionId == generalDescriptionRootId &&
                                         x.TranslationOrder.TargetLanguageData != null &&
                                         x.TranslationOrder.TargetLanguageId == entityLanguageId)
                                ?.TranslationOrder?.TargetLanguageData;

                            if (!string.IsNullOrEmpty(targetLanguageData))
                            {
                                var modelData = JsonConvert.DeserializeObject<VmJsonGeneralDescriptionTranslation>(targetLanguageData);
                                previusName = modelData.Name?.Text;
                            }
                        }

                        if (!string.IsNullOrEmpty(previusName))
                        {
                            generalDescriptionNameRep.Add(new StatutoryServiceName
                            {
                                StatutoryServiceGeneralDescriptionVersionedId = entityVersioned.Id,
                                TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                                Name = previusName,
                                LocalizationId = entityLanguageId
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"Added {counter} general description names.");
        }

        public void UpdateAllWrongServiceProcessingInfo()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var startMessage = "STARTING Update all wrong service processing info ......";
                Console.WriteLine(startMessage);
                logger.LogInformation(startMessage);

                var arrivedTranslationStateTypeId =
                    typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
                var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();

                var translationOrders = translationOrderRep.All()
                    .Include(i => i.TranslationOrderStates)
                    .Include(i => i.ServiceTranslationOrders)
                    .Where(x => (arrivedTranslationStateTypeId ==
                                 x.TranslationOrderStates.FirstOrDefault(y => y.Last).TranslationStateId)
                                && x.TargetLanguageData != null)
                    .ToList();

                var serviceTranslationOrders = translationOrders.Where(x => x.ServiceTranslationOrders.Any()).ToList();

                foreach (var translationOrder in serviceTranslationOrders)
                {
                    var unificRootId = translationOrder.ServiceTranslationOrders.Select(x => x.ServiceId)
                        .FirstOrDefault();

                    try
                    {
                        UpdateAllWrongServiceProcessingInfo(unitOfWork, unificRootId,
                            translationOrder.TargetLanguageData, translationOrder.TargetLanguageId);
                    }
                    catch (Exception e)
                    {
                        var errorMessage =
                            $"Cannot update service name of service unificRootId: {unificRootId} and exception: {e.Message} with trace: {e.StackTrace}";
                        logger.LogInformation(errorMessage);
                    }
                }

                unitOfWork.Save(SaveMode.NonTrackedDataMigration); //Test
                var endMessage = "FINISHED Update all wrong service processing info.";
                Console.WriteLine(endMessage);
                logger.LogInformation(endMessage);
            });
        }

        private void UpdateAllWrongServiceProcessingInfo(IUnitOfWorkWritable unitOfWork, Guid unificRootId, string targetLanguageData, Guid targetLanguageId)
        {
            var modelData = JsonConvert.DeserializeObject<VmJsonServiceTranslation>(targetLanguageData);
            if (modelData != null)
            {
                modelData.Id = GetLastVersionOfEntityId<ServiceVersioned>(unitOfWork, unificRootId); //Last version of data
                var arrivedChargeAdditionalInformationDescription = modelData?.ChargeAdditionalInformation?.Text;

                if (!string.IsNullOrEmpty(arrivedChargeAdditionalInformationDescription))
                {
                    var serviceVersioned = GetEntity<ServiceVersioned>(modelData.Id, unitOfWork,
                        q => q.Include(x => x.ServiceDescriptions));

                    var processingTimeAdditionalInfoId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString());

                    foreach (var serviceProcessingTimeDescription in serviceVersioned.ServiceDescriptions.Where(x => x.LocalizationId == targetLanguageId && x.TypeId == processingTimeAdditionalInfoId))
                    {
                        var arrivedProcessingTimeAdditionalDescription = modelData?.ProcessingTime?.Text;

                        var serviceProcessTimeDescriptionCompareText = Regex.Replace(serviceProcessingTimeDescription?.Description, @"\s+", " ");
                        var arrivedChargeAdditionalInformationCompareText = Regex.Replace(arrivedChargeAdditionalInformationDescription, @"\s+", " ");

                        if (serviceProcessTimeDescriptionCompareText.Equals(arrivedChargeAdditionalInformationCompareText)) //compare exisiting proccesTime with wrong arrived charge additional information
                        {
                            //Console.WriteLine($"ARRIVED chargeAdditionalInformationDescription: {arrivedChargeAdditionalInformationDescription}");
                            //Console.WriteLine($"PREVIOUS serviceProcessingTimeDescription: {serviceProcessingTimeDescription.Description}");
                            logger.LogInformation($"PREVIOUS serviceProcessingTimeDescription: {serviceProcessingTimeDescription.Description}");

                            if (!string.IsNullOrEmpty(arrivedProcessingTimeAdditionalDescription))
                            {
                                serviceProcessingTimeDescription.Description = arrivedProcessingTimeAdditionalDescription;

                                var updatedMessage = $"UPDATED processingTimeDescription: {serviceProcessingTimeDescription.Description} " +
                                                     $"for languageId: {targetLanguageId} of service unificRootId: {unificRootId} vID:{serviceVersioned.Id}";
                                //Console.WriteLine(updatedMessage);
                                logger.LogInformation(updatedMessage);
                            }
                            else
                            {
                                var removeMessage = $"REMOVED processingTimeDescription: {serviceProcessingTimeDescription.Description} " +
                                                    $"for languageId: {targetLanguageId} of service unificRootId: {unificRootId} vID:{serviceVersioned.Id}";
                                //Console.WriteLine(removeMessage);
                                logger.LogInformation(removeMessage);

                                serviceProcessingTimeDescription.Description = string.Empty;
                            }
                        }
                        else
                        {
                            var message = $"WIHTOUT changes(changed by USER) processingTimeDescription: {serviceProcessingTimeDescription.Description} " +
                                          $"for languageId: {targetLanguageId} of service unificRootId: {unificRootId} vID:{serviceVersioned.Id}";

                            //Console.WriteLine(message);
                            logger.LogInformation(message);
                        }
                    }
                }
            }
        }

        public void UpdateAllWrongGeneralDescriptionProcessingInfo()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var startMessage = $"STARTING Update all wrong general description processing info ......";
                //Console.WriteLine(startMessage);
                logger.LogInformation(startMessage);

                var arrivedTranslationStateTypeId =
                    typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString());
                var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();

                var translationOrders = translationOrderRep.All()
                    .Include(i => i.TranslationOrderStates)
                    .Include(i => i.GeneralDescriptionTranslationOrders)
                    .Where(x => (arrivedTranslationStateTypeId ==
                                 x.TranslationOrderStates.FirstOrDefault(y => y.Last).TranslationStateId)
                                && x.TargetLanguageData != null)
                    .ToList();

                var gdTranslationOrders = translationOrders.Where(x => x.GeneralDescriptionTranslationOrders.Any()).ToList();

                foreach (var translationOrder in gdTranslationOrders)
                {
                    var unificRootId = translationOrder.GeneralDescriptionTranslationOrders.Select(x => x.StatutoryServiceGeneralDescriptionId)
                        .FirstOrDefault();

                    try
                    {
                        UpdateAllWrongGeneralDescriptionProcessingInfo(unitOfWork, unificRootId,
                            translationOrder.TargetLanguageData, translationOrder.TargetLanguageId);

                    }
                    catch (Exception e)
                    {
                        var errorMessage = $"Cannot update processing info of general description unificRootId: {unificRootId} and exception: {e.Message} with trace: {e.StackTrace}";
                        logger.LogInformation(errorMessage);
                    }
                }

                unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                var endMessage = $"FINISHED Update all wrong general description processing info.";
                //Console.WriteLine(endMessage);
                logger.LogInformation(endMessage);
            });
        }

        private void UpdateAllWrongGeneralDescriptionProcessingInfo(IUnitOfWorkWritable unitOfWork, Guid unificRootId, string targetLanguageData, Guid targetLanguageId)
        {
            var modelData = JsonConvert.DeserializeObject<VmJsonGeneralDescriptionTranslation>(targetLanguageData);
            if (modelData != null)
            {
                modelData.Id = GetLastVersionOfEntityId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, unificRootId); //Last version of data
                var arrivedChargeAdditionalInformationDescription = modelData?.ChargeAdditionalInformation?.Text;
                var arrivedProcessingTimeAdditionalDescription = modelData?.ProcessingTime?.Text;
                var generalDescriptionVersioned = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(modelData.Id, unitOfWork,
                    q => q.Include(x => x.Descriptions));

                var processingTimeAdditionalInfoId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString());
                var gdProcessingTimeDescription = generalDescriptionVersioned.Descriptions.FirstOrDefault(x => x.LocalizationId == targetLanguageId && x.TypeId == processingTimeAdditionalInfoId);

                var gdProcessingTimeDescriptionIsNull = gdProcessingTimeDescription?.Description == null;
                var gdProcessingTimeDescriptionIsEmpty = gdProcessingTimeDescription?.Description == string.Empty;

                //ProcessingTime arrived, but is null for current language
                if (gdProcessingTimeDescriptionIsNull && !string.IsNullOrEmpty(arrivedProcessingTimeAdditionalDescription))
                {
                    generalDescriptionVersioned.Descriptions.Add(new StatutoryServiceDescription()
                    {
                        Description = arrivedProcessingTimeAdditionalDescription,
                        TypeId = processingTimeAdditionalInfoId,
                        LocalizationId = targetLanguageId
                    });
                    var updatedMessage =
                        $"ADD NEW processingTimeDescription: {arrivedProcessingTimeAdditionalDescription} " +
                        $"for languageId: {targetLanguageId} of general description unificRootId: {unificRootId} vID:{generalDescriptionVersioned.Id}";
                    //Console.WriteLine(updatedMessage);
                    logger.LogInformation(updatedMessage);
                    return;
                }

                //If processingtime is empty, despite exist arrived processing Time
                if (gdProcessingTimeDescriptionIsEmpty && !string.IsNullOrEmpty(arrivedProcessingTimeAdditionalDescription))
                {
                    gdProcessingTimeDescription.Description = arrivedProcessingTimeAdditionalDescription;
                    var updatedMessage =
                        $"UPDATED EMPTY processingTimeDescription: {gdProcessingTimeDescription.Description} " +
                        $"for languageId: {targetLanguageId} of general description unificRootId: {unificRootId} vID:{generalDescriptionVersioned.Id}";
                    //Console.WriteLine(updatedMessage);
                    logger.LogInformation(updatedMessage);
                    return;
                }

                //If arrived charge additional information ins't null and arrived processing time isn't null, compare and then updating or removing
                if (!string.IsNullOrEmpty(arrivedChargeAdditionalInformationDescription) && !gdProcessingTimeDescriptionIsEmpty && !gdProcessingTimeDescriptionIsNull)
                {
                    var gdProcessTimeDescriptionCompareText = Regex.Replace(gdProcessingTimeDescription?.Description, @"\s+", " ");
                    var arrivedChargeAdditionalInformationCompareText = Regex.Replace(arrivedChargeAdditionalInformationDescription, @"\s+", " ");

                    if (gdProcessTimeDescriptionCompareText.Equals(arrivedChargeAdditionalInformationCompareText)) //compare exisiting proccesTime with wrong arrived charge additional information
                    {
                        logger.LogInformation($"PREVIOUS processingTimeDescription: {gdProcessingTimeDescription.Description}");
                        logger.LogInformation($"ARRIVED processingTimeDescription: {arrivedProcessingTimeAdditionalDescription}");

                        if (!string.IsNullOrEmpty(arrivedProcessingTimeAdditionalDescription))
                        {
                            gdProcessingTimeDescription.Description = arrivedProcessingTimeAdditionalDescription;

                            var updatedMessage =
                                $"UPDATED EQUALS processingTimeDescription: {gdProcessingTimeDescription.Description} " +
                                $"for languageId: {targetLanguageId} of general description unificRootId: {unificRootId} vID:{generalDescriptionVersioned.Id}";
                            //Console.WriteLine(updatedMessage);
                            logger.LogInformation(updatedMessage);
                        }
                        else
                        {
                            var removeMessage =
                                $"REMOVED processingTimeDescription: {gdProcessingTimeDescription.Description} " +
                                $"for languageId: {targetLanguageId} of general description unificRootId: {unificRootId} vID:{generalDescriptionVersioned.Id}";
                            //Console.WriteLine(removeMessage);
                            logger.LogInformation(removeMessage);

                            gdProcessingTimeDescription.Description = string.Empty;
                        }
                    }
                }
            }
        }


        #endregion DataImportConsole

        #region can be translated

        private Dictionary<string, bool> CanBeTranslated<T>(List<Guid> serviceIds, IUnitOfWork unitOfWork, Guid checkId)
        {
            var areaWholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var areaWholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
            var fundingTypePublicId = typesCache.Get<ServiceFundingType>(ServiceFundingTypeEnum.PubliclyFunded.ToString());
            var areaTypeBussinedId = typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString());
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var result = serviceRep.All()
               .Where(x => serviceIds.Contains(x.Id))
               .Where(x => x.AreaInformationTypeId == areaWholeCountryId || x.AreaInformationTypeId == areaWholeCountryExceptAlandId)
               .Where(x => x.FundingTypeId == fundingTypePublicId).Any();

            result = result || ServiceWithGeneralDescriptionAndTargetGroupCheck(serviceIds, serviceRep, areaTypeBussinedId, fundingTypePublicId, unitOfWork);

            if (result)
            {
                var entityCheck = validationManager.CheckEntity<T>(checkId, unitOfWork);
                return languageCache.TranslationOrderLanguageCodes
                    .ToDictionary(x => x, y => !entityCheck.Keys.Any(z=>z == languageCache.Get(y)));
            }

            return languageCache.TranslationOrderLanguageCodes.ToDictionary(x => x, y => false);
        }

        private bool ServiceWithGeneralDescriptionAndTargetGroupCheck(List<Guid> serviceIds, IServiceVersionedRepository serviceRep, Guid areaTypeBussinedId, Guid fundingTypePublicId, IUnitOfWork unitOfWork)
        {
            // todo change to cache
            Guid enterpriceAndOrganizationId = entityTreesCache.Get<TargetGroup>("KR2");

            var services = serviceRep.All()
                .Where(x => serviceIds.Contains(x.Id))
                .Where(x => x.AreaMunicipalities.Any() || x.Areas.Any(y => y.Area.AreaTypeId == areaTypeBussinedId))
                .Where(x => !x.ServiceTargetGroups.Any(y => y.Override && y.TargetGroupId == enterpriceAndOrganizationId))
                .Where(x => x.StatutoryServiceGeneralDescriptionId.HasValue)
                .Where(x => x.FundingTypeId == fundingTypePublicId)
                .ToList();

            foreach (var service in services)
            {
                var stgRep = unitOfWork.CreateRepository<IServiceTargetGroupRepository>();
                var stg = stgRep.All()
                    .Where(x => x.ServiceVersionedId == service.Id)
                    .Any(y => y.TargetGroupId == enterpriceAndOrganizationId);
                var gdtg = false;
                var gdbs = false;

                if (service.StatutoryServiceGeneralDescriptionId.HasValue)
                {
                    var generalDescriptionTypeBusinessSubregionId = typesCache.Get<GeneralDescriptionType>(GeneralDescriptionTypeEnum.BusinessSubregion.ToString());
                    var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    var gdLastPublishedId = VersioningManager.GetLastPublishedVersion<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, service.StatutoryServiceGeneralDescriptionId.Value)?.EntityId;
                    if (gdLastPublishedId.HasValue)
                    {
                        gdtg = gdRep.All()
                            .Where(x => x.Id == gdLastPublishedId.Value)
                            .Any(x => x.TargetGroups.Any(y => y.TargetGroupId == enterpriceAndOrganizationId));

                        gdbs = gdRep.All()
                            .Where(x => x.Id == gdLastPublishedId.Value)
                            .Any(x => x.GeneralDescriptionTypeId == generalDescriptionTypeBusinessSubregionId);
                    }
                }

                if ((stg || gdtg) && gdbs)
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<string, bool> CanBeGeneralDescriptionTranslated(IUnitOfWork unitOfWork, Guid checkId)
        {
            var result = utilities.UserHighestRole() == UserRoleEnum.Eeva;
            if (result)
            {
                var entityCheck = validationManager.CheckEntity<StatutoryServiceGeneralDescriptionVersioned>(checkId, unitOfWork);
                return languageCache.TranslationOrderLanguageCodes
                    .ToDictionary(x => x, y => entityCheck.Keys.All(z => z != languageCache.Get(y)));
            }
            return languageCache.TranslationOrderLanguageCodes.ToDictionary(x => x, y => false);
        }

        #endregion
    }
}
