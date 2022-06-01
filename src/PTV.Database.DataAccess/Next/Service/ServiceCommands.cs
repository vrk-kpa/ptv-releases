using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Next.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using PTV.Framework.Exceptions;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Service
{
    [RegisterService(typeof(IServiceCommands), RegisterType.Transient)]
    internal class ServiceCommands : IServiceCommands
    {
        private readonly IServiceService service;
        private readonly ITypesCache typesCache;
        private readonly ITargetGroupDataCache targetGroupCache;
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly IServiceQueries serviceQueries;

        public ServiceCommands(
            IServiceService service,
            ITypesCache typesCache,
            ITargetGroupDataCache targetGroupCache,
            IPahaTokenAccessor pahaTokenAccessor,
            IServiceQueries serviceQueries)
        {
            this.service = service;
            this.typesCache = typesCache;
            this.targetGroupCache = targetGroupCache;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.serviceQueries = serviceQueries;
        }

        public Guid? Save(ServiceModel model)
        {
            // TODO: change the save action based on input model

            var oldVm = model.ToOldViewModel(ActionTypeEnum.Save, typesCache, targetGroupCache,
                pahaTokenAccessor.ActiveOrganizationId);
            return service.SaveServiceSimple(oldVm);
        }

        public Guid? Publish(Guid id, Dictionary<LanguageEnum, PublishingModel> model)
        {
            ValidatePublishLanguageVersionStatuses(id, model);
            var userName = pahaTokenAccessor.UserName;
            var oldVm = model.ToOldViewModel(id, userName, typesCache);

            if (model.Values.All(x => x.PublishAt.HasValue && x.PublishAt >= DateTime.UtcNow.Date))
            {
                return service.ScheduleService(oldVm).Id;
            }

            return service.PublishService(oldVm).Id;
        }


        public Guid? Archive(Guid id, LanguageEnum? language)
        {
            if (language.HasValue)
            {
                var languageVersionPublishingStatus =
                    serviceQueries.GetLanguageVersionPublishingStatus(id, language.Value);
                if (!languageVersionPublishingStatus.HasValue ||
                    !ServiceStatusValidation.CanArchiveLanguageVersion(
                        languageVersionPublishingStatus.GetValueOrDefault()))
                {
                    throw new OperationForbiddenException(
                        $"Language version publishing status {languageVersionPublishingStatus.GetValueOrDefault()} is not allowed to be archived ");
                }

                return service.ArchiveLanguageSimple(new VmServiceBasic
                {
                    Id = id,
                    LanguageId = typesCache.Get<Language>(language.Value.ToString())
                });
            }

            return service.DeleteServiceSimple(id);
        }

        public Guid? Withdraw(Guid id, LanguageEnum? language)
        {
            if (language.HasValue)
            {
                return service.WithdrawLanguageSimple(new VmServiceBasic
                {
                    Id = id,
                    LanguageId = typesCache.Get<Language>(language.Value.ToString())
                });
            }

            return service.WithdrawServiceSimple(id);
        }

        public Guid? Restore(Guid id, LanguageEnum? language)
        {
            if (language.HasValue)
            {
                var languageVersionPublishingStatus =
                    serviceQueries.GetLanguageVersionPublishingStatus(id, language.Value);
                if (!languageVersionPublishingStatus.HasValue ||
                    ServiceStatusValidation.CanRestoreLanguageVersion(
                        languageVersionPublishingStatus.Value))
                {
                    throw new OperationForbiddenException(
                        "Restore is only allowed for language versions with Draft publishing status.");
                }

                return service.RestoreLanguageSimple(new VmServiceBasic
                {
                    Id = id,
                    LanguageId = typesCache.Get<Language>(language.Value.ToString())
                });
            }

            return service.RestoreServiceSimple(id);
        }

        public Guid? Remove(Guid id)
        {
            var servicePublishingStatus =
                serviceQueries.GetServicePublishingStatus(id);
            if (!servicePublishingStatus.HasValue ||
                ServiceStatusValidation.CanRemoveService(
                    servicePublishingStatus.Value))
            {
                throw new OperationForbiddenException(
                    "Remove is only allowed for service with Archived or Modified publishing status.");
            }

            return service.RemoveServiceSimple(id);
        }

        private void ValidatePublishLanguageVersionStatuses(Guid id, Dictionary<LanguageEnum, PublishingModel> model)
        {
            foreach (var languageVersion in model)
            {
                var languageVersionPublishingStatus =
                    serviceQueries.GetLanguageVersionPublishingStatus(id, languageVersion.Key);
                if (!languageVersionPublishingStatus.HasValue ||
                    !ServiceStatusValidation.CanPublishLanguageVersion(languageVersionPublishingStatus.Value))
                {
                    throw new OperationForbiddenException(
                        "Publishing is not allowed for language versions with publishing statuses that are not allowed to be published.");
                }
            }
        }
    }
}