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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Domain.Model.Models.V2.TranslationOrder.Soap;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface ITranslationService
    {
        /// <summary>
        /// Check service order update
        /// </summary>
        /// <param name="model"></param>
        /// <param name="unitOfWork"></param>
        void CheckServiceOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork);

        /// <summary>
        /// Save service translation order
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Guid SaveServiceTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model);

        /// <summary>
        /// Get service translation order states
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetServiceTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId);

        /// <summary>
        /// Check general description order update
        /// </summary>
        /// <param name="model"></param>
        /// <param name="unitOfWork"></param>
        void CheckGeneralDescriptionOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork);

        /// <summary>
        /// Save general description translation order
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Guid SaveGeneralDescriptionTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model);

        /// <summary>
        /// Get geeneral description translation order states
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetGeneralDescriptionTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId);

        /// <summary>
        /// Check channel order update
        /// </summary>
        /// <param name="model"></param>
        /// <param name="unitOfWork"></param>
        void CheckChannelOrderUpdate(VmTranslationOrderInput model, IUnitOfWorkWritable unitOfWork);

        /// <summary>
        /// Save channel translation order
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="model"></param>
        Guid SaveChannelTranslationOrder(IUnitOfWorkWritable unitOfWork, VmTranslationOrderInput model);

        /// <summary>
        /// Get channel translation order states
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetChannelTranslationOrderStates(IUnitOfWork unitOfWork, Guid entityId, Guid languageId);

        /// <summary>
        /// Get Translation data 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetChannelTranslationData(VmTranslationDataInput model);

        /// <summary>
        /// Translation Order sent success
        /// </summary>
        /// <param name="translationOrderId"></param>
        /// <param name="companyOrderIdentifier"></param>
        void TranslationOrderSentSuccess(Guid translationOrderId, string companyOrderIdentifier);

        /// <summary>
        /// Translation order sent failure
        /// </summary>
        /// <param name="translationOrderId"></param>
        void TranslationOrderSentFailure(Guid translationOrderId);

        /// <summary>
        /// Add new translation order with state
        /// </summary>
        /// <param name="translationOrderId"></param>
        /// <param name="translationStateType"></param>
        /// <param name="infoDetails"></param>
        /// <param name="deliverAt"></param>
        void AddTranslationOrderState(Guid translationOrderId, TranslationStateTypeEnum? translationStateType, string infoDetails = null, DateTime? deliverAt = null);

        /// <summary>
        /// Get translation order id
        /// </summary>
        /// <param name="translationOrderId"></param>
        /// <returns></returns>
        string GetTranslationDataJson(Guid translationOrderId);

        /// <summary>
        /// Processing translated file
        /// </summary>
        /// <param name="translationOrderId"></param>
        /// <param name="targetFileJson"></param>
        void ProcessTranslatedFile(Guid translationOrderId, string targetFileJson);

        /// <summary>
        /// Is service translation delivered
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        VmTranslationOrderDataResult IsServiceLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId);

        /// <summary>
        /// COnfirm service devivered translation
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="versionId"></param>
        /// <param name="languageId"></param>
        void ConfirmServiceDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId);
            
        /// <summary>
        /// Service language version in Translation
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        bool IsServiceLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId);

        /// <summary>
        /// general description language version delivered
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid rootId, Guid languageId);

        /// <summary>
        /// Is general description in state
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId"></param>
        /// <param name="languageId"></param>
        /// <param name="translationStates"></param>
        /// <returns></returns>
        VmTranslationOrderDataResult IsGeneralDescriptionLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates);

        /// <summary>
        /// General description language version in trnasaltion
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        bool IsGeneralDescriptionLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId);

        /// <summary>
        /// Confirm general description delivered translation
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="versionId"></param>
        /// <param name="languageId"></param>
        void ConfirmGeneralDescriptionDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId);
        
        /// <summary>
        /// Channel langugae version delivered
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="versionId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        VmTranslationOrderDataResult IsChannelLanguageVersionDelivered(IUnitOfWork unitOfWork, Guid versionId, Guid languageId);

        /// <summary>
        /// Is channel in state
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId"></param>
        /// <param name="languageId"></param>
        /// <param name="translationStates"></param>
        /// <returns></returns>
        VmTranslationOrderDataResult IsChannelLanguageVersionInState(IUnitOfWork unitOfWork, Guid rootId, Guid languageId, List<TranslationStateTypeEnum> translationStates);

        /// <summary>
        /// COnfirm channel delivered translation
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="versionId"></param>
        /// <param name="languageId"></param>
        void ConfirmChannelDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid versionId, Guid languageId);
        
        /// <summary>
        /// Channel language version in translation
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        bool IsChannelLanguageVersionInTranslation(IUnitOfWork unitOfWork, Guid rootId, Guid languageId);
        /// <summary>
        /// Get service translation svailabilities
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityVersionedId"></param>
        /// <param name="entityRootId"></param>
        /// <returns></returns>
        Dictionary<string, VmTranslationOrderAvailability> GetServiceTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId);
        /// <summary>
        /// Get general description translation svailabilities
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityVersionedId"></param>
        /// <param name="entityRootId"></param>
        /// <returns></returns>
        Dictionary<string, VmTranslationOrderAvailability> GetGeneralDescriptionTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId);

        /// <summary>
        /// Get translation orders by state types
        /// </summary>
        /// <param name="stateTypes"></param>
        /// <returns></returns>
        IReadOnlyList<VmSoapTranslationOrder> GetTranslationOrdersByStateTypes(IReadOnlyCollection<TranslationStateTypeEnum> stateTypes);

        /// <summary>
        /// Get service and channel related translation information.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateBefore"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiTranslationItem> GetTranslationItems(DateTime? date, DateTime? dateBefore, int pageNumber, int pageSize);

        /// <summary>
        /// Add translation errro state with repetition check
        /// </summary>
        /// <param name="translationOrderId"></param>
        /// <param name="repetitionNumber"></param>
        /// <param name="translationStateType"></param>
        /// <param name="infoDetails"></param>
        void AddTranslationErrorStateWithRepetitionCheck(Guid translationOrderId, int repetitionNumber, TranslationStateTypeEnum? translationStateType, string infoDetails = null);
        
        /// <summary>
        /// Update all missing language version in translation
        /// </summary>
        void UpdateAllMissingLanguageVersionInTranslation(IUnitOfWorkWritable unitOfWork);

        /// <summary>
        /// Adding all Missing translation entity names
        /// Names are missing only for source language, names are find in sourceLanguageData
        /// </summary>
        /// <param name="unitOfWork"></param>
        void AddAllMissingEntityNamesAfterTranslationOrder(IUnitOfWorkWritable unitOfWork);

        /// <summary>
        /// Update all wrong service processing info description influence arrived chargeAdditionalInformationDescription
        /// </summary>
        void UpdateAllWrongServiceProcessingInfo();
        
        /// <summary>
        /// Add translation order state
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="translationOrderId"></param>
        /// <param name="stateId"></param>
        /// <param name="infoDetails"></param>
        /// <param name="deliverAt"></param>
        void AddTranslationOrderState(IUnitOfWorkWritable unitOfWork, Guid translationOrderId, Guid stateId, string infoDetails = null, DateTime? deliverAt = null);

        /// <summary>
        /// Check and set last error state
        /// </summary>
        /// <param name="translationOrders"></param>
        void CheckAndSetLastErrorState(IReadOnlyList<VmSoapTranslationOrder> translationOrders);

        /// <summary>
        /// Update all wrong arrived general description processing info 
        /// </summary>
        void UpdateAllWrongGeneralDescriptionProcessingInfo();

    }

    internal interface ITranslationServiceInternal : ITranslationService
    {
        /// <summary>
        /// Get channel translation svailabilities
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityVersionedId"></param>
        /// <param name="entityRootId"></param>
        /// <param name="connections"></param>
        /// <returns></returns>
        Dictionary<string, VmTranslationOrderAvailability> GetChannelTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId, List<VmChannelConnectionOutput> connections);
    }
}
