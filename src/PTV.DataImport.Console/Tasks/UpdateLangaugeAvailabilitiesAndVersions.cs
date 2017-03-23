using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class UpdateLangaugeAvailabilitiesAndVersions
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        public UpdateLangaugeAvailabilitiesAndVersions(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateMunicipalityOrganizationsTask>();

            _logger.LogDebug("UpdateLangaugeAvailabilitiesAndVersions .ctor");
        }

        private void AddAvailability<TEntity, TLocalized, TNameProp>(IUnitOfWorkWritable unitOfWork, Expression<Func<TEntity, IEnumerable<TNameProp>>> nameSelector)
            where TEntity : class, IMultilanguagedEntity<TLocalized> where TNameProp : ILocalizable where TLocalized : ILanguageAvailability, new()
        {
            var publishingStatusPublishedId =
                unitOfWork.CreateRepository<IPublishingStatusTypeRepository>().All().Where(i => i.Code == PublishingStatus.Published.ToString()).Select(i => i.Id).First();

            var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            entityRep.All().Where(i => !i.LanguageAvailabilities.Any()).Include(nameSelector).ToList().ForEach(entity =>
            {
                var localizations = nameSelector.Compile()(entity).Select(i => i.LocalizationId).Distinct().ToList();
                foreach (var localization in localizations)
                {
                    entity.LanguageAvailabilities.Add(new TLocalized()
                    {
                        LanguageId = localization,
                        StatusId = publishingStatusPublishedId
                    });
                }
            });
        }

        private void AddVersions<TEntity>(IUnitOfWorkWritable unitOfWork) where TEntity : class, IVersionedVolume
        {
            var publishingStatusData = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>().All().ToList();
            var publishingStatusPublishedId = publishingStatusData.Where(i => i.Code == PublishingStatus.Published.ToString()).Select(i => i.Id).First();
            var publishingStatusDraftId = publishingStatusData.Where(i => i.Code == PublishingStatus.Draft.ToString()).Select(i => i.Id).First();
            var publishingStatusDeletedId = publishingStatusData.Where(i => i.Code == PublishingStatus.Deleted.ToString()).Select(i => i.Id).First();

            var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            entityRep.All().Where(i => i.VersioningId == null && ((i.PublishingStatusId == publishingStatusPublishedId) || (i.PublishingStatusId == publishingStatusDraftId) || (i.PublishingStatusId == publishingStatusDeletedId) || (i.PublishingStatusId == null))).ToList().ForEach(entity =>
            {
                if (entity.PublishingStatusId == null)
                {
                    entity.PublishingStatusId = publishingStatusPublishedId;
                }
                entity.Versioning = new Versioning()
                {
                    Id = Guid.NewGuid(),
                    VersionMajor = entity.PublishingStatusId == publishingStatusDraftId ? 0 : 1,
                    VersionMinor = entity.PublishingStatusId == publishingStatusDraftId ? 1 : 0,
                };
            });
        }

        public void CheckAndUpdateLangaugeAvailabilitiesAndVersions()
        {
            // Language availabilities
            Console.WriteLine("Updating language availabilities...");
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    AddAvailability<ServiceVersioned, ServiceLanguageAvailability, ServiceName>(unitOfWork, i => i.ServiceNames);
                    AddAvailability<OrganizationVersioned, OrganizationLanguageAvailability, OrganizationName>(unitOfWork, i => i.OrganizationNames);
                    AddAvailability<ServiceChannelVersioned, ServiceChannelLanguageAvailability, ServiceChannelName>(unitOfWork, i => i.ServiceChannelNames);
                    AddAvailability<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability, StatutoryServiceName>(unitOfWork, i => i.Names);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
            Console.WriteLine("Done.");
            Console.WriteLine("Updating versioning info...");
            // Entities versions
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    AddVersions<ServiceVersioned>(unitOfWork);
                    AddVersions<OrganizationVersioned>(unitOfWork);
                    AddVersions<ServiceChannelVersioned>(unitOfWork);
                    AddVersions<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
            Console.WriteLine("Done.");
        }

    }
}
