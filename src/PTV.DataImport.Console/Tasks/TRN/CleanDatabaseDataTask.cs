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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.DataImport.Console.Tasks.TRN
{
    public class CleanDatabaseDataTask
    {
        private readonly IServiceProvider serviceProvider;

        public CleanDatabaseDataTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<UpdateServiceDescriptionByGeneralDescriptionTask>();
            logger.LogDebug("UpdateBackgroundGeneralDescriptionTask .ctor");
        }

        public void AnonymisationDatabaseEmails()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var serviceVersionedRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var serviceChannelVersionedRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var generalDescriptionVersionedRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    var serviceServiceChannelRepository = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();

                    const string testEmail = "test.person@test.com";

                    foreach (var entity in serviceVersionedRepository.All().ToList())
                    {
                        SetModifiedAndCreatedBy(entity, testEmail);
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    foreach (var entity in serviceChannelVersionedRepository.All().ToList())
                    {
                        entity.ModifiedBy = testEmail;
                        entity.CreatedBy = testEmail;
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    foreach (var entity in organizationServiceRepository.All().ToList())
                    {
                        entity.ModifiedBy = testEmail;
                        entity.CreatedBy = testEmail;
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    foreach (var entity in generalDescriptionVersionedRepository.All().ToList())
                    {
                        entity.ModifiedBy = testEmail;
                        entity.CreatedBy = testEmail;
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    foreach (var entity in serviceServiceChannelRepository.All().ToList())
                    {
                        entity.ModifiedBy = testEmail;
                        entity.CreatedBy = testEmail;
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    //foreach (var entity in serviceCollectionVersionedRepository.All().ToList())
                    //{
                    //    entity.ModifiedBy = testEmail;
                    //    entity.CreatedBy = testEmail;
                    //}
                    //unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                });
            }
        }

        private static void SetModifiedAndCreatedBy<T>(T entity, string testEmail) where T : IAuditing
        {
            entity.ModifiedBy = testEmail;
            entity.CreatedBy = testEmail;
        }

        public void CleanDatabaseData()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    System.Console.WriteLine("Clean database data");

                    var serviceVersionedRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var serviceChannelVersionedRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

                    var allowOrganizationRootIds = new List<Guid>
                    {
                        new Guid("b458d033-042d-4cbe-b030-34e81da28821"),
                        new Guid("df499a95-3f53-4a4c-b794-015b25710ee8"),
                        new Guid("746538f1-6ddc-4042-bd7b-923d6401ecae"),
                        new Guid("c60381d6-fbd7-45d7-994e-c99ec0fc8f3f"),
                        new Guid("011154df-3726-461e-ae9c-a1182d1746de"),
                        new Guid("3d1759a2-e47a-4947-9a31-cab1c1e2512b"),
                        new Guid("6745e341-be2a-45a4-b184-bbc2f8465615"),
                        new Guid("92374b0f-7d3c-4017-858e-666ee3ca2761"),
                        new Guid("7fdd7f84-e52a-4c17-a59a-d7c2a3095ed5"),
                        new Guid("52e0f6dc-ec1f-48d5-a0a2-7a4d8b657d53"),
                        new Guid("0d34eb3a-d7e5-4af0-aa0d-009b5fb0e91c"),
                        new Guid("e9d022bc-97b8-41c6-b953-d052ad53bc91"),
                        new Guid("3e8356bd-377f-4cad-97b1-a027bd4bbf25"),
                        new Guid("4bc4fad0-84fe-412f-8fb9-c431f4ba48b2"),
                        new Guid("ae788356-6950-48fc-b3ff-63243f74fe53"),
                        new Guid("c225a17a-b767-4148-80ae-b78506275534"),
                        new Guid("269685e8-9c00-4f2e-b6c7-e61d94e13b96")
                    };


                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    var publishingStatusData = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>().All().ToList();
                    var publishingStatusPublishedId = publishingStatusData.Where(i => i.Code == PublishingStatus.Published.ToString()).Select(i => i.Id).First();

                    //organization
                    var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var allOrganization = organizationRep.All().Include(j => j.OrganizationWebAddress).ToList();
                    var allowOrganizations = allOrganization.Where(x => allowOrganizationRootIds.Contains(x.UnificRootId)).ToList();
                    var removeOrganizations = allOrganization.Where(x => !allowOrganizationRootIds.Contains(x.UnificRootId)).ToList();
                    var allowOrganizationWebPageIds = new List<Guid>();
                    var removeOrganizationWebPageIds = new List<Guid>();

                    //services
                    var allowServiceIds = new List<Guid>();
                    var allowServiceWebPageIds = new List<Guid>();

                    //channels
                    var allowChannelIds = new List<Guid>();
                    var allowChannelWebPageIds = new List<Guid>();

                    //versioning
                    var allowVersioningIds = new List<Guid>();

                    allowOrganizationWebPageIds.AddRange(allowOrganizations.SelectMany(x => x.OrganizationWebAddress.Select(y => y.WebPageId)));
                    removeOrganizationWebPageIds.AddRange(removeOrganizations.SelectMany(x => x.OrganizationWebAddress.Select(y => y.WebPageId)));

                    //TO ALLOW
                    foreach (var organization in allowOrganizations)
                    {
                        if (organization != null)
                        {
                            var services = serviceVersionedRepository.All()
                                .Include(j => j.ServiceWebPages)
                                .Where(x => x.OrganizationId == organization.UnificRootId &&
                                            x.PublishingStatusId == publishingStatusPublishedId).ToList();
                            allowServiceIds.AddRange(services.Select(x => x.Id));
                            allowServiceWebPageIds.AddRange(services.SelectMany(x => x.ServiceWebPages.Select(y => y.WebPageId)));
                            allowVersioningIds.AddRange(services.Where(x => x.VersioningId != null).Select(x => x.VersioningId.Value));

                            var channels = serviceChannelVersionedRepository.All()
                                .Include(j => j.WebPages)
                                .Where(x => x.OrganizationId == organization.UnificRootId &&
                                            x.PublishingStatusId == publishingStatusPublishedId).ToList();
                            allowChannelIds.AddRange(channels.Select(x => x.Id));
                            allowChannelWebPageIds.AddRange(channels.SelectMany(x => x.WebPages.Select(y => y.WebPageId)));
                            allowVersioningIds.AddRange(channels.Where(x => x.VersioningId != null).Select(x => x.VersioningId.Value));
                        }
                    }

                    //TO REMOVE
                    //services
                    //var removeServices = serviceVersionedRepository.All()
                    //                    //.Include(j => j.ServiceWebPages)
                    //                    .ToList();

                    //removeServiceIds = removeServices
                    //                    .Select(x => x.Id)
                    //                    .ToList();

                    //removeServiceWebPageIds.AddRange(removeServices.SelectMany(x => x.ServiceWebPages.Select(y => y.WebPageId)));

                    //channels
                    //var removeChannels = serviceChannelVersionedRepository.All()
                    //                     //.Include(j => j.WebPages)
                    //                     .ToList();

                    //removeChannelIds = removeChannels
                    //                    .Select(x => x.Id)
                    //                    .ToList();

                    //removeChannelWebPageIds.AddRange(removeChannels.SelectMany(x => x.WebPages.Select(y => y.WebPageId)));

                    allowVersioningIds.AddRange(allowOrganizations.Where(x => x.VersioningId != null).Select(x => x.VersioningId.Value));

                    //1. Versioning
                    //update
                    //var versioningRepository = unitOfWork.CreateRepository<IVersioningRepository>();
                    //var versionings = versioningRepository.All().Where(x => allowVersioningIds.Contains(x.Id)).ToList();
                    //foreach (var versioning in versionings)
                    //{
                    //    versioning.VersionMajor = 1;
                    //    versioning.VersionMinor = 0;
                    //    versioning.PreviousVersionId = null;
                    //}
                    //unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    //2. Webpages
                    //2.1 DELETE ServiceWebPages
                    //RemoveData(databaseRawContext, "ServiceWebPage", "ServiceVersionedId", allRemoveServiceIds);
                    //RemoveData(databaseRawContext, "WebPage", "Id", removeServiceWebPageIds.Except(allowServiceWebPageIds).Distinct().ToList());
                    //2.2 DELETE ServiceChannelWebpages
                    //RemoveData(databaseRawContext, "ServiceChannelWebPage","ServiceChannelVersionedId", allRemoveChannelIds);
                    //RemoveData(databaseRawContext, "WebPage", "Id", removeChannelWebPageIds.Except(allowChannelWebPageIds).Distinct().ToList());
                    //2.3 DELETE OrganizationWebPage
                    //RemoveData(databaseRawContext, "OrganizationWebPage", "OrganizationVersionedId", allRemoveOrganizationIds);
                    //RemoveData(databaseRawContext, "WebPage", "Id", removeOrganizationWebPageIds.Except(allowOrganizationWebPageIds).Distinct().ToList());

                    //3. Services
                    //RemoveData(databaseRawContext, "ServiceVersioned","Id", allRemoveServiceIds.Take(50000).ToList());

                    //4. Channels
                    //RemoveData(databaseRawContext, "ServiceChannelVersioned", "Id", allRemoveChannelIds.Take(70000).ToList());

                    //5. Organization
                    //RemoveData(databaseRawContext, "OrganizationVersioned", "Id", allRemoveOrganizationIds);


                    ////5. Versioning delete
                    //var serviceCollectionVersionedRepository = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
                    //var statutoryServiceGeneralDescriptionVersionedRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    //var versioningRepository = unitOfWork.CreateRepository<IVersioningRepository>();

                    //var serviceCollectionVersionsIds = serviceCollectionVersionedRepository.All().Where(x => x.VersioningId != null).Select(x => x.VersioningId.Value).ToList();
                    //allowVersioningIds.AddRange(serviceCollectionVersionsIds);
                    //var statutoryServiceGeneralDescriptionVersionsIds = statutoryServiceGeneralDescriptionVersionedRepository.All().Where(x => x.VersioningId != null).Select(x => x.VersioningId.Value).ToList();
                    //allowVersioningIds.AddRange(statutoryServiceGeneralDescriptionVersionsIds);

                    //Zero step
                    //var versionings = versioningRepository.All().Where(x => serviceCollectionVersionsIds.Union(statutoryServiceGeneralDescriptionVersionsIds).Contains(x.Id)).ToList();
                    //foreach (var versioning in versionings)
                    //{
                    //    versioning.PreviousVersionId = null;
                    //}
                    //unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    //var allVersioningIds = versioningRepository.All().Select(x => x.Id).ToList();
                    //var allRemoveVersioningIds = allVersioningIds.Except(allowVersioningIds).Distinct().ToList();

                    //First step
                    //var versionings = versioningRepository.All().Where(x => allRemoveVersioningIds.Contains(x.Id)).ToList();
                    //foreach (var versioning in versionings)
                    //{
                    //    versioning.PreviousVersionId = null;
                    //}
                    //unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                    //Second step
                    //RemoveData(databaseRawContext, "Versioning","Id", allRemoveVersioningIds.Take(150000).ToList());


                    //6. Remove address
                    //var allowAddressIds = new List<Guid>();
                    //var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();

                    //var organizationAddressRepository = unitOfWork.CreateRepository<IOrganizationAddressRepository>();
                    //var organizationAddressIds = organizationAddressRepository.All().Select(x => x.AddressId).Distinct().ToList();
                    //allowAddressIds.AddRange(organizationAddressIds);

                    //var serviceChannelAddressRepository = unitOfWork.CreateRepository<IServiceChannelAddressRepository>();
                    //var serviceChannelAddressIds = serviceChannelAddressRepository.All().Select(x => x.AddressId).Distinct().ToList();
                    //allowAddressIds.AddRange(serviceChannelAddressIds);

                    //var accessibilityRegisterEntranceRepository = unitOfWork.CreateRepository<IAccessibilityRegisterEntranceRepository>();
                    //var accessibilityRegisterEntranceIds = accessibilityRegisterEntranceRepository.All().Where(x => x.AddressId != null).Select(x => x.AddressId.Value).Distinct().ToList();
                    //allowAddressIds.AddRange(accessibilityRegisterEntranceIds);

                    //var allRemoveAddressIds = addressRepository.All().Where(x => !allowAddressIds.Contains(x.Id)).Select(x => x.Id).ToList();
                    //RemoveData(databaseRawContext, "Address", "Id", allRemoveAddressIds.Take(150000).ToList());

                    //7.Remove phones
                    //var allowPhoneIds = new List<Guid>();
                    //var phoneRepository = unitOfWork.CreateRepository<IPhoneRepository>();

                    //var organizationPhoneRepository = unitOfWork.CreateRepository<IOrganizationPhoneRepository>();
                    //var organizationPhoneIds = organizationPhoneRepository.All().Select(x => x.Phone.Id).Distinct().ToList();
                    //allowPhoneIds.AddRange(organizationPhoneIds);

                    //var serviceChannelPhoneRepository = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                    //var serviceChannelPhoneIds = serviceChannelPhoneRepository.All().Select(x => x.Phone.Id).Distinct().ToList();
                    //allowPhoneIds.AddRange(serviceChannelPhoneIds);

                    //var serviceServiceChannelPhoneRepository = unitOfWork.CreateRepository<IServiceServiceChannelPhoneRepository>();
                    //var serviceServiceChannelIds = serviceServiceChannelPhoneRepository.All().Select(x => x.Phone.Id).Distinct().ToList();
                    //allowPhoneIds.AddRange(serviceServiceChannelIds);

                    //var allRemovePhoneIds = phoneRepository.All().Where(x => !allowPhoneIds.Contains(x.Id)).Select(x => x.Id).ToList();
                    //RemoveData(databaseRawContext, "Phone", "Id", allRemovePhoneIds.Take(150000).ToList());

                    //8. Remove emails
                    //var allowEmailIds = new List<Guid>();
                    //var emailRepository = unitOfWork.CreateRepository<IEmailRepository>();

                    //var organizationEmailRepository = unitOfWork.CreateRepository<IOrganizationEmailRepository>();
                    //var organizationEmailIds = organizationEmailRepository.All().Select(x => x.EmailId).Distinct().ToList();
                    //allowEmailIds.AddRange(organizationEmailIds);

                    //var serviceChannelEmailRepository = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();
                    //var serviceChannelEmailIds = serviceChannelEmailRepository.All().Select(x => x.EmailId).Distinct().ToList();
                    //allowEmailIds.AddRange(serviceChannelEmailIds);

                    //var serviceServiceChannelEmailRepository = unitOfWork.CreateRepository<IServiceServiceChannelEmailRepository>();
                    //var serviceServiceChannelEmailIds = serviceServiceChannelEmailRepository.All().Select(x => x.EmailId).Distinct().ToList();
                    //allowEmailIds.AddRange(serviceServiceChannelEmailIds);

                    //var allRemoveEmailIds = emailRepository.All().Where(x => !allowEmailIds.Contains(x.Id)).Select(x => x.Id).ToList();
                    //RemoveData(databaseRawContext, "Email", "Id", allRemoveEmailIds.Take(150000).ToList());

                    watch.Stop();
                    System.Console.WriteLine($"Execution Time: {watch.Elapsed}");

                //unitOfWork.Save(SaveMode.NonTrackedDataMigration);

                });

            }
        }

//        private static void RemoveData(IDatabaseRawContext databaseRawContext, string tableName, string columnName,
//            List<Guid> removeIds)
//        {
//            databaseRawContext.ExecuteWriter(unitOfDbWork =>
//            {
//                try
//                {
//                    var command = $"DELETE FROM \"{tableName}\" WHERE \"{columnName}\" = ANY(@data);";
//                    unitOfDbWork.Command(command, new {data = removeIds});
//                    unitOfDbWork.Save();
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine(e);
//                    throw;
//                }
//            });
//        }
    }
}
