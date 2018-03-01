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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
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
using PTV.Framework.ServiceManager;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(ITranslationService), RegisterType.Transient)]
    internal class TranslationService : ServiceBase, ITranslationService
    {
        private readonly IContextManager contextManager;
        private IVersioningManager versioningManager;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private ServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private IConnectionsServiceInternal connectionsService;

        public TranslationService(
            IContextManager contextManager,
            IPublishingStatusCache publishingStatusCache,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ICacheManager cacheManager,
            IVersioningManager versioningManager,
            IUserOrganizationChecker userOrganizationChecker,
            ServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IConnectionsServiceInternal connectionsService

        ) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {

            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.contextManager = contextManager;
            this.versioningManager = versioningManager;
            this.utilities = utilities;
            this.commonService = commonService;
            this.connectionsService = connectionsService;
        }
        
        #region Service

        public VmTranslationOrderStateSaveOutputs SendServiceEntityToTranslation(VmTranslationOrderInput model)
        {
            VmTranslationOrderStateSaveOutputs result = null;
            
            contextManager.ExecuteWriter(unitOfWork =>
            {
                CheckServiceOrderUpdate(model, unitOfWork);
                SaveServiceTranslationOrder(unitOfWork, model);
                unitOfWork.Save();
                result = GetServiceTranslationSaveData(unitOfWork, model.EntityId, model.SourceLanguage);
            });

            return result;
        }

        private void CheckServiceOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork)
        {
            var rootEntityId = versioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, model.EntityId);
            if (rootEntityId.HasValue)
            {
                foreach (var targetLanguage in model.RequiredLanguages)
                {
                    var isDelivered = IsServiceLanguageVersionDelivered(unitOfWork, rootEntityId.Value, targetLanguage);
                    if (isDelivered.TranslationOrderId.IsAssigned() && !isDelivered.Result) //Check update posibility 
                    {
                        throw new PtvTranslationException();
                    }
                }
            }
        }

        public VmTranslationOrderStateOutputs GetServiceTranslationData(VmTranslationDataInput model)
        {
            //Test service
            //var testData = "{\r\n  \"Name\": {\r\n    \"Text\": \"Name Prelozeno\",\r\n    \"MaxLength\": 100\r\n  },\r\n  \"AlternateName\": null,\r\n  \"Summary\": {\r\n    \"Text\": \"PRELOZENO Maksuttoman Matchmaking-palvelun avulla yritykset voivat etsiä uusia yhteistyömahdollisuuksia ja ilmoittaa omista hankkeista, joihin etsitään liikeku\",\r\n    \"MaxLength\": 150\r\n  },\r\n  \"Description\": {\r\n    \"Text\": \"PRELOZENO Maksuttoman Matchmaking-palvelun avulla yritykset voivat etsiä uusia yhteistyömahdollisuuksia ja ilmoittaa omista hankkeista, joihin etsitään liikekumppania.\\n\\nFinnpartnership välittää yritysten jättämiä liikekumppanuusaloitteita kehitysmaista suomalaisyrityksille ja päinvastoin. Liikekumppanuuksilla tarkoitetaan suomalaisten ja kehitysmaayritysten tai muiden taloudellisten toimijoiden pitkäaikaista yhteistoimintaa, esimerkiksi yhteisyritystä tai tuontia kehitysmaista.\\n\\nRekisteröityminen palveluun tapahtuu täyttämällä Matchmaking-hakulomake sähköisessä muodossa ja lähettämällä se osoitteeseen matchmaking(at)finnpartnership.fi. Rekisteröinnin jälkeen yrityksen yhteystiedot lisätään Finnpartnershipin nettisivuilla olevaan Partner Search -tietokantaan, josta niitä voi etsiä hakukoneen avulla. Valituissa tapauksissa käytetään ulkopuolisia konsultteja tarkistamaan hakulomakkeita ja yrityksiä ja etsimään näille potentiaalisia partnereita.\",\r\n    \"MaxLength\": 2500\r\n  },\r\n  \"ConditionsAndCriteria\": {\r\n    \"Text\": \"PRELOZENO Palvelu on tarkoitettu yrityksille, jotka etsivät liikekumppaneita kehitysmaista sekä kehitysmaayrityksille, jotka etsivät liikekumppaneita Suomesta.\",\r\n    \"MaxLength\": 2500\r\n  },\r\n  \"UserInstructions\": {\r\n    \"Text\": \"PRELOZENO- Täytä Matchmaking-hakulomake sähköisessä muodossa ja lähetä se osoitteeseen matchmaking(at)finnpartnership.fi. Täytä lomake englanniksi.\\n- Finnpartnership käy läpi Matchmaking Registration -lomakkeen ja on tarvittaessa yhteydessä.\\n- Jos hakemus on Finnpartnershipin palveluun soveltuva, tullaan yrityksen yhteystiedot lisäämään Finnpartnershipin nettisivuilla olevaan Partner Search-tietokantaan, josta niitä voi etsiä hakukoneen avulla.\\n- Finnpartnership voi lähettää tietoa yrityksestä suoraan sopivaksi katsomilleen yrityksille, tuonti/vientiorganisaatioille tai muille relevanteille yrityskontakteille. Yrityksestä kiinnostuneet tahot ottavat suoraan yhteyttä.\",\r\n    \"MaxLength\": 2500\r\n  },\r\n  \"ChargeAdditionalInformation\": null,\r\n  \"Deadline\": null,\r\n  \"ProcessingTime\": null,\r\n  \"PeriodOfValidity\": null\r\n}";
            
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetServiceTranslationOrderStates(unitOfWork, model.EntityId, model.SourceLanguage);
            });
        }

        private VmTranslationOrderStateSaveOutputs GetServiceTranslationSaveData(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            return new VmTranslationOrderStateSaveOutputs
            {
                Id = entityId,
                Services = new List<VmTranslationOrderAvailabilitySaveOutputs>
                {
                    new VmTranslationOrderAvailabilitySaveOutputs
                    {
                        Id = entityId,
                        TranslationAvailability = GetServiceTranslationAvailabilities(unitOfWork, entityId, versioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, entityId).Value)
                    }
                },
                Translations = new List<VmTranslationOrderStateOutputs>
                {
                    GetServiceTranslationOrderStates(unitOfWork, entityId, languageId)
                }
            };
        }

        private void SaveServiceTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model)
        {
            var lastOrderIdentifier = GetLastOrderIdentifier(unitOfWork);
            foreach (var targetLangaugeId in model.RequiredLanguages)
            {
                model.OrderIdentifier = ++lastOrderIdentifier;
                SetServiceModelValues(unitOfWork, model, targetLangaugeId);

                var data = TranslationManagerToEntity.Translate<VmTranslationOrderInput, ServiceTranslationOrder>(model, unitOfWork);
                data.TranslationOrder.PreviousTranslationOrderId = IsServiceLanguageVersionDelivered(unitOfWork, model.EntityRootId, targetLangaugeId).TranslationOrderId; //Is delivered only

                var sourceJsonData = GenerateServiceTranslationJson(unitOfWork,
                    new VmJsonTranslationInput()
                    {
                        Id = model.EntityId,
                        SourceLanguage = model.SourceLanguage
                    });
                
                if (string.IsNullOrEmpty(sourceJsonData.Json))
                {
                    //throw new Exception();
                }
                else
                {
                    data.TranslationOrder.SourceLanguageCharAmount = sourceJsonData.JsonTextFieldsCharAmount;
                    data.TranslationOrder.SourceLanguageData = sourceJsonData.Json;
                    data.TranslationOrder.SourceLanguageDataHash = sourceJsonData.Json?.GetHashCode().ToString();
                }
            }
        }

        private void SetServiceModelValues(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model, Guid targetLangaugeId)
        {
            model.TargetLanguage = targetLangaugeId;

            var serviceVersioned = GetEntity<ServiceVersioned>(model.EntityId, unitOfWork,
                q => q.Include(x => x.ServiceNames)
                    .Include(x => x.Organization));
            model.SourceEntityName = serviceVersioned?.ServiceNames.Where(x => x.LocalizationId == model.SourceLanguage)
                .Select(x => x.Name).FirstOrDefault();

            if (serviceVersioned != null) //TODO throw exception
            {
                model.EntityRootId = serviceVersioned.UnificRootId;
                SetOrganizationModelValues(unitOfWork, model, serviceVersioned.OrganizationId);
            }
        }

        private void SetOrganizationModelValues(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model, Guid organizationId)
        {
            var organizationVersionInfo =
                versioningManager.GetLastPublishedModifiedDraftVersion<OrganizationVersioned>(unitOfWork, organizationId);

            var organizationVersioned = GetEntity<OrganizationVersioned>(organizationVersionInfo?.EntityId, unitOfWork,
                q => q.Include(x => x.OrganizationNames)
                    .Include(x => x.Business));

            model.OrganizationIdentifier = organizationVersioned?.UnificRootId;
            model.OrganizationName = organizationVersioned?.OrganizationNames
                .Where(x => x.LocalizationId == model.SourceLanguage).Select(x => x.Name).FirstOrDefault();
            model.OrganizationBusinessCode = organizationVersioned?.Business?.Code;
        }

        private VmTranslationOrderStateOutputs GetServiceTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var unificRootId = versioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, entityId);

            var translationOrders = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder)
                .ThenInclude(i => i.ServiceTranslationOrders)
                .ThenInclude(i => i.Service)
                .Include(i => i.TranslationOrder)
                .ThenInclude(i => i.TranslationCompany)
                .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y => y.ServiceId == unificRootId) &&
                            x.TranslationOrder.SourceLanguageId == languageId)
                .OrderByDescending(x => x.SendAt).ToList();

            var targetLanguagesInUse = translationOrders.Select(x => x.TranslationOrder.TargetLanguageId).Distinct()
               .ToList();
        
            var serviceOrderExist = translationOrderStateRep.All()
               .Include(i => i.TranslationOrder)
               .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y => y.ServiceId == unificRootId))
               .Any();
            
            if (serviceOrderExist && !targetLanguagesInUse.Any())
            {
                targetLanguagesInUse.AddRange(new List<Guid>
                {
                    languageCache.Get(LanguageCode.fi.ToString()),
                    languageCache.Get(LanguageCode.sv.ToString()),
                    languageCache.Get(LanguageCode.en.ToString())
                });
            }
            var translationOrderStates = TranslationManagerToVm.TranslateAll<TranslationOrderState, VmTranslationOrderStateOutput>(translationOrders);

            foreach (var orderState in translationOrderStates) //Content id
            {
                orderState.TranslationOrder.ContentId = GetTranslationOrderParentBaseId(unitOfWork, orderState.TranslationOrder.Id);
            }
            
            return new VmTranslationOrderStateOutputs()
            {
                Id = entityId,
                TranslationOrderStates = translationOrderStates,
                TargetLanguagesInUse = targetLanguagesInUse.Distinct().ToList(),
            };
        }

        private VmJsonTranslationData GenerateServiceTranslationJson(IUnitOfWork unitOfWork, VmJsonTranslationInput model)
        {
            var service = GetEntity<ServiceVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.ServiceNames)
                    .Include(x => x.ServiceDescriptions)
                    .Include(x => x.ServiceRequirements));

            TranslationManagerToVm.SetLanguage(model.SourceLanguage);
            var jsonModel = TranslationManagerToVm.Translate<ServiceVersioned, VmJsonServiceTranslation>(service);

            return new VmJsonTranslationData()
            {
                Json = JsonConvert.SerializeObject(jsonModel, Formatting.Indented),
                JsonTextFieldsCharAmount = GetCharacterCountOfVmJsonTranslationTexts(jsonModel)
            };
        }

        public VmTranslationOrderDataResult IsServiceLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep.All()
                .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y => y.ServiceId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        private VmTranslationOrderDataResult IsServiceLanguageVersionInStateByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates, bool confirm = false)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep.All()
                .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y => y.ServiceId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Checked == confirm)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        public VmTranslationOrderDataResult IsServiceLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsServiceLanguageVersionInState(unitOfWork, rootId, languageId, new List<TranslationStateTypeEnum>() {TranslationStateTypeEnum.Arrived});
        }

        private VmTranslationOrderDataResult IsServiceLanguageVersionDeliveredByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, bool confirm)
        {
            return IsServiceLanguageVersionInStateByUserConfirm(unitOfWork, rootId, languageId,
                new List<TranslationStateTypeEnum>(){ TranslationStateTypeEnum.Arrived}, confirm: confirm);
        }

        public bool IsServiceLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsServiceLanguageVersionInState(unitOfWork, rootId, languageId, GetInTranslationStateList()).Result;
        }

        public void ConfirmServiceDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId)
        {
            var unificRootId = versioningManager.GetUnificRootId<ServiceVersioned>(unitOfWork, versionId);
            if (unificRootId != null)
            {
                var isDeliveredDataWithoutUserConfirm = IsServiceLanguageVersionDeliveredByUserConfirm(unitOfWork, unificRootId.Value, languageId, false);
                if (isDeliveredDataWithoutUserConfirm.Result && isDeliveredDataWithoutUserConfirm.OrderStatusId.HasValue)
                {
                    UpdateTranslationOrderState(unitOfWork, isDeliveredDataWithoutUserConfirm.OrderStatusId.Value, true);
                }
            }
        }

        #endregion Service

        #region General description

        public VmTranslationOrderStateSaveOutputs SendGeneralDescriptionEntityToTranslation(VmTranslationOrderInput model)
        {
            VmTranslationOrderStateSaveOutputs result = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                CheckGeneralDescriptionOrderUpdate(model, unitOfWork);
                SaveGeneralDescriptionTranslationOrder(unitOfWork, model);
                unitOfWork.Save();
                result = GetGeneralDescriptionTranslationSaveData(unitOfWork, model.EntityId, model.SourceLanguage);
            });

            return result;
        }

        private void CheckGeneralDescriptionOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork)
        {
            var rootEntityId = versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, model.EntityId);
            if (rootEntityId.HasValue)
            {
                foreach (var targetLanguage in model.RequiredLanguages)
                {
                    var isDelivered = IsGeneralDescriptionLanguageVersionDelivered(unitOfWork, rootEntityId.Value, targetLanguage);
                    if (isDelivered.TranslationOrderId.IsAssigned() && !isDelivered.Result) //Check update posibility 
                    {
                        throw new PtvTranslationException();
                    }
                }
            }
        }
        
        public VmTranslationOrderStateOutputs GetGeneralDescriptionTranslationData(VmTranslationDataInput model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetGeneralDescriptionTranslationOrderStates(unitOfWork, model.EntityId, model.SourceLanguage);
            });
        }

        private VmTranslationOrderStateSaveOutputs GetGeneralDescriptionTranslationSaveData(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            return new VmTranslationOrderStateSaveOutputs
            {
                Id = entityId,
                GeneralDescriptions = new List<VmTranslationOrderAvailabilitySaveOutputs>
                {
                    new VmTranslationOrderAvailabilitySaveOutputs
                    {
                        Id = entityId,
                        TranslationAvailability = GetGeneralDescriptionTranslationAvailabilities(unitOfWork, entityId, versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, entityId).Value)
                    }
                },
                Translations = new List<VmTranslationOrderStateOutputs>
                {
                    GetGeneralDescriptionTranslationOrderStates(unitOfWork, entityId, languageId)
                }
            };
        }

        private void SaveGeneralDescriptionTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model)
        {
            var lastOrderIdentifier = GetLastOrderIdentifier(unitOfWork);
            foreach (var targetLangaugeId in model.RequiredLanguages)
            {
                model.OrderIdentifier = ++lastOrderIdentifier;
                SetGeneralDescriptionModelValues(unitOfWork, model, targetLangaugeId);

                var data = TranslationManagerToEntity.Translate<VmTranslationOrderInput, GeneralDescriptionTranslationOrder>(model, unitOfWork);
                data.TranslationOrder.PreviousTranslationOrderId = IsGeneralDescriptionLanguageVersionDelivered(unitOfWork, model.EntityRootId, targetLangaugeId).TranslationOrderId;

                var sourceJsonData = GenerateGeneralDescriptionTranslationJson(unitOfWork,
                    new VmJsonTranslationInput()
                    {
                        Id = model.EntityId,
                        SourceLanguage = model.SourceLanguage
                    });

                if (string.IsNullOrEmpty(sourceJsonData.Json))
                {
                    //throw new Exception();
                }
                else
                {
                    data.TranslationOrder.SourceLanguageCharAmount = sourceJsonData.JsonTextFieldsCharAmount;
                    data.TranslationOrder.SourceLanguageData = sourceJsonData.Json;
                    data.TranslationOrder.SourceLanguageDataHash = sourceJsonData.Json?.GetHashCode().ToString();
                }
            }
        }

        private void SetGeneralDescriptionModelValues(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model, Guid targetLangaugeId)
        {
            model.TargetLanguage = targetLangaugeId;

            var generalDescriptionVersioned = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(model.EntityId, unitOfWork,
                q => q.Include(x => x.Names));

            model.SourceEntityName = generalDescriptionVersioned?.Names.Where(x => x.LocalizationId == model.SourceLanguage)
                .Select(x => x.Name).FirstOrDefault();

            if (generalDescriptionVersioned != null)
            {
                model.EntityRootId = generalDescriptionVersioned.UnificRootId;
            }
        }

        private VmTranslationOrderStateOutputs GetGeneralDescriptionTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            var unificRootId = versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, entityId);
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrders = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder)
                .ThenInclude(i => i.GeneralDescriptionTranslationOrders)
                .ThenInclude(i => i.StatutoryServiceGeneralDescription)
                .Include(i => i.TranslationOrder)
                .ThenInclude(i => i.TranslationCompany)
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any(y => y.StatutoryServiceGeneralDescriptionId == unificRootId) &&
                            x.TranslationOrder.SourceLanguageId == languageId)
                .OrderByDescending(x => x.SendAt).ToList();

            var targetLanguagesInUse = translationOrders.Select(x => x.TranslationOrder.TargetLanguageId).Distinct()
                .ToList();

            var entityOrderExist = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder)
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any(y => y.StatutoryServiceGeneralDescriptionId == unificRootId))
                .Any();

            if (entityOrderExist && !targetLanguagesInUse.Any())
            {
                targetLanguagesInUse.AddRange(new List<Guid>
                {
                    languageCache.Get(LanguageCode.fi.ToString()),
                    languageCache.Get(LanguageCode.sv.ToString()),
                    languageCache.Get(LanguageCode.en.ToString())
                });
            }

            var translationOrderStates = TranslationManagerToVm.TranslateAll<TranslationOrderState, VmTranslationOrderStateOutput>(translationOrders);

            foreach (var orderState in translationOrderStates) //Content Id
            {
                orderState.TranslationOrder.ContentId = GetTranslationOrderParentBaseId(unitOfWork, orderState.TranslationOrder.Id);
            }

            return new VmTranslationOrderStateOutputs()
            {
                Id = entityId,
                TranslationOrderStates = translationOrderStates,
                TargetLanguagesInUse = targetLanguagesInUse
            };
        }

        private VmJsonTranslationData GenerateGeneralDescriptionTranslationJson(IUnitOfWork unitOfWork, VmJsonTranslationInput model)
        {
            var generalDescriptionVersioned = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.Names)
                    .Include(x => x.Descriptions)
                    .Include(x => x.StatutoryServiceRequirements));

            TranslationManagerToVm.SetLanguage(model.SourceLanguage);
            var jsonModel = TranslationManagerToVm.Translate<StatutoryServiceGeneralDescriptionVersioned, VmJsonGeneralDescriptionTranslation>(generalDescriptionVersioned);

            return new VmJsonTranslationData()
            {
                Json = JsonConvert.SerializeObject(jsonModel, Formatting.Indented),
                JsonTextFieldsCharAmount = GetCharacterCountOfVmJsonTranslationTexts(jsonModel)
            };
        }

        public VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep.All()
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any(y => y.StatutoryServiceGeneralDescriptionId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        private VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionInStateByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates, bool confirm = false)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep.All()
                .Where(x => x.TranslationOrder.GeneralDescriptionTranslationOrders.Any(y => y.StatutoryServiceGeneralDescriptionId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Checked == confirm)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        public VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsGeneralDescriptionLanguageVersionInState(unitOfWork, rootId, languageId, new List<TranslationStateTypeEnum>() { TranslationStateTypeEnum.Arrived });
        }

        private VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionDeliveredByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, bool confirm)
        {
            return IsGeneralDescriptionLanguageVersionInStateByUserConfirm(unitOfWork, rootId, languageId,
                new List<TranslationStateTypeEnum>() { TranslationStateTypeEnum.Arrived }, confirm: confirm);
        }

        public bool IsGeneralDescriptionLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsGeneralDescriptionLanguageVersionInState(unitOfWork, rootId, languageId, GetInTranslationStateList()).Result;
        }

        public void ConfirmGeneralDescriptionDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId)
        {
            var unificRootId = versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, versionId);
            var isDeliveredData = IsGeneralDescriptionLanguageVersionDeliveredByUserConfirm(unitOfWork, unificRootId.Value, languageId, false);
            if (isDeliveredData.Result && isDeliveredData.OrderStatusId.HasValue)
            {
                UpdateTranslationOrderState(unitOfWork, isDeliveredData.OrderStatusId.Value, true);
            }
        }

        #endregion  General description

        #region Channels

        public VmTranslationOrderStateSaveOutputs SendChannelEntityToTranslation(VmTranslationOrderInput model)
        {
            VmTranslationOrderStateSaveOutputs result = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                CheckChannelOrderUpdate(model, unitOfWork);

                SaveChannelTranslationOrder(unitOfWork, model);
                unitOfWork.Save();
                result = GetChannelTranslationSaveData(unitOfWork, model.EntityId, model.SourceLanguage);
            });

            return result;
        }

        private void CheckChannelOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork)
        {
            var rootEntityId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, model.EntityId);
            if (rootEntityId.HasValue)
            {
                foreach (var targetLanguage in model.RequiredLanguages)
                {
                    var isDelivered = IsChannelLanguageVersionDelivered(unitOfWork, rootEntityId.Value, targetLanguage);
                    if (isDelivered.TranslationOrderId.IsAssigned() && !isDelivered.Result) //Check update posibility 
                    {
                        throw new PtvTranslationException();
                    }
                }
            }
        }

        public VmTranslationOrderStateOutputs GetChannelTranslationData(VmTranslationDataInput model)
        {
            //Test channel webservice
            //var testData = "{\r\n  \"Name\": {\r\n    \"Text\": \"PRELOZENO Sobota test preklad do neexistujicho jazyka SV\",\r\n    \"MaxLength\": 100\r\n  },\r\n  \"Summary\": {\r\n    \"Text\": \" PRELOZENO Veroprosenttilaskurilla voit arvioida, tarvitsetko muutoksen verokortissasi näkyvään ennakonpidätysprosenttiin.\",\r\n    \"MaxLength\": 150\r\n  },\r\n  \"Description\": {\r\n    \"Text\": \" PRELOZENO Veroprosenttilaskurilla voit arvioida, tarvitsetko muutoksen verokortissasi näkyvään ennakonpidätysprosenttiin. Käytä palvelua silloin kun tulosi tai vähennyksesi ovat oleellisesti muuttuneet esimerkiksi työstä pois jäännin tai työhön menon vuoksi. Laskurilla voit arvioida veron tarpeen tavallisimmissa tapauksissa, esimerkiksi kun saat palkkaa, erilaisia Kelan ja työttömyyskassojen maksamia etuuksia, opintorahaa, merityötuloa tai eläkettä.\\nLaskuri soveltuu myös yrittäjien (YEL) ja maatalousyrittäjien (MYEL) eläkelakien mukaan vakuutetuille henkilöille, joiden sairausvakuutusmaksu on laskettu vuodesta 2003 alkaen eri tulojen perusteella, kuin muilla tulonsaajilla. Näillä henkilöillä YEL-/MYEL-työtulo korvaa yritystoiminnasta saadut ansiotulot. Sairausvakuutusmaksu lasketaan työtulon ja niiden ansiotulojen perusteella, joita ei ole korvattu.\\nLaskuri soveltuu myös tilanteisiin, joissa tuloista ei makseta Suomeen sairausvakuutusmaksuja.\",\r\n    \"MaxLength\": 2500\r\n  }\r\n}";

            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetChannelTranslationOrderStates(unitOfWork, model.EntityId, model.SourceLanguage);
            });
        }

        private VmTranslationOrderStateSaveOutputs GetChannelTranslationSaveData(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            return new VmTranslationOrderStateSaveOutputs
            {
                Id = entityId,
                Channels = new List<VmTranslationOrderAvailabilitySaveOutputs>
                {
                    new VmTranslationOrderAvailabilitySaveOutputs
                    {
                        Id = entityId,
                        TranslationAvailability = GetChannelTranslationAvailabilities(unitOfWork, entityId, versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, entityId).Value, null)
                    }
                },
                Translations = new List<VmTranslationOrderStateOutputs>
                {
                    GetChannelTranslationOrderStates(unitOfWork, entityId, languageId)
                }
            };
        }

        private void SaveChannelTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model)
        {
            var lastOrderIdentifier = GetLastOrderIdentifier(unitOfWork);
            foreach (var targetLangaugeId in model.RequiredLanguages)
            {
                model.OrderIdentifier = ++lastOrderIdentifier;
                SetChannelModelValues(unitOfWork, model, targetLangaugeId);

                var data = TranslationManagerToEntity.Translate<VmTranslationOrderInput, ServiceChannelTranslationOrder>(model, unitOfWork);
                data.TranslationOrder.PreviousTranslationOrderId = IsChannelLanguageVersionDelivered(unitOfWork, model.EntityRootId, targetLangaugeId).TranslationOrderId;

                var sourceJsonData = GenerateChannelTranslationJson(unitOfWork,
                    new VmJsonTranslationInput()
                    {
                        Id = model.EntityId,
                        SourceLanguage = model.SourceLanguage
                    });

                if (string.IsNullOrEmpty(sourceJsonData.Json))
                {
                    //throw new Exception();
                }
                else
                {
                    data.TranslationOrder.SourceLanguageCharAmount = sourceJsonData.JsonTextFieldsCharAmount;
                    data.TranslationOrder.SourceLanguageData = sourceJsonData.Json;
                    data.TranslationOrder.SourceLanguageDataHash = sourceJsonData.Json?.GetHashCode().ToString();
                }
            }
        }

        private void SetChannelModelValues(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model, Guid targetLangaugeId)
        {
            model.TargetLanguage = targetLangaugeId;

            var channelVersioned = GetEntity<ServiceChannelVersioned>(model.EntityId, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames)
                    .Include(x => x.Organization));
            model.SourceEntityName = channelVersioned?.ServiceChannelNames.Where(x => x.LocalizationId == model.SourceLanguage)
                .Select(x => x.Name).FirstOrDefault();

            if (channelVersioned != null)
            {
                model.EntityRootId = channelVersioned.UnificRootId;
                SetOrganizationModelValues(unitOfWork, model, channelVersioned.OrganizationId);
            }
        }

        private VmTranslationOrderStateOutputs GetChannelTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId)
        {
            var unificRootId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, entityId);
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrders = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder)
                .ThenInclude(i => i.ServiceChannelTranslationOrders)
                .ThenInclude(i => i.ServiceChannel)
                .Include(i => i.TranslationOrder)
                .ThenInclude(i => i.TranslationCompany)
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any(y => y.ServiceChannelId == unificRootId) &&
                            x.TranslationOrder.SourceLanguageId == languageId)
                .OrderByDescending(x => x.SendAt).ToList();
            
            var targetLanguagesInUse = translationOrders.Select(x => x.TranslationOrder.TargetLanguageId).Distinct()
                .ToList();

            var entityOrderExist = translationOrderStateRep.All()
                .Include(i => i.TranslationOrder)
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any(y => y.ServiceChannelId == unificRootId))
                .Any();

            if (entityOrderExist && !targetLanguagesInUse.Any())
            {
                targetLanguagesInUse.AddRange(new List<Guid>
                {
                    languageCache.Get(LanguageCode.fi.ToString()),
                    languageCache.Get(LanguageCode.sv.ToString()),
                    languageCache.Get(LanguageCode.en.ToString())
                });
            }

            var translationOrderStates = TranslationManagerToVm.TranslateAll<TranslationOrderState, VmTranslationOrderStateOutput>(translationOrders);

            foreach (var orderState in translationOrderStates) //Content Id
            {
                orderState.TranslationOrder.ContentId = GetTranslationOrderParentBaseId(unitOfWork, orderState.TranslationOrder.Id);
            }
            
            return new VmTranslationOrderStateOutputs()
            {
                Id = entityId,
                TranslationOrderStates = translationOrderStates,
                TargetLanguagesInUse = targetLanguagesInUse
            };
        }

        private VmJsonTranslationData GenerateChannelTranslationJson(IUnitOfWork unitOfWork, VmJsonTranslationInput model)
        {
            var entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames)
                    .Include(x => x.ServiceChannelDescriptions));

            TranslationManagerToVm.SetLanguage(model.SourceLanguage);
            var jsonModel = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmJsonChannelTranslation>(entity);

            return new VmJsonTranslationData()
            {
                Json = JsonConvert.SerializeObject(jsonModel, Formatting.Indented),
                JsonTextFieldsCharAmount = GetCharacterCountOfVmJsonTranslationTexts(jsonModel)
            };
        }

        public VmTranslationOrderDataResult IsChannelLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep.All()
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any(y => y.ServiceChannelId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }

        private VmTranslationOrderDataResult IsChannelLanguageLanguageVersionInStateByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates, bool confirm = false)
        {
            var translationOrderStateRep = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
            var translationOrderState = translationOrderStateRep.All()
                .Where(x => x.TranslationOrder.ServiceChannelTranslationOrders.Any(y => y.ServiceChannelId == rootId) &&
                            x.TranslationOrder.TargetLanguageId == languageId &&
                            x.Checked == confirm)
                .OrderByDescending(x => x.SendAt)
                .FirstOrDefault();

            return GetTranslationOrderIsInStateResult(translationOrderState, translationStates);
        }
        
        public VmTranslationOrderDataResult IsChannelLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsChannelLanguageVersionInState(unitOfWork, rootId, languageId, new List<TranslationStateTypeEnum>() { TranslationStateTypeEnum.Arrived });
        }

        private VmTranslationOrderDataResult IsChannelLanguageVersionDeliveredByUserConfirm(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, bool confirm)
        {
            return IsChannelLanguageLanguageVersionInStateByUserConfirm(unitOfWork, rootId, languageId,
                new List<TranslationStateTypeEnum>() { TranslationStateTypeEnum.Arrived }, confirm: confirm);
        }

        public bool IsChannelLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId)
        {
            return IsChannelLanguageVersionInState(unitOfWork, rootId, languageId, GetInTranslationStateList()).Result;
        }

        public void ConfirmChannelDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId)
        {
            var unificRootId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, versionId);
            var isDeliveredDataWithoutUserConfirm = IsChannelLanguageVersionDeliveredByUserConfirm(unitOfWork, unificRootId.Value, languageId, false);
            if (isDeliveredDataWithoutUserConfirm.Result && isDeliveredDataWithoutUserConfirm.OrderStatusId.HasValue)
            {
                UpdateTranslationOrderState(unitOfWork ,isDeliveredDataWithoutUserConfirm.OrderStatusId.Value, true);
            }
        }

        #endregion channels

        public long GetLastOrderIdentifier(IUnitOfWorkWritable unitOfWork)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();

            return translationOrderRep.All()
                .OrderByDescending(x => x.Created)
                .ThenBy(x => x.OrderIdentifier)
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

        private List<TranslationStateTypeEnum> GetInTranslationStateList()
        {
            var translationList = new List<TranslationStateTypeEnum>()
            {
                TranslationStateTypeEnum.ReadyToSend,
                TranslationStateTypeEnum.Sent,
                TranslationStateTypeEnum.InProgress,
                TranslationStateTypeEnum.FileError,
                TranslationStateTypeEnum.SendError,
                TranslationStateTypeEnum.DeliveredFileError
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

        private List<TranslationOrder> GetTranslationOrdersByState(IUnitOfWork unitOfWork, Guid stateId)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var translationOrderList = translationOrderRep.All()
                .Include(i => i.TranslationOrderStates)
                .Where(x => x.TranslationOrderStates
                                .OrderByDescending(y => y.SendAt)
                                .FirstOrDefault().TranslationStateId == stateId )
                                .ToList();

            return translationOrderList;
        }

        private List<TranslationOrder> GetTranslationOrdersByStateList(IUnitOfWork unitOfWork, List<Guid> states)
        {
            var translationOrderRep = unitOfWork.CreateRepository<ITranslationOrderRepository>();
            var results = new List<TranslationOrder>();

            foreach (var state in states)
            {
               results.AddRange(translationOrderRep.All()
                    .Include(i => i.TranslationOrderStates)
                    .Where(x => state == (x.TranslationOrderStates
                                    .OrderByDescending(y => y.SendAt)
                                    .FirstOrDefault().TranslationStateId))
                    .ToList());
            }

            return results;
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

        public void AddTranslationOrderState(Guid translationOrderId, TranslationStateTypeEnum? translationStateType, DateTime? deliverAt = null)
        {
            if (!translationStateType.HasValue) return; 

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var stateTypeId = typesCache.Get<TranslationStateType>(translationStateType.ToString());
                AddTranslationOrderState(unitOfWork, translationOrderId, stateTypeId, deliverAt);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        private void AddTranslationOrderState(IUnitOfWorkWritable unitOfWork, Guid translationOrderId, Guid stateId, DateTime? deliverAt = null)
        {  
            var vm = new VmTranslationOrderStateInput(){ TranslationOrderId = translationOrderId, TranslationStateId = stateId};
            TranslationManagerToEntity.Translate<VmTranslationOrderStateInput, TranslationOrderState>(vm, unitOfWork);
            UpdateTranslationOrderDeliverAt(unitOfWork, translationOrderId, deliverAt);
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
            return new VmTranslationOrderDataResult()
            {
                Result = result,
                TranslationOrderId = translationOrderState?.TranslationOrderId,
                OrderStatusId = result ? translationOrderState?.Id : (Guid?)null
            };
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
                    throw new PtvTranslationException($"Error of saving translated file to translation order({translationOrderId}).", e);
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

                try
                {
                    if (translationOrder.ServiceTranslationOrders.Any())
                    {
                        var modelData = JsonConvert.DeserializeObject<VmJsonServiceTranslation>(targetFileJson);
                        modelData.Id = GetLastVersionOfEntityId<ServiceVersioned>(unitOfWork, translationOrder.ServiceTranslationOrders.Select(x => x.ServiceId).FirstOrDefault());
                        TranslationManagerToEntity.Translate<VmJsonServiceTranslation, ServiceVersioned>(modelData, unitOfWork);
                    }
                    else if (translationOrder.ServiceChannelTranslationOrders.Any())
                    {
                        var modelData = JsonConvert.DeserializeObject<VmJsonChannelTranslation>(targetFileJson);
                        modelData.Id = GetLastVersionOfEntityId<ServiceChannelVersioned>(unitOfWork, translationOrder.ServiceChannelTranslationOrders.Select(x => x.ServiceChannelId).FirstOrDefault());
                        TranslationManagerToEntity.Translate<VmJsonChannelTranslation, ServiceChannelVersioned>(modelData, unitOfWork);
                    }
                    else if (translationOrder.GeneralDescriptionTranslationOrders.Any())
                    {
                        var modelData = JsonConvert.DeserializeObject<VmJsonGeneralDescriptionTranslation>(targetFileJson);
                        modelData.Id = GetLastVersionOfEntityId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, translationOrder.GeneralDescriptionTranslationOrders.Select(x => x.StatutoryServiceGeneralDescriptionId).FirstOrDefault());
                        TranslationManagerToEntity.Translate<VmJsonGeneralDescriptionTranslation, StatutoryServiceGeneralDescriptionVersioned>(modelData, unitOfWork);
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

            return versioningManager.GetLastPublishedModifiedDraftVersion<TEntityType>(unitOfWork, unificRootId.Value).EntityId;
        }

        #endregion
        //SOAP VM

        #region translation availability
        public Dictionary<string, VmTranslationOrderAvailability> GetServiceTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId)
        {
            var canBeTranslated = utilities.CanBeTranslated<ServiceVersioned>(new List<Guid> { entityVersionedId }, unitOfWork, entityVersionedId);

            return commonService.GetTranslationLanguageList()
                .ToDictionary(x => x, y => new VmTranslationOrderAvailability()
                {
                    IsInTranslation = IsServiceLanguageVersionInTranslation(unitOfWork, entityRootId, languageCache.Get(y)),
                    IsTranslationDelivered = IsServiceLanguageVersionDeliveredByUserConfirm(unitOfWork, entityRootId, languageCache.Get(y), false).Result,
                    CanBeTranslated = canBeTranslated.ContainsKey(y) && canBeTranslated[y]
                });
        }

        public Dictionary<string, VmTranslationOrderAvailability> GetChannelTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId, List<VmChannelConnectionOutput> connections)
        {
            var connectionsIds = connections == null ? connectionsService.GetServiceChannelRelationIds(unitOfWork, entityRootId) : connections.Select(x => x.Id).ToList();
            var canBeTranslated = utilities.CanBeTranslated<ServiceChannelVersioned>(connectionsIds, unitOfWork, entityVersionedId);
            return commonService.GetTranslationLanguageList()
                .ToDictionary(x => x, y => new VmTranslationOrderAvailability()
                {
                    IsInTranslation = IsChannelLanguageVersionInTranslation(unitOfWork, entityRootId, languageCache.Get(y)),
                    IsTranslationDelivered = IsChannelLanguageVersionDeliveredByUserConfirm(unitOfWork, entityRootId, languageCache.Get(y), false).Result,
                    CanBeTranslated = canBeTranslated.ContainsKey(y) && canBeTranslated[y]
                });
        }

        public Dictionary<string, VmTranslationOrderAvailability> GetGeneralDescriptionTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId)
        {
            var canBeTranslated = utilities.CanBeGeneralDescriptionTranslated(unitOfWork, entityVersionedId); ;

            return commonService.GetTranslationLanguageList()
                .ToDictionary(x => x, y => new VmTranslationOrderAvailability()
                {
                    IsInTranslation = IsGeneralDescriptionLanguageVersionInTranslation(unitOfWork, entityRootId, languageCache.Get(y)),
                    IsTranslationDelivered = IsGeneralDescriptionLanguageVersionDeliveredByUserConfirm(unitOfWork, entityRootId, languageCache.Get(y), false).Result,
                    CanBeTranslated = canBeTranslated.ContainsKey(y) && canBeTranslated[y]
                });
        }
        #endregion

        #region OpenApi

        public IVmOpenApiGuidPageVersionBase GetTranslationItems(DateTime? date, int pageNumber, int pageSize)
        {
            var vm = new VmOpenApiTranslationItemsPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var filters = new List<Expression<Func<TranslationOrder, bool>>>();
                if (date.HasValue)
                {
                    filters.Add(t => t.Modified > date.Value);
                }
                var items = GetEntitiesForPage<TranslationOrder>(vm, unitOfWork, q =>
                    q.Include(i => i.TranslationOrderStates)
                    .Include(i => i.ServiceChannelTranslationOrders)
                    .Include(i => i.ServiceTranslationOrders), filters);

                if (items?.Count > 0)
                {
                    vm.ItemList = TranslationManagerToVm.TranslateAll<TranslationOrder, VmOpenApiTranslationItem>(items).ToList();
                }
            });

            return vm;
        }

        #endregion
    }
}
