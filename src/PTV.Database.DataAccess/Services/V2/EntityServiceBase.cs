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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.V2
{
    internal abstract class EntityServiceBase<TVersioned, TRoot> : ServiceBase where TRoot : IVersionedRoot where TVersioned : class, IEntityIdentifier, IVersionedVolume<TRoot>
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

        public TOutModel ExecuteSave<TOutModel>(Func<IUnitOfWorkWritable, IEntityIdentifier> saveFunc, Func<IUnitOfWork, IEntityIdentifier, TOutModel> loadFunc)
        {
            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                var savedResult = saveFunc(unitOfWork);
                unitOfWork.Save(parentEntity: savedResult);
                return savedResult;
            });
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(unitOfWork, entity));
        }

        public TOutModel ExecuteSaveAndValidate<TOutModel>(Guid? id, Func<IUnitOfWorkWritable, IEntityIdentifier> saveFunc, Func<IUnitOfWork, IEntityIdentifier, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
//            lockEntity();
            if (id.IsAssigned())
            {
                Utilities.LockEntityVersioned<TVersioned, TRoot>(id ?? Guid.Empty);
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
                var validated = CommonService.GetValidatedHeader(model,
                                    ValidationManager.CheckEntity<TVersioned>(model.Id.Value, unitOfWork)) as TOutModel;
                validated.Action = ActionTypeEnum.SaveAndValidate;
                return validated;
            });
        }

        public TOutModel ExecuteValidate<TOutModel>(Action lockEntity, Func<IUnitOfWork, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            lockEntity();
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var model = loadFunc(unitOfWork);

                return CommonService.GetValidatedHeader(model,
                    ValidationManager.CheckEntity<TVersioned>(model.Id.Value, unitOfWork)) as TOutModel;
            });
        }
    }
}