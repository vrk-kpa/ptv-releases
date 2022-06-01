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
using System.Collections.Generic;
using System.Linq;
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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Exceptions.DataAccess;

namespace PTV.Database.DataAccess.Services.V2
{
    internal abstract class EntityServiceBase<TVersioned, TRoot, TLanguageAvail> : ServiceBase
        where TRoot : IVersionedRoot
        where TVersioned : class, IEntityIdentifier, IVersionedVolume<TRoot>, IMultilanguagedEntity<TLanguageAvail>, IValidity, IAuditing, new()
        where TLanguageAvail : class, ILanguageAvailability
    {
        protected IContextManager ContextManager { get; }
        protected IServiceUtilities Utilities { get; }

        protected ICommonServiceInternal CommonService { get; }
        protected IValidationManager ValidationManager { get; }

        internal EntityServiceBase(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IContextManager contextManager,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IValidationManager validationManager,
            IVersioningManager versioningManager) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            ContextManager = contextManager;
            Utilities = utilities;
            CommonService = commonService;
            ValidationManager = validationManager;
        }

        protected TOutModel ExecuteGet<TOutModel>(IVmEntityGet getModel, Func<IUnitOfWork, IVmEntityGet, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            if (getModel == null || !getModel.Id.IsAssigned())
            {
                throw CreateEntityNotFound<TVersioned>(null);
            }
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var model = loadFunc(unitOfWork, getModel);
                bool timedPublishSet = model.LanguagesAvailabilities.Any(x => x.ValidFrom.HasValue);
                if (getModel.IncludeValidation || timedPublishSet)
                {
                    model = AddValidation(model, unitOfWork, timedPublishSet && !getModel.IncludeValidation);
                }
                return model;
            });
        }
        
        protected Guid? ExecuteSaveSimple(VmEntityHeaderBase inputModel,
            Func<IUnitOfWorkWritable, IEntityIdentifier> saveFunc,
            IEnumerable<Action<IUnitOfWorkWritable>> preSaveActions = null,
            IEnumerable<Action<IUnitOfWorkWritable, IEntityIdentifier>> postSaveActions = null)
        {
            if (inputModel != null && inputModel.Id.IsAssigned() && inputModel.Action == ActionTypeEnum.SaveAndPublish)
            {
                Utilities.LockEntityVersioned<TVersioned, TRoot>(inputModel.Id ?? Guid.Empty);
            }

            if (preSaveActions != null)
            {
                foreach (var preSaveAction in preSaveActions)
                {
                    ContextManager.ExecuteWriter(unitOfWork =>
                    {
                        preSaveAction(unitOfWork);
                        unitOfWork.Save();
                    });
                }
            }

            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                var savedResult = saveFunc(unitOfWork);
                unitOfWork.Save();
                return savedResult;
            });

            if (postSaveActions != null)
            {
                foreach (var postSaveAction in postSaveActions)
                {
                    ContextManager.ExecuteWriter(unitOfWork =>
                    {
                        postSaveAction(unitOfWork, entity);
                        unitOfWork.Save();
                    });
                }
            }
            
            return entity.Id;
        }

        protected TOutModel ExecuteSave<TOutModel>(VmEntityHeaderBase inputModel,
            Func<IUnitOfWorkWritable, IEntityIdentifier> saveFunc,
            Func<IUnitOfWork, IVmEntityGet, TOutModel> loadFunc,
            IEnumerable<Action<IUnitOfWorkWritable>> preSaveActions = null,
            IEnumerable<Action<IUnitOfWorkWritable, IEntityIdentifier>> postSaveActions = null) where TOutModel : VmEntityHeaderBase
        {
            if (inputModel != null && inputModel.Id.IsAssigned() && inputModel.Action == ActionTypeEnum.SaveAndPublish)
            {
                Utilities.LockEntityVersioned<TVersioned, TRoot>(inputModel.Id ?? Guid.Empty);
            }

            if (preSaveActions != null)
            {
                foreach (var preSaveAction in preSaveActions)
                {
                    ContextManager.ExecuteWriter(unitOfWork =>
                    {
                        preSaveAction(unitOfWork);
                        unitOfWork.Save();
                    });
                }
            }

            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                var savedResult = saveFunc(unitOfWork);
                unitOfWork.Save();
                return savedResult;
            });

            if (postSaveActions != null)
            {
                foreach (var postSaveAction in postSaveActions)
                {
                    ContextManager.ExecuteWriter(unitOfWork =>
                    {
                        postSaveAction(unitOfWork, entity);
                        unitOfWork.Save();
                    });
                }
            }
            
            var result = ExecuteGet(new VmEntityBasic
            {
                Id = entity.Id,
                IncludeValidation = inputModel?.Action != ActionTypeEnum.Save
            }, loadFunc);
            result.Action = inputModel?.Action ?? ActionTypeEnum.Save;
            result.AlternativeId = inputModel?.AlternativeId;
            return result;
        }

        protected TOutModel ExecuteScheduleEntity<TOutModel>(IVmLocalizedEntityModel inputModel, Func<IUnitOfWork, IVmEntityGet, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            if (!inputModel.Id.IsAssigned()) return default(TOutModel);
            Guid id = inputModel.Id;
            var result = ContextManager.ExecuteWriter(unitOfWork =>
            {
                //Validate mandatory values
                if (inputModel.PublishAction == PublishActionTypeEnum.SchedulePublish)
                {
                    var validationMessages = ValidationManager.CheckEntity<TVersioned>(id, unitOfWork, inputModel);
                    if (validationMessages.Any())
                    {
                        throw new SchedulePublishException();
                    }
                }

                //Schedule publish/archive
                return CommonService.SchedulePublishArchiveEntity<TVersioned, TLanguageAvail>(unitOfWork, inputModel);
            });
            return ExecuteGet(new VmEntityBasic
            {
                Id = result.Id
            }, loadFunc);
        }

        protected TOutModel ExecuteValidate<TOutModel>(Action lockEntity, Func<IUnitOfWork, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            lockEntity?.Invoke();
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var model = loadFunc(unitOfWork);

                return AddValidation(model, unitOfWork, false);
            });
        }

        private TOutModel AddValidation<TOutModel>(TOutModel model, IUnitOfWork unitOfWork, bool checkTimedPublish) where TOutModel : VmEntityHeaderBase
        {
            var validationMessages = ValidationManager.CheckEntity<TVersioned>(model.Id.Value, unitOfWork);
            if (checkTimedPublish && model.LanguagesAvailabilities.All(x => !x.ValidFrom.HasValue || !validationMessages.ContainsKey(x.LanguageId)))
            {
                return model;
            }
            return CommonService.GetValidatedHeader(model, validationMessages) as TOutModel;
        }


        protected TOutModel ExecuteDelete<TOutModel>(Guid id, Func<Guid?, IUnitOfWork, TOutModel> loadFunc, Action<IUnitOfWorkWritable> onDeletingAction = null)
        {
            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                onDeletingAction?.Invoke(unitOfWork);
                var deletedEntity = CommonService.ChangeEntityToDeleted<TVersioned, TLanguageAvail>(unitOfWork, id, HistoryAction.Delete);
                unitOfWork.Save();
                return deletedEntity;

            });
            return ContextManager.ExecuteReader(unitOfWork => loadFunc(entity.Id, unitOfWork));
        }

        protected TOutModel ExecuteRemove<TOutModel>(Guid id, Func<Guid?, IUnitOfWork, TOutModel> loadFunc)
        {
            var entity = ContextManager.ExecuteWriter(unitOfWork =>
            {
                var publishedEntity = CommonService.ChangeEntityToRemoved<TVersioned, TLanguageAvail>(unitOfWork, id);;
                unitOfWork.Save();
                return publishedEntity;

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
