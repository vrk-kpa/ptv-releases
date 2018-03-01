using System;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Caches;
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
            IUserOrganizationChecker userOrganizationChecker)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
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
        
        public void Delete(Guid entityId, string userName)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var formState = formStateRepository
                    .All()
                    .FirstOrDefault(
                        fs => fs.EntityId == entityId &&
                        fs.UserName == userName
                     );
                if (formState != null)
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
                    .All().FirstOrDefault(fs => fs.EntityId == search.EntityId &&
                                          fs.EntityType == search.EntityType &&
                                          fs.LanguageId == search.LanguageId &&
                                          fs.UserName == search.UserName);
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
