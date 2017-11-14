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
        private IContextManager contextManager;
        private readonly ITranslationEntity translationManager;

        public FormStateService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {

            this.translationManager = translationManagerToVm;
            this.contextManager = contextManager;
        }
        public void Delete(string id)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var formStateToDelete = formStateRepository
                    .All()
                    .Where(formState => formState.Id == new Guid(id))
                    .FirstOrDefault();
                formStateRepository.Remove(formStateToDelete);
                unitOfWork.Save();
            });
        }

        public VmFormState Search(VmFormStateSearch search)
        {

            VmFormState result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var _formState = formStateRepository
                    .All()
                    .Where(formState =>
                        formState.EntityId == search.EntityId &&
                        formState.EntityType == search.EntityType &&
                        formState.LanguageId == search.LanguageId &&
                        formState.UserName == search.UserName
                    )
                    .FirstOrDefault();
                result = TranslationManagerToVm.Translate<FormState, VmFormState>(_formState);
            });
            return result ?? new VmFormState() { Exists = false };
        }

        public string Exists(VmFormStateSearch search)
        {
            string result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                result = formStateRepository
                    .All()
                    .Where(formState =>
                        formState.EntityId == search.EntityId &&
                        formState.EntityType == search.EntityType &&
                        formState.LanguageId == search.LanguageId &&
                        formState.UserName == search.UserName
                    )
                    .Select(formState => formState.Id.ToString())
                    .FirstOrDefault();
            });
            return result;
        }

        public VmFormState GetById(string id)
        {

            VmFormState result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var _formState = formStateRepository
                    .All()
                    .Where(formState => formState.Id == new Guid(id)).FirstOrDefault();
                result = TranslationManagerToVm.Translate<FormState, VmFormState>(_formState);
            });
            return result;
        }

        public VmFormState Save(VmFormState formState)
        {
            VmFormState result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var formStateRepository = unitOfWork.CreateRepository<IFormStateRepository>();
                var isExistingFormState = formState.Id != null;
                if (isExistingFormState)
                {
                    var existingFormState = formStateRepository.All()
                        .Where(x => x.Id == new Guid(formState.Id))
                        .FirstOrDefault();
                    existingFormState.State = formState.State;
                    unitOfWork.Save();
                    result = TranslationManagerToVm.Translate<FormState, VmFormState>(existingFormState);
                }
                else
                {
                    var formStateToSave = TranslationManagerToEntity.Translate<VmFormState, FormState>(formState, unitOfWork);
                    formStateRepository.Add(formStateToSave);
                    unitOfWork.Save();
                    result = TranslationManagerToVm.Translate<FormState, VmFormState>(formStateToSave);
                }
            });
            return result;
        }
    }
}
