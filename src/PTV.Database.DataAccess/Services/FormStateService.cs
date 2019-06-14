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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IFormStateService), RegisterType.Transient)]
    internal class FormStateService : ServiceBase, IFormStateService
    {
        private readonly IContextManager contextManager;

        public FormStateService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versioningManager)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.contextManager = contextManager;
        }
        public void Delete(Guid id)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                formStateRepository.BatchDelete(i => i.Id, id);
                unitOfWork.Save();
            });
        }
        
        public void Delete(Guid? entityId, string userName)
        {
            if (!entityId.HasValue) return;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                IEnumerable<FormState> formState;
                
                if (string.IsNullOrEmpty(userName))
                {
                    formState = formStateRepository
                        .All()
                        .Where(fs => fs.EntityId == entityId);
                }
                else
                {
                    formState = formStateRepository
                        .All()
                        .Where(
                            fs => fs.EntityId == entityId && fs.UserName == userName
                        );
                }
                
                if (formState.Any())
                {
                    formStateRepository.Remove(formState);
                    unitOfWork.Save();
                }
            });
        }

        public VmFormState Search(VmFormStateSearch search)
        {
            VmFormState defaultState = new VmFormState() { Exists = false };
            return (search == null) ? defaultState : contextManager.ExecuteReader(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var formState = formStateRepository
                    .All().Where(fs => fs.EntityId == search.EntityId &&
                                          fs.EntityType == search.EntityType &&
                                          fs.UserName == search.UserName)
                        .OrderByDescending(x => x.Modified)
                        .FirstOrDefault();
                return TranslationManagerToVm.Translate<FormState, VmFormState>(formState) ?? defaultState;
            });
        }

        public VmFormState GetById(Guid id)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var formState = formStateRepository
                    .All().FirstOrDefault(i => i.Id == id);
                return TranslationManagerToVm.Translate<FormState, VmFormState>(formState);
            });
        }

        public VmFormState Save(VmFormState formState)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var formStateId = formState.Id;
                FormState formStateToSave = null;
                if (formStateId.IsAssigned())
                {
                    formStateToSave = formStateRepository
                        .All()
                        .FirstOrDefault(x => x.Id == formStateId);
                }
                if (formStateToSave != null)
                {
                    formStateToSave.State = formState.State;
                }
                else
                {
                    formStateToSave = TranslationManagerToEntity.Translate<VmFormState, FormState>(formState, unitOfWork);
                    if (formStateId.IsAssigned())
                    {
                        formStateToSave.Id = formStateId;
                    }
                    formStateRepository.Add(formStateToSave);
                }
                unitOfWork.Save();
                return TranslationManagerToVm.Translate<FormState, VmFormState>(formStateToSave);
            });
        }
    }
}
