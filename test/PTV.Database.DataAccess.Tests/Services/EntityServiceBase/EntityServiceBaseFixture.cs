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
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Tests.Services.EntityServiceBase
{
    internal class EntityServiceBaseFixture : EntityServiceBase<TestEntityVersioned, TestEntityRoot, TestLanguageAvail>
    {
        public EntityServiceBaseFixture(ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity, IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker, IContextManager contextManager,
            IServiceUtilities utilities, ICommonServiceInternal commonService, IValidationManager validationManager,
            IVersioningManager versioningManager)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                contextManager, utilities, commonService, validationManager, versioningManager)
        {

        }

        public TOutModel CallExecuteGet<TOutModel>(IVmEntityGet entityModel, Func<IUnitOfWork, IVmEntityGet, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            return ExecuteGet(entityModel, loadFunc);
        }     
        
        public TOutModel CallExecuteSave<TOutModel>
        (
            VmEntityHeaderBase inputModel,
            Func<IUnitOfWorkWritable, IEntityIdentifier> saveFunc,
            Func<IUnitOfWork, IVmEntityGet, TOutModel> loadFunc
        ) where TOutModel : VmEntityHeaderBase
        {
            return ExecuteSave(inputModel, saveFunc, loadFunc);
        }

        public TOutModel CallExecuteScheduleEntity<TOutModel>(IVmLocalizedEntityModel inputModel,
            Func<IUnitOfWork, IVmEntityGet, TOutModel> loadFunc) where TOutModel : VmEntityHeaderBase
        {
            return ExecuteScheduleEntity(inputModel, loadFunc);
        }

        public TOutModel CallExecuteValidate<TOutModel>(Action lockEntity, Func<IUnitOfWork, TOutModel> loadFunc)
            where TOutModel : VmEntityHeaderBase
        {
            return ExecuteValidate(lockEntity, loadFunc);
        }
    }
}