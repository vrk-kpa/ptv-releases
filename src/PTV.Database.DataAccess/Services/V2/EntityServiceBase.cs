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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.V2
{
    internal abstract class EntityServiceBase<TVersioned, TRoot, TLanguageAvail> : ServiceBase 
        where TRoot : IVersionedRoot 
        where TVersioned : class, IEntityIdentifier, IVersionedVolume<TRoot>, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
        where TLanguageAvail : class, ILanguageAvailability
    {
        protected IContextManager ContextManager { get; }
        protected ServiceUtilities Utilities { get; }

        protected ICommonServiceInternal CommonService { get; }
        protected IValidationManager ValidationManager { get; }
        internal EntityServiceBase(ITranslationEntity translationManagerToVm, ITranslationViewModel translationManagerToEntity, IPublishingStatusCache publishingStatusCache, IUserOrganizationChecker userOrganizationChecker, IContextManager contextManager, ServiceUtilities utilities, ICommonServiceInternal commonService, IValidationManager validationManager) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            ContextManager = contextManager;
            Utilities = utilities;
            CommonService = commonService;
            ValidationManager = validationManager;
        }
        
        protected TOutModel ExecuteGet<TOutModel>(IVmEntityGet entityModel, Func<IUnitOfWork, IVmEntityGet, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var model = loadFunc(unitOfWork, entityModel);
                if (entityModel.IncludeValidation)
                {
                    model = AddValidation(model, unitOfWork);
                }
                return model;
            });
        }

        protected TOutModel ExecuteSave<TOutModel>(Func<IUnitOfWorkWritable, IEntityIdentifier> saveFunc, Func<IUnitOfWork, IEntityIdentifier, TOutModel> loadFunc)
        {
            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                var savedResult = saveFunc(unitOfWork);
                unitOfWork.Save(parentEntity: savedResult);
                return savedResult;
            });
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(unitOfWork, entity));
        }

        protected TOutModel ExecuteSaveAndValidate<TOutModel>(VmEntityHeaderBase inputModel,
            Func<IUnitOfWorkWritable, IEntityIdentifier> saveFunc,
            Func<IUnitOfWork, IEntityIdentifier, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
//            lockEntity();
            if (inputModel != null && inputModel.Id.IsAssigned())
            {
                Utilities.LockEntityVersioned<TVersioned, TRoot>(inputModel.Id ?? Guid.Empty);
            }
            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                var savedResult = saveFunc(unitOfWork);
                unitOfWork.Save();
                return savedResult;
            });
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var model = loadFunc(unitOfWork, entity);
                var validated = AddValidation(model, unitOfWork);
                validated.Action = inputModel?.Action ?? ActionTypeEnum.SaveAndPublish;
                return validated;
            });
        }

        protected TOutModel ExecuteValidate<TOutModel>(Action lockEntity, Func<IUnitOfWork, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            lockEntity();
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var model = loadFunc(unitOfWork);

                return AddValidation(model, unitOfWork);
            });
        }

        private TOutModel AddValidation<TOutModel>(TOutModel model, IUnitOfWork unitOfWork) where TOutModel : VmEntityHeaderBase
        {
            return CommonService.GetValidatedHeader(model,
                ValidationManager.CheckEntity<TVersioned>(model.Id.Value, unitOfWork)) as TOutModel;
        }


        protected TOutModel ExecuteDelete<TOutModel>(Guid id, Func<Guid?, IUnitOfWork, TOutModel> loadFunc, Action<IUnitOfWorkWritable> onDeletingAction = null)
        {
            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                onDeletingAction?.Invoke(unitOfWork);
                var deletedEntity = CommonService.ChangeEntityToDeleted<TVersioned, TLanguageAvail>(unitOfWork, id);;
                unitOfWork.Save();
                return deletedEntity;

            });
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(entity.Id, unitOfWork));
        }
        
        protected TOutModel ExecuteWithdraw<TOutModel>(Guid id, Func<Guid?, IUnitOfWork, TOutModel> loadFunc, Action onWithdrawingAction = null)
        {
            onWithdrawingAction?.Invoke();
            var result = CommonService.WithdrawEntity<TVersioned, TLanguageAvail>(id);
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(result.Id, unitOfWork));
        }

        protected TOutModel ExecuteRestore<TOutModel>(Guid id, Func<Guid?, IUnitOfWork, TOutModel> loadFunc)
        {
            var result = CommonService.RestoreEntity<TVersioned, TLanguageAvail>(id);
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(result.Id, unitOfWork));
        }

        protected TOutModel ExecuteArchiveLanguage<TOutModel>(VmEntityBasic model, Func<Guid?, IUnitOfWork, TOutModel> loadFunc)
        {
            var entity = CommonService.ArchiveLanguage<TVersioned, TLanguageAvail>(model);
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(entity.Id, unitOfWork));
        }

        protected TOutModel ExecuteRestoreLanguage<TOutModel>(VmEntityBasic model, Func<Guid?, IUnitOfWork, TOutModel> loadFunc)
        {
            var entity = CommonService.RestoreLanguage<TVersioned, TLanguageAvail>(model);
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(entity.Id, unitOfWork));
        }

        protected TOutModel ExecuteWithdrawLanguage<TOutModel>(VmEntityBasic model, Func<Guid?, IUnitOfWork, TOutModel> loadFunc)
        {
            var entity = CommonService.WithdrawLanguage<TVersioned, TLanguageAvail>(model);
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(entity.Id, unitOfWork));
        }
    }
}