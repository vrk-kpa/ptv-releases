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

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface ITranslationService
    {
        /// <summary>
        /// Send entity to translation. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateSaveOutputs SendServiceEntityToTranslation(VmTranslationOrderInput model);

        /// <summary>
        /// Get Translation data 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetServiceTranslationData(VmTranslationDataInput model);

        /// <summary>
        /// Send entity to translation. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateSaveOutputs SendGeneralDescriptionEntityToTranslation(VmTranslationOrderInput model);

        /// <summary>
        /// Get Translation data 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetGeneralDescriptionTranslationData(VmTranslationDataInput model);

        /// <summary>
        /// Send entity to translation. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateSaveOutputs SendChannelEntityToTranslation(VmTranslationOrderInput model);

        /// <summary>
        /// Get Translation data 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetChannelTranslationData(VmTranslationDataInput model);
        
        /// <summary>
        /// Translation Order sent success
        /// </summary>
        /// <param name="translationOrderId"></param>
        /// <param name="orderIdentifier"></param>
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
        /// <param name="deliverAt"></param>
        void AddTranslationOrderState(Guid translationOrderId, TranslationStateTypeEnum? translationStateType, DateTime? deliverAt = null);

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
        /// <param name="confirm"></param>
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
        /// <param name="rootId"></param>
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
        /// <param name="confirm"></param>
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
        /// Get channel translation svailabilities
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityVersionedId"></param>
        /// <param name="entityRootId"></param>
        /// <param name="connections"></param>
        /// <returns></returns>
        Dictionary<string, VmTranslationOrderAvailability> GetChannelTranslationAvailabilities(IUnitOfWork unitOfWork, Guid entityVersionedId, Guid entityRootId, List<VmChannelConnectionOutput> connections);
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
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IVmOpenApiGuidPageVersionBase GetTranslationItems(DateTime? date, int pageNumber, int pageSize);
    }
}