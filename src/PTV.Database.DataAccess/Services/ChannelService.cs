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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;

using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq.Expressions;
using System.Net;
using PTV.Domain.Logic.Channels;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Logic;
using PTV.Framework.Extensions;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Logic.Address;
using PTV.Framework.Exceptions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.Interfaces.V2;
using Microsoft.Extensions.Options;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IChannelService), RegisterType.Transient)]
    internal class ChannelService : ServiceBase, IChannelService
    {
        private IContextManager contextManager;
        private readonly IUserIdentification userIdentification;
        private ILogger logger;
        private const string invalidElectronicChannelUrl = "Electronic channel url '{0}'";
        private const string invalidWebPageChannelUrl = "Web page channel url '{0}'";
        private const string invalidElectronicChannelAttachmentUrl = "Electronic channel attachment url '{0}'";
        private ServiceChannelLogic channelLogic;
        private ServiceUtilities utilities;
        private ApplicationConfiguration configuration;
        private ICommonService commonService;
        private VmListItemLogic listItemLogic;
        private readonly DataUtils dataUtils;
        private VmOwnerReferenceLogic ownerReferenceLogic;
        private IAddressService addressService;
        private ITypesCache typesCache;
        private readonly ProxyServerSettings proxySettings;
        private ILanguageCache languageCache;

        public ChannelService(IContextManager contextManager, IUserIdentification userIdentification, ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity, ILogger<ChannelService> logger, ServiceChannelLogic channelLogic,
            ServiceUtilities utilities, ApplicationConfiguration configuration, ICommonService commonService, VmListItemLogic listItemLogic,
            DataUtils dataUtils, VmOwnerReferenceLogic ownerReferenceLogic, IAddressService addressService, ICacheManager cacheManager, IOptions<ProxyServerSettings> proxySettings,
            IPublishingStatusCache publishingStatusCache) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.channelLogic = channelLogic;
            this.userIdentification = userIdentification;
            this.utilities = utilities;
            this.configuration = configuration;
            this.commonService = commonService;
            this.listItemLogic = listItemLogic;
            this.dataUtils = dataUtils;
            this.ownerReferenceLogic = ownerReferenceLogic;
            this.addressService = addressService;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.proxySettings = proxySettings.Value;
        }

        public IVmChannelSearch GetChannelSearch()
        {
            string statusDeletedCode = PublishingStatus.Deleted.ToString();

            VmChannelSearch result = new VmChannelSearch();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var preselectedOrg = utilities.GetUserOrganization(unitOfWork);
                result = new VmChannelSearch()
                {
                    OrganizationId = preselectedOrg?.Id

                };
                var publishingStatuses = commonService.GetPublishingStatuses(unitOfWork);
                var phoneNumbers = commonService.GetPhoneTypes(unitOfWork);
                var channelTypes = commonService.GetServiceChannelTypes(unitOfWork);
                var chargeTypes = commonService.GetPhoneChargeTypes(unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", phoneNumbers),
                    () => GetEnumEntityCollectionModel("ChannelTypes", channelTypes),
                    () => GetEnumEntityCollectionModel("ChargeTypes", chargeTypes)
                );

                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusDeletedCode).Select(x => x.Id).ToList();
                result.SelectedPhoneNumberTypes = phoneNumbers.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            });
            return result;
        }

        public IVmChannelSearchResult SearchChannelResult(VmChannelSearchParams vm)
        {

            IReadOnlyList<IVmChannelListItem> result = new List<VmChannelListItem>();
            var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(vm);

                var searchText = vm.ChannelName.ToLower();
                var formIdentifier = vm.ChannelFormIdentifier?.ToLower();
                var channelType = vm.ChannelType?.ToLower();

                var channelRep = unitOfWork.CreateRepository<IServiceChannelRepository>(); //TODO use right repository by channelType
                var phoneRep = unitOfWork.CreateRepository<IPhoneRepository>();

                if (!string.IsNullOrEmpty(vm.ChannelType))
                {
                    vm.ChannelTypeId = unitOfWork.CreateRepository<IServiceChannelTypeRepository>().All().First(cht => cht.Code.ToLower() == vm.ChannelType.ToLower()).Id;
                }

                var resultTemp = channelRep.All();

                //For relation section
                var excludeChannelIds= PrefilterServiceChannelIds(vm.serviceChannelRelations);
                if (excludeChannelIds.Any())
                {
                    resultTemp = resultTemp.Where(sc => !excludeChannelIds.Contains(sc.Id));
                }

                if (vm.ChannelTypeId.HasValue)
                {
                    resultTemp = resultTemp.Where(sc => sc.TypeId == vm.ChannelTypeId);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    resultTemp =
                        resultTemp.Where(sc => sc.ServiceChannelNames.Any(y => y.Name.ToLower().Contains(searchText) &&
                                                                               y.LocalizationId ==
                                                                               languageCache.Get(vm.Language.ToString())));
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(sc => sc.ServiceChannelNames.Any(y => y.LocalizationId == languageCache.Get(vm.Language.ToString()) &&
                                        !string.IsNullOrEmpty(y.Name)));
                }

                if (vm.OrganizationId != null)
                {
                    resultTemp = resultTemp.Where(sc => sc.OrganizationId != null && sc.OrganizationId == vm.OrganizationId);
                }

                if (vm.SelectedPublishingStatuses != null)
                {
                    resultTemp = resultTemp.WherePublishingStatusIn(vm.SelectedPublishingStatuses);
                }

                if (vm.ChannelType?.ToLower() == ServiceChannelTypeEnum.Phone.ToString().ToLower())
                {
                    if (vm.SelectedPhoneNumberTypes?.Any() == true)
                    {
                        var scIds = phoneRep.All().Where(x => vm.SelectedPhoneNumberTypes.Contains(x.TypeId)).SelectMany(x => x.ServiceChannelPhones.Select(y => y.ServiceChannelId)).ToList();

                        resultTemp = resultTemp.Where(sc => scIds.Contains(sc.Id));
                    }
                }
                if (vm.ChannelType?.ToLower() == ServiceChannelTypeEnum.PrintableForm.ToString().ToLower())
                {
                    if (!string.IsNullOrEmpty(formIdentifier))
                    {
                        resultTemp = resultTemp.Where(sc => sc.PrintableFormChannels.Any(y => y.FormIdentifier.ToLower().Contains(formIdentifier)));
                    }
                }

                count = resultTemp.Count();
                resultTemp = resultTemp.OrderBy(x => x.Id);

                resultTemp = vm.PageNumber > 0
                    ? resultTemp.Skip(vm.PageNumber * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems)
                    : resultTemp.Take(CoreConstants.MaximumNumberOfAllItems);

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.Type)
                    .Include(i => i.PublishingStatus)
                    .Include(i => i.Phones).ThenInclude(i => i.Phone).ThenInclude(i => i.Type)
                    .Include(i => i.PrintableFormChannels)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.Service));

                result = TranslationManagerToVm.TranslateAll<ServiceChannel, VmChannelListItem>(resultTemp);
            });

            return new VmChannelSearchResult() { Channels = result, ChannelType = vm.ChannelType, PageNumber = ++vm.PageNumber, Count = count };
        }

        public IVmChannelServiceStep GetChannelServiceStep(VmChannelSearchParams vm)
        {
            var result = new VmChannelServiceStep();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(vm);
                result = GetModel<ServiceChannel, VmChannelServiceStep>(GetEntity<ServiceChannel>(vm.ChannelId, unitOfWork,
                    q => q.Include(x => x.ServiceServiceChannels).ThenInclude(x=>x.Service).ThenInclude(x=>x.ServiceNames).ThenInclude(i => i.Type).
                        Include(x => x.ServiceServiceChannels).ThenInclude(x => x.Service).ThenInclude(x=>x.Type)
                        ));
            });
            return result;
        }



        public IVmChannelSearchResultBase ConnectingChannelSearchResult(VmServiceStep4 vm)
        {
            IReadOnlyList<IVmChannelListItem> result = new List<VmChannelListItem>();
            var maxNumberOfItems = CoreConstants.MaximumNumberOfAllItems;
            int numberOfAllItems = 0;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var channelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
                var resultTemp = channelRep.All();

                if (!string.IsNullOrEmpty(vm.ChannelName))
                {
                    var searchText = vm.ChannelName.ToLower();
                    resultTemp = resultTemp.Where(x => x.ServiceChannelNames.Any(y => y.Name.ToLower().Contains(searchText)));
                }

                if (vm.OrganizationId != null && vm.OrganizationId != Guid.Empty)
                {
                    resultTemp = resultTemp.Where(x => x.OrganizationId == vm.OrganizationId);
                }

                if (vm.SelectedChannelTypes != null && vm.SelectedChannelTypes.Any())
                {
                    resultTemp = resultTemp.Where(x => vm.SelectedChannelTypes.Any(y => y == x.TypeId));
                }

                numberOfAllItems = resultTemp.Count();

                resultTemp = resultTemp
                    .OrderBy(i => i.ServiceChannelNames.FirstOrDefault(x => x.Type.Code == NameTypeEnum.Name.ToString()).Name)
                    .Take(maxNumberOfItems);

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.ServiceChannelNames).ThenInclude(i => i.Type))
                     .Include(i => i.Type);

                result = TranslationManagerToVm.TranslateAll<ServiceChannel, VmChannelListItem>(resultTemp);
            });

            return new VmChannelSearchResultBase() { Channels = result, IsMoreThanMax = (numberOfAllItems > maxNumberOfItems), NumberOfAllItems = numberOfAllItems };
        }

        void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
        }

        public IVmUrlChecker CheckUrl(VmUrlChecker model)
        {
            if (!configuration.IsInternetAvailable())
            {
                return new VmUrlChecker() { UrlExists = null, Id = model.Id, UrlAddress = model.UrlAddress };
            }

            WebProxy proxy = null;

            if ((proxySettings != null) && (!string.IsNullOrEmpty(proxySettings.Address)))
            {
                string proxyUri = string.Format("{0}:{1}", proxySettings.Address, proxySettings.Port);
                NetworkCredential proxyCreds = new NetworkCredential(proxySettings.UserName, proxySettings.Password);
                proxy = new WebProxy(proxyUri)
                {
                    Credentials = proxyCreds,
                };
            }

            var uri = new UriBuilder(model.UrlAddress).Uri;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.ContinueTimeout = 1000;
            request.Method = "HEAD";
            request.Proxy = proxy;
            try
            {
                var task = request.GetResponseAsync();
                task.Wait();
                using (var response = (HttpWebResponse)task.Result)
                {
                    return new VmUrlChecker() { UrlExists = response.StatusCode == HttpStatusCode.OK, Id = model.Id, UrlAddress = model.UrlAddress };
                }

            }
            catch (Exception)
            {
                return new VmUrlChecker() { UrlExists = false, Id = model.Id, UrlAddress = model.UrlAddress };
            }


        }

        private void SetupChannelDescription(IUnitOfWork unitOfWork, IVmChannelDescription model, bool isNew)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (isNew)
            {
                var userOrg = utilities.GetUserOrganization(unitOfWork);
                model.OrganizationId = userOrg?.Id;
            }
        }

        private VmPhone SetupPhoneTypes(IUnitOfWork unitOfWork, Guid? entityId, string numberTypeCode = null)
        {
            if (entityId.IsAssigned())
            {
                var serviceChannelPhoneRep = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                var phones = serviceChannelPhoneRep.All().Where(x => x.ServiceChannelId == entityId).Include(x=>x.Phone);
                var phone = string.IsNullOrEmpty(numberTypeCode) ? phones.FirstOrDefault() : phones.FirstOrDefault(x => x.Phone.Type.Code == numberTypeCode);
                return phone != null ? new VmPhone() { ChargeTypeId = phone.Phone.ServiceChargeTypeId, TypeId = phone.Phone.TypeId, Id = Guid.NewGuid() } : null;
            }
            return null;
        }

        private void UpdatePhoneTypes(IUnitOfWork unitOfWork, VmPhone model, Guid? entityId, string numberTypeCode = null)
        {
            if (entityId.IsAssigned())
            {
                var serviceChannelPhoneRep = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                var phones = serviceChannelPhoneRep.All().Where(x => x.ServiceChannelId == entityId && x.PhoneId.IsAssigned()).Include(x=>x.Phone);
                if (!string.IsNullOrEmpty(numberTypeCode))
                {
                    phones = phones.Where(x => x.Phone.Type.Code == numberTypeCode).Include(x=>x.Phone);
                }
                phones.ForEach(x =>
                {
                    x.Phone.ServiceChargeTypeId = model.ChargeTypeId;
                    x.Phone.TypeId = model.TypeId;
                });
            }
        }

        #region Electronic channel

        public IVmElectronicChannelStep1 GetElectronicChannelStep1(IVmGetChannelStep model)
        {
            VmElectronicChannelStep1 result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannel, VmElectronicChannelStep1>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                    q => q.Include(x => x.ServiceChannelNames)
                         .Include(x => x.ServiceChannelDescriptions)
                         .Include(x => x.PublishingStatus)
                         .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                         .Include(x => x.ElectronicChannels).ThenInclude(x => x.LocalizedUrls)
                         .Include(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                         .Include(i => i.ServiceHours).ThenInclude(i => i.ServiceHourType)
                         .Include(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
                         .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.Type)
                         .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.ServiceChargeType)
                         .Include(x => x.Phones).ThenInclude(x => x.Phone)
                         .Include(x => x.Emails).ThenInclude(x => x.Email)
                         ));
                var publishingStatuses = commonService.GetPublishingStatuses(unitOfWork);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses)
               );

                SetupChannelDescription(unitOfWork, result, !model.EntityId.IsAssigned());
                result.PhoneNumber = result.PhoneNumber ?? SetupPhoneTypes(unitOfWork, model.EntityId);
            });
            return result;
        }

        public IVmEntityBase AddElectronicChannel(VmElectronicChannel model)
        {
            Guid? entityId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                entityId = AddElectronicChannel(unitOfWork, model);
            });
            return new VmEntityStatusBase() { Id = entityId, PublishingStatus = commonService.GetDraftStatusId()};
        }

        public IVmElectronicChannelStep1 SaveElectronicChannelStep1(VmElectronicChannelStep1 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
                channelLogic.PrefilterViewModel(model);
                var electronicChannelRep = unitOfWork.CreateRepository<IElectronicChannelRepository>();
                model.ElectronicChannelId = electronicChannelRep.All().Where(i => i.ServiceChannelId == model.Id).Select(i => i.Id).FirstOrDefault();
                var serviceChannel = TranslateElectronicChannel(unitOfWork, model);

                //Removing attachments
                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Attachments.Select(x => x.Attachment).ToList(),
                    curr => curr.ServiceChannelAttachments.Any(x => x.ServiceChannelId == model.Id) && curr.LocalizationId == languageId);

                UpdatePhoneTypes(unitOfWork, model.PhoneNumber, model.Id);
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;
            });
            return GetElectronicChannelStep1(new VmGetChannelStep { EntityId = channelId, Language = model.Language});
        }

        public IVmOpeningHours SaveOpeningHoursStep(VmOpeningHoursStep model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                channelLogic.PrefilterViewModel(model);
                var serviceChannel = TranslationManagerToEntity.Translate<VmOpeningHoursStep, ServiceChannel>(model, unitOfWork);
                serviceChannel.ServiceHours = dataUtils.UpdateCollectionWithRemove(unitOfWork, serviceChannel.ServiceHours,
                    query => query.ServiceChannelId == model.Id);

                // Remove daily opening hours
                var openingTimeRep = unitOfWork.CreateRepository<IDailyOpeningTimeRepository>();

                foreach (var serviceHour in serviceChannel.ServiceHours)
                {
                    var openingTimes = serviceHour.DailyOpeningTimes.ToList();
                    var openingTimesToRemove = openingTimeRep.All().Where(x => x.OpeningHourId == serviceHour.Id && !openingTimes.Contains(x));
                    openingTimesToRemove.ForEach(x => openingTimeRep.Remove(x));
                }

                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;
            });
            return GetOpeningHoursStep(new VmGetChannelStep { EntityId = channelId, Language = model.Language });
        }

        private Guid? AddElectronicChannel(IUnitOfWorkWritable unitOfWork, VmElectronicChannel vm)
        {
            SetTranslatorLanguage(vm);
            channelLogic.PrefilterViewModel(vm);
            vm.PublishingStatus = commonService.GetDraftStatusId();
            var electronicChannelRep = unitOfWork.CreateRepository<IElectronicChannelRepository>();
            vm.Step1Form.ElectronicChannelId = electronicChannelRep.All().Where(i => i.ServiceChannelId == vm.Id).Select(i => i.Id).FirstOrDefault();
            var serviceChannel = TranslateElectronicChannel(unitOfWork, vm);
            unitOfWork.Save();
            return serviceChannel.Id;
        }

        private ServiceChannel TranslateElectronicChannel<TElectronicChannelModel>(IUnitOfWorkWritable unitOfWork, TElectronicChannelModel vm) where TElectronicChannelModel : class
        {
            var serviceChannel = TranslationManagerToEntity.Translate<TElectronicChannelModel, ServiceChannel>(vm, unitOfWork);
            CheckElectronicChannelValidity(serviceChannel);
            return serviceChannel;
        }

        private void CheckElectronicChannelValidity(ServiceChannel channel)
        {
            var channelUrl = channel.ElectronicChannels?.FirstOrDefault()?.LocalizedUrls?.FirstOrDefault();
            if (channelUrl != null)
            {
                var resultUrl = CheckUrl(new VmUrlChecker() { UrlAddress = channelUrl.Url });
                // the channel should be stored, even the webpage is not exist
                //if (resultUrl.UrlExists.HasValue && !resultUrl.UrlExists.Value)
                //{
                //    throw new ArgumentException("", string.Format(invalidElectronicChannelUrl, channelUrl.Url));
                //}
            }
            if (channel.Attachments?.Any() == true)
            {
                foreach (var attachment in channel.Attachments)
                {
                    if (!string.IsNullOrEmpty(attachment.Attachment.Url))
                    {
                        var resultUrl = CheckUrl(new VmUrlChecker() { UrlAddress = attachment.Attachment.Url });
                        // the channel should be stored, even the webpage is not exist
                        //if (resultUrl.UrlExists.HasValue && !resultUrl.UrlExists.Value)
                        //{
                        //    throw new ArgumentException("", string.Format(invalidElectronicChannelAttachmentUrl, attachment.Attachment.Url) );
                        //}
                    }
                }
            }
        }

        #endregion Electronic channel

        #region Location channel

        public IVmLocationChannelStep1 GetLocationChannelStep1(IVmGetChannelStep model)
        {
            var result = new VmLocationChannelStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result =
                    GetModel<ServiceChannel, VmLocationChannelStep1>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                        q => q.Include(x => x.ServiceChannelNames)
                         .Include(x => x.ServiceChannelDescriptions)
                         .Include(x => x.PublishingStatus)
                         .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.ServiceAreas).ThenInclude(x => x.Municipality)
                         .Include(x => x.Languages).ThenInclude(x => x.Language).ThenInclude(x => x.LanguageNames)
                         ));

                SetupChannelDescription(unitOfWork, result, !model.EntityId.IsAssigned());

                var publishingStatuses = commonService.GetPublishingStatuses(unitOfWork);
                var municipalities = commonService.GetMunicipalities(unitOfWork);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("Municipalities", municipalities),
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages(unitOfWork))
                );
            });
            return result;
        }

        public IVmLocationChannelStep2 GetLocationChannelStep2(IVmGetChannelStep model)
        {
            var result = new VmLocationChannelStep2();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannel, VmLocationChannelStep2>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                    q => q.Include(x => x.WebPages).ThenInclude(x => x.WebPage)
                         .Include(x => x.WebPages).ThenInclude(x => x.Type)
                         .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.Type)
                         .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.ServiceChargeType)
                         .Include(x => x.Phones).ThenInclude(x => x.Phone)
                         .Include(x => x.Emails).ThenInclude(x => x.Email)
                         ));


                //utilities.SetupContactInfoModel(unitOfWork, result, !channelId.IsAssigned());
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("WebPageTypes", commonService.GetWebPageTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes(unitOfWork))
                );
                result.PhoneNumber = result.PhoneNumber ?? SetupPhoneTypes(unitOfWork, model.EntityId, PhoneNumberTypeEnum.Phone.ToString());
                result.Fax = result.Fax ?? SetupPhoneTypes(unitOfWork, model.EntityId, PhoneNumberTypeEnum.Fax.ToString());
            });
            return result;
        }

        public IVmLocationChannelStep3 GetLocationChannelStep3(IVmGetChannelStep model)
        {
            var result = new VmLocationChannelStep3();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannel, VmLocationChannelStep3>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                    q => q.Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Type)
                         .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.StreetNames)
                         .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.PostalCode)
						 .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                         ));

            });
            return result;
        }

        public IVmOpeningHours GetOpeningHoursStep(IVmGetChannelStep model)
        {
            var result = new VmOpeningHoursStep();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannel, VmOpeningHoursStep>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                    q => q.Include(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
                        .Include(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                        .Include(j => j.ServiceHours)));

            });
            return result;
        }

        public IVmEntityBase AddLocationChannel(VmLocationChannel model)
        {
            var result = new ServiceChannel();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddLocationChannel(unitOfWork, model);
                unitOfWork.Save();
            });
            var addresses = result.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses.ToList());
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = commonService.GetDraftStatusId() };
        }

        private ServiceChannel AddLocationChannel(IUnitOfWorkWritable unitOfWork, VmLocationChannel vm)
        {
            SetTranslatorLanguage(vm);
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            vm.PublishingStatus = commonService.GetDraftStatusId();
            channelLogic.PrefilterViewModel(vm);
            var serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannel, ServiceChannel>(vm, unitOfWork);
            serviceChannelRep.Add(serviceChannel);
            return serviceChannel;
        }

        public IVmLocationChannelStep1 SaveLocationChannelStep1(VmLocationChannelStep1 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                var serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannelStep1, ServiceChannel>(model, unitOfWork);
                serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                    query => query.ServiceChannelId == serviceChannel.Id,
                    lang => lang.LanguageId);

                serviceChannel.ServiceLocationChannels?.FirstOrDefault().SafeCall(i => i.ServiceAreas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, i.ServiceAreas,
                    query => query.ServiceLocationChannelId == i.Id,
                    lang => lang.MunicipalityId));
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;

            });
            return GetLocationChannelStep1(new VmGetChannelStep { EntityId = channelId, Language = model.Language });
        }
        public IVmLocationChannelStep2 SaveLocationChannelStep2(VmLocationChannelStep2 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
                channelLogic.PrefilterViewModel(model);
                var serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannelStep2, ServiceChannel>(model, unitOfWork);


                //Removing web pages
                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.WebPages.Select(x => x.WebPage).ToList(),
                    curr => curr.ServiceChannelWebPages.Any(x => x.ServiceChannelId == model.Id) && curr.LocalizationId == languageId);


                UpdatePhoneTypes(unitOfWork, model.PhoneNumber, model.Id, PhoneNumberTypeEnum.Phone.ToString());
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;

            });
            return GetLocationChannelStep2(new VmGetChannelStep { EntityId = channelId, Language = model.Language });
        }
        public IVmLocationChannelStep3 SaveLocationChannelStep3(VmLocationChannelStep3 model)
        {
            Guid? channelId = null;
            ServiceChannel serviceChannel = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                channelLogic.PrefilterViewModel(model);
                if (model.Id.HasValue)
                {
                    ownerReferenceLogic.SetOwnerReference(model.PostalAddresses.Concat(model.VisitingAddresses), model.Id.Value);
                }
                serviceChannel = TranslationManagerToEntity.Translate<VmLocationChannelStep3, ServiceChannel>(model, unitOfWork);

                var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();
                var channelAddressRepository = unitOfWork.CreateRepository<IServiceLocationChannelAddressRepository>();
                var wpIds = serviceChannel.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.Address.Id).ToList();

                var existingAddresses = channelAddressRepository.All().Where(address => address.ServiceLocationChannel.ServiceChannelId == model.Id).Select(x => x.Address).ToList();
                if (wpIds != null)
                {
                    addressRepository.Remove(existingAddresses.Where(x => !wpIds.Contains(x.Id)));
                }
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;

            });
            var addresses = serviceChannel.ServiceLocationChannels?.FirstOrDefault()?.Addresses?.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses.ToList());
            return GetLocationChannelStep3(new VmGetChannelStep { EntityId = channelId, Language = model.Language });
        }

        #endregion Location channel

        #region PhoneChannel

        public IVmPhoneChannelStep1 GetPhoneChannelStep1(IVmGetChannelStep model)
        {
            var result = new VmPhoneChannelStep1();

            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannel, VmPhoneChannelStep1>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                    q => q.Include(j => j.Attachments).ThenInclude(j => j.Attachment)
                        .Include(j => j.PublishingStatus)
                        .Include(j => j.ServiceChannelDescriptions)
                        .Include(j => j.ServiceChannelNames)
                        .Include(j => j.Languages).ThenInclude(j => j.Language).ThenInclude(j => j.LanguageNames)
                        .Include(j => j.Organization).ThenInclude(j => j.OrganizationNames).ThenInclude(k => k.Type)
                        .Include(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                        .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                        .Include(j => j.Emails).ThenInclude(j => j.Email)
                        .Include(j => j.Phones).ThenInclude(j => j.Phone)));

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages(unitOfWork).ToList()),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", commonService.GetPhoneTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes(unitOfWork).ToList())
                    );

                SetupChannelDescription(unitOfWork, result, !model.EntityId.IsAssigned());
                result.PhoneNumber = result.PhoneNumber ?? SetupPhoneTypes(unitOfWork, model.EntityId);
            });
            return result;
        }

        public IVmEntityBase AddPhoneChannel(VmPhoneChannel model)
        {
            ServiceChannel result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddPhoneChannel(model, unitOfWork);
                unitOfWork.Save();
            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = commonService.GetDraftStatusId() };
        }

        private ServiceChannel AddPhoneChannel(VmPhoneChannel vm, IUnitOfWorkWritable unitOfWork)
        {
            SetTranslatorLanguage(vm);
            vm.PublishingStatus = PublishingStatus.Draft;
            channelLogic.PrefilterViewModel(vm);
            return TranslationManagerToEntity.Translate<VmPhoneChannel, ServiceChannel>(vm, unitOfWork);
        }

        public IVmPhoneChannelStep1 SavePhoneChannelStep1(VmPhoneChannelStep1 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                var serviceChannel = TranslationManagerToEntity.Translate<VmPhoneChannelStep1, ServiceChannel>(model, unitOfWork);
                serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                query => query.ServiceChannelId == serviceChannel.Id,
                lang => lang.LanguageId);
                UpdatePhoneTypes(unitOfWork,model.PhoneNumber, model.Id);
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;
            });
            return GetPhoneChannelStep1(new VmGetChannelStep { EntityId = channelId, Language = model.Language });
        }

        #endregion PhoneChannel

        #region WebPage channel

        public IVmWebPageChannelStep1 GetWebPageChannelStep1(IVmGetChannelStep model)
        {
            var result = new VmWebPageChannelStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannel, VmWebPageChannelStep1>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                    q => q.Include(j => j.Attachments).ThenInclude(j => j.Attachment)
                        .Include(x => x.PublishingStatus)
                        .Include(j => j.ServiceChannelDescriptions)
                        .Include(j => j.ServiceChannelNames)
                        .Include(j => j.WebpageChannels).ThenInclude(j => j.LocalizedUrls)
                        .Include(j => j.Keywords).ThenInclude(j => j.Keyword)
                        .Include(j => j.Languages).ThenInclude(j => j.Language).ThenInclude(j => j.LanguageNames)
                        .Include(x => x.Emails).ThenInclude(x => x.Email)
                        .Include(x => x.Phones).ThenInclude(x => x.Phone)
                        .Include(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                        .Include(j => j.Organization).ThenInclude(j => j.OrganizationNames).ThenInclude(k => k.Type)
                        .Include(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass).ThenInclude(k => k.Names)
                        .Include(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                        .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                		.Include(j => j.WebPages).ThenInclude(j => j.Type)
                ));

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork))
                );

                SetupChannelDescription(unitOfWork, result, !model.EntityId.IsAssigned());
                result.PhoneNumber = result.PhoneNumber ?? SetupPhoneTypes(unitOfWork, model.EntityId);
            });
            return result;
        }

        public IVmEntityBase AddWebPageChannel(VmWebPageChannel model)
        {
            ServiceChannel result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddWebPageChannel(unitOfWork, model);
                unitOfWork.Save();

            });
            return new VmChannelBase() { Id = result.Id, PublishingStatus = commonService.GetDraftStatusId() };
        }

        public IVmWebPageChannelStep1 SaveWebPageChannelStep1(VmWebPageChannelStep1 model)
        {
            Guid? channelId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                var webpageChannelRep = unitOfWork.CreateRepository<IWebpageChannelRepository>();
                model.WebPageChannelId = webpageChannelRep.All().Where(i => i.ServiceChannelId == model.Id).Select(i => i.Id).FirstOrDefault();
                var serviceChannel = TranslationManagerToEntity.Translate<VmWebPageChannelStep1, ServiceChannel>(model, unitOfWork);
                serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                query => query.ServiceChannelId == serviceChannel.Id,
                lang => lang.LanguageId);;
                UpdatePhoneTypes(unitOfWork, model.PhoneNumber, model.Id);
                unitOfWork.Save(parentEntity: serviceChannel);
                channelId = serviceChannel.Id;

            });
            return GetWebPageChannelStep1(new VmGetChannelStep { EntityId = channelId, Language = model.Language });
        }

        private ServiceChannel AddWebPageChannel(IUnitOfWorkWritable unitOfWork, VmWebPageChannel vm)
        {
            SetTranslatorLanguage(vm);
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            vm.PublishingStatus = commonService.GetDraftStatusId();
            var serviceChannel = TranslationManagerToEntity.Translate<VmWebPageChannel, ServiceChannel>(vm, unitOfWork);
            CheckWebPageChannelValidity(serviceChannel);
            serviceChannelRep.Add(serviceChannel);
            return serviceChannel;
        }

        private void CheckWebPageChannelValidity(ServiceChannel channel)
        {
            var channelUrl = channel.WebpageChannels?.FirstOrDefault()?.LocalizedUrls.FirstOrDefault();
            if (channelUrl != null)
            {
                var resultUrl = CheckUrl(new VmUrlChecker() { UrlAddress = channelUrl.Url });

                // the channel should be stored, even the webpage is not exist
                //if (resultUrl.UrlExists.HasValue && !resultUrl.UrlExists.Value)
                //{
                //    //throw new ArgumentException("", string.Format(invalidElectronicChannelUrl, channelUrl.Url));
                //}
            }
        }
        #endregion WebPage channel

        #region Printable channel
        public IVmPrintableFormChannelStep1 GetPrintableFormChannelStep1(IVmGetChannelStep model)
        {
            var result = new VmPrintableFormChannelStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                result = GetModel<ServiceChannel, VmPrintableFormChannelStep1>(GetEntity<ServiceChannel>(model.EntityId, unitOfWork,
                    q => q.Include(x => x.ServiceChannelNames)
                         .Include(x => x.ServiceChannelDescriptions)
                         .Include(x => x.Emails).ThenInclude(x => x.Email)
                         .Include(x => x.Phones).ThenInclude(x => x.Phone)
                         .Include(x => x.PublishingStatus)
                         .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(x => x.StreetNames)
                         .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(x => x.PostalCode)
                         .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                         .Include(x => x.PrintableFormChannels).ThenInclude(x => x.ChannelUrls).ThenInclude(x => x.Type)
                         .Include(x => x.PrintableFormChannels).ThenInclude(x => x.DeliveryAddress).ThenInclude(x => x.AddressAdditionalInformations)
                         ));

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("PrintableFormUrlTypes", commonService.GetPrintableFormUrlTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork))
                );

                SetupChannelDescription(unitOfWork, result, !model.EntityId.IsAssigned());
                result.PhoneNumber = result.PhoneNumber ?? SetupPhoneTypes(unitOfWork, model.EntityId);
            });
            return result;
        }

        public IVmEntityBase AddPrintableFormChannel(VmPrintableFormChannel model)
        {
            ServiceChannel result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddPrintableFormChannel(unitOfWork, model);
                unitOfWork.Save();

            });

            addressService.UpdateAddress(result?.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select(x => x.DeliveryAddressId.Value).ToList());
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = commonService.GetDraftStatusId() };
        }

        private ServiceChannel AddPrintableFormChannel(IUnitOfWorkWritable unitOfWork, VmPrintableFormChannel vm)
        {
            SetTranslatorLanguage(vm);
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            vm.PublishingStatus = commonService.GetDraftStatusId();
            //channelLogic.PrefilterViewModel(vm);  TODO Change
            var serviceChannel = TranslationManagerToEntity.Translate<VmPrintableFormChannel, ServiceChannel>(vm, unitOfWork);
            //CheckWebPageChannelValidity(serviceChannel); TODO Change
            serviceChannelRep.Add(serviceChannel);
            return serviceChannel;
        }

        public IVmPrintableFormChannelStep1 SavePrintableFormChannelStep1(VmPrintableFormChannelStep1 model)
        {
            Guid? channelId = null;
            ServiceChannel serviceChannel = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Guid languageId = languageCache.Get(SetTranslatorLanguage(model).ToString());
                serviceChannel = TranslationManagerToEntity.Translate<VmPrintableFormChannelStep1, ServiceChannel>(model, unitOfWork);

                var webPageRep = unitOfWork.CreateRepository<IPrintableFormChannelUrlRepository>();
                var wpIds = serviceChannel.PrintableFormChannels.SelectMany(x => x.ChannelUrls).Select(x => x.Id).ToList();

                var existingWebPages = webPageRep.All().Where(wp => wp.PrintableFormChannel.ServiceChannelId == model.Id && wp.LocalizationId == languageId).ToList();
                webPageRep.Remove(existingWebPages.Where(x => !wpIds.Contains(x.Id)));

                //Removing attachments
                dataUtils.UpdateCollectionWithRemove(unitOfWork,
                    serviceChannel.Attachments.Select(x => x.Attachment).ToList(),
                    curr => curr.ServiceChannelAttachments.Any(x => x.ServiceChannelId == model.Id) && curr.LocalizationId == languageId);

                UpdatePhoneTypes(unitOfWork, model.PhoneNumber, model.Id);
                unitOfWork.Save(parentEntity: serviceChannel);

                channelId = serviceChannel.Id;
            });

            addressService.UpdateAddress(serviceChannel?.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select( x => x.DeliveryAddressId.Value).ToList());
            return GetPrintableFormChannelStep1(new VmGetChannelStep { EntityId = channelId, Language = model.Language });
        }

        #endregion Printable channel

        #region OpenApi
        public IVmOpenApiGuidPage GetServiceChannels(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            var pagingVm = new VmOpenApiGuidPage(pageNumber, pageSize);

            contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
                // Get only service channels that are published
                var query = serviceChannelRep.All().Where(PublishedFilter<ServiceChannel>()).Where(ValidityFilter<ServiceChannel>());
                if (date.HasValue)
                {
                    query = query.Where(o => o.Modified > date.Value);
                }

                pagingVm.SetPageCount(query.Count());
                if (pagingVm.IsValidPageNumber())
                {
                    if (pagingVm.PageCount > 1)
                    {
                        query = query.OrderBy(o => o.Created).Skip(pagingVm.GetSkipSize()).Take(pagingVm.GetTakeSize());
                    }
                    pagingVm.GuidList = query.ToList().Select(o => o.Id).ToList();
                }
            });

            return pagingVm;
        }

        public IVmOpenApiServiceChannel GetServiceChannel(Guid id)
        {
            return TranslateToVersion1(V2GetServiceChannel(id));
        }

        public IVmOpenApiServiceChannel V2GetServiceChannel(Guid id, bool getOnlyPublished = true)
        {
            try
            {
                var filters = new List<Expression<Func<ServiceChannel, bool>>>() { serviceChannel => serviceChannel.Id.Equals(id) };
                return GetServiceChannelsWithDetails(filters, getOnlyPublished).FirstOrDefault();
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a service channel with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg);
                throw ex;
            }
        }

        public Guid GetServiceChannelBySource(string sourceId, string userName)
        {
            Guid result = Guid.Empty;
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    result = GetPTVId<ServiceChannel>(sourceId, userName, unitOfWork);
                });

                return result;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a service channel with source id {0}. {1}", sourceId, ex.Message);
                logger.LogError(errorMsg);
                throw ex;
            }
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date)
        {
            return TranslateToVersion1(V2GetServiceChannelsByType(type, date));
        }

        public IList<IVmOpenApiServiceChannel> V2GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date)
        {
            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var typeId = GetServiceChannelTypeId(type, unitOfWork);
                    // Define filters that are used to get required service channel.
                    var filters = new List<Expression<Func<ServiceChannel, bool>>>()
                    {
                        serviceChannel => serviceChannel.TypeId == typeId// Get service channels of defined service channel type
                    };
                    if (date.HasValue)
                    {
                        filters.Add(serviceChannel => serviceChannel.Modified > date.Value);
                    }

                    result = GetServiceChannelsWithDetails(filters, unitOfWork);
                });
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error occured while getting service channels of type {0}. {1}", type.ToString(), ex.Message));
                throw ex;
            }

            return result;
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, ServiceChannelTypeEnum? type = null)
        {
            return TranslateToVersion1(V2GetServiceChannelsByOrganization(organizationId, date, type));
        }

        public IList<IVmOpenApiServiceChannel> V2GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, ServiceChannelTypeEnum? type = null)
        {
            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();

            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    // Define filters that are used to get required service channel.
                    var filters = new List<Expression<Func<ServiceChannel, bool>>>()
                    {
                        serviceChannel => serviceChannel.OrganizationId.Equals(organizationId)
                    };
                    if (date.HasValue)
                    {
                        filters.Add(serviceChannel => serviceChannel.Modified > date.Value);
                    }
                    if (type.HasValue)
                    {
                        var typeId = GetServiceChannelTypeId(type.Value, unitOfWork);
                        // Filter by service channel type
                        filters.Add(serviceChannel => serviceChannel.TypeId == typeId);
                    }
                    // Get all the detailed information about service channels matching the filters
                    result = GetServiceChannelsWithDetails(filters, unitOfWork);
                });
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error occured while getting service channels for organization {0}. {1}", organizationId.ToString(), ex.Message));
                throw ex;
            }

            return result;
        }

        public IVmOpenApiElectronicChannel AddElectronicChannel(IVmOpenApiElectronicChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiElectronicChannelInBase, V2VmOpenApiElectronicChannelInBase>(vm);
            return TranslateToVersion1(V2AddElectronicChannel(vm2, allowAnonymous));
        }

        public IV2VmOpenApiElectronicChannel V2AddElectronicChannel(IV2VmOpenApiElectronicChannelInBase vm, bool allowAnonymous)
        {
            return AddServiceChannel<IV2VmOpenApiElectronicChannelInBase, V2VmOpenApiElectronicChannel>(vm, allowAnonymous);
        }

        public IVmOpenApiServiceChannel SaveElectronicChannel(IVmOpenApiElectronicChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiElectronicChannelInBase, V2VmOpenApiElectronicChannelInBase>(vm);
            return TranslateToVersion1(V2SaveElectronicChannel(vm2, allowAnonymous));
        }

        public IVmOpenApiServiceChannel V2SaveElectronicChannel(IV2VmOpenApiElectronicChannelInBase vm, bool allowAnonymous)
        {
            return SaveServiceChannel(vm, allowAnonymous);
        }

        public IVmOpenApiPhoneChannel AddPhoneChannel(IVmOpenApiPhoneChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiPhoneChannelInBase, V2VmOpenApiPhoneChannelInBase>(vm);
            return TranslateToVersion1(V2AddPhoneChannel(vm2, allowAnonymous));
        }

        public IV2VmOpenApiPhoneChannel V2AddPhoneChannel(IV2VmOpenApiPhoneChannelInBase vm, bool allowAnonymous)
        {
            return AddServiceChannel<IV2VmOpenApiPhoneChannelInBase, V2VmOpenApiPhoneChannel>(vm, allowAnonymous);
        }

        public IVmOpenApiServiceChannel SavePhoneChannel(IVmOpenApiPhoneChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiPhoneChannelInBase, V2VmOpenApiPhoneChannelInBase>(vm);
            return TranslateToVersion1(V2SavePhoneChannel(vm2, allowAnonymous));
        }

        public IVmOpenApiServiceChannel V2SavePhoneChannel(IV2VmOpenApiPhoneChannelInBase vm, bool allowAnonymous)
        {
            return SaveServiceChannel(vm, allowAnonymous);
        }

        public IVmOpenApiWebPageChannel AddWebPageChannel(IVmOpenApiWebPageChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiWebPageChannelInBase, V2VmOpenApiWebPageChannelInBase>(vm);
            return TranslateToVersion1(V2AddWebPageChannel(vm2, allowAnonymous));
        }

        public IV2VmOpenApiWebPageChannel V2AddWebPageChannel(IV2VmOpenApiWebPageChannelInBase vm, bool allowAnonymous)
        {
            return AddServiceChannel<IV2VmOpenApiWebPageChannelInBase, V2VmOpenApiWebPageChannel>(vm, allowAnonymous);
        }


        public IVmOpenApiServiceChannel SaveWebPageChannel(IVmOpenApiWebPageChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiWebPageChannelInBase, IV2VmOpenApiWebPageChannelInBase>(vm);
            return TranslateToVersion1(V2SaveWebPageChannel(vm2, allowAnonymous));
        }

        public IVmOpenApiServiceChannel V2SaveWebPageChannel(IV2VmOpenApiWebPageChannelInBase vm, bool allowAnonymous)
        {
            return SaveServiceChannel(vm, allowAnonymous);
        }

        public IVmOpenApiPrintableFormChannel AddPrintableFormChannel(IVmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiPrintableFormChannelInBase, V2VmOpenApiPrintableFormChannelInBase>(vm);
            return TranslateToVersion1(V2AddPrintableFormChannel(vm2, allowAnonymous));
        }

        public IV2VmOpenApiPrintableFormChannel V2AddPrintableFormChannel(IV2VmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous)
        {
            return AddServiceChannel<IV2VmOpenApiPrintableFormChannelInBase, V2VmOpenApiPrintableFormChannel>(vm, allowAnonymous);
        }

        public IVmOpenApiServiceChannel SavePrintableFormChannel(IVmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiPrintableFormChannelInBase, V2VmOpenApiPrintableFormChannelInBase>(vm);
            return TranslateToVersion1(V2SavePrintableFormChannel(vm2, allowAnonymous));
        }

        public IVmOpenApiServiceChannel V2SavePrintableFormChannel(IV2VmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous)
        {
            return SaveServiceChannel(vm, allowAnonymous);
        }

        public IVmOpenApiServiceLocationChannel AddServiceLocationChannel(IVmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiServiceLocationChannelInBase, V2VmOpenApiServiceLocationChannelInBase>(vm);
            return TranslateToVersion1(V2AddServiceLocationChannel(vm2, allowAnonymous));
        }

        public IV2VmOpenApiServiceLocationChannel V2AddServiceLocationChannel(IV2VmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null)
        {
            return AddServiceChannel<IV2VmOpenApiServiceLocationChannelInBase, V2VmOpenApiServiceLocationChannel>(vm, allowAnonymous, userName);
        }

        public IVmOpenApiServiceChannel SaveServiceLocationChannel(IVmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiServiceLocationChannelInBase, V2VmOpenApiServiceLocationChannelInBase>(vm);
            return TranslateToVersion1(V2SaveServiceLocationChannel(vm2, allowAnonymous));
        }

        public IVmOpenApiServiceChannel V2SaveServiceLocationChannel(IV2VmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null)
        {
            return SaveServiceChannel(vm, allowAnonymous, userName);
        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IList<Expression<Func<ServiceChannel, bool>>> filters, bool getOnlyPublished = true)
        {
            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceChannelsWithDetails(filters, unitOfWork, getOnlyPublished);
            });

            return result;
        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IList<Expression<Func<ServiceChannel, bool>>> filters, IUnitOfWork unitOfWork, bool getOnlyPublished = true)
        {
            var resultList = new List<IVmOpenApiServiceChannel>();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            var query = serviceChannelRep.All();
            if (getOnlyPublished)
            {
                query = query.Where(PublishedFilter<ServiceChannel>()).Where(ValidityFilter<ServiceChannel>()); // Get only published service channels
            }

            // Add all filters into query
            filters.ForEach(p => query = query.Where(p));

            var queryWithData = unitOfWork.ApplyIncludes(query, q =>
                q.Include(i => i.ServiceChannelNames)
                .Include(i => i.ServiceChannelDescriptions)
                .Include(i => i.Type)
                .Include(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(j => j.ServiceHours).ThenInclude(j => j.DailyOpeningTimes)
                .Include(j => j.ServiceHours).ThenInclude(j => j.AdditionalInformations)
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment).ThenInclude(i => i.Type)
                .Include(i => i.ElectronicChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.StreetNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.PostalCode)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Municipality)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.ServiceLocationChannels).ThenInclude(i => i.ServiceAreas).ThenInclude(i => i.Municipality)
                .Include(i => i.WebpageChannels).ThenInclude(i => i.LocalizedUrls)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.StreetNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.PostalCode)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.Municipality)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.DeliveryAddress).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.PrintableFormChannels).ThenInclude(i => i.ChannelUrls)
                .Include(i => i.PublishingStatus)
                .Include(j => j.Emails).ThenInclude(j => j.Email)
                .Include(j => j.Phones).ThenInclude(j => j.Phone)

// includes commented because of performance (use typeCache instead)
//                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(j => j.Type)
//                .Include(j => j.Phones).ThenInclude(j => j.Phone).ThenInclude(j => j.ServiceChargeType)
            );
            var serviceChannels = queryWithData.ToList();
            var eChannel = ServiceChannelTypeEnum.EChannel.ToString();
            resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannel, V2VmOpenApiElectronicChannel>(serviceChannels.Where(s => s.Type.Code == eChannel).ToList()));
            var phoneChannel = ServiceChannelTypeEnum.Phone.ToString();
            resultList.AddRange(TranslationManagerToVm.TranslateAll <ServiceChannel, V2VmOpenApiPhoneChannel>(serviceChannels.Where(s => s.Type.Code == phoneChannel).ToList()));
            var serviceLocationChannel = ServiceChannelTypeEnum.ServiceLocation.ToString();
            resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannel, V2VmOpenApiServiceLocationChannel>(serviceChannels.Where(s => s.Type.Code == serviceLocationChannel).ToList()));
            var transactionFormChannel = ServiceChannelTypeEnum.PrintableForm.ToString();
            resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannel, V2VmOpenApiPrintableFormChannel>(serviceChannels.Where(s => s.Type.Code == transactionFormChannel).ToList()));
            var webpageChannel = ServiceChannelTypeEnum.WebPage.ToString();
            resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannel, V2VmOpenApiWebPageChannel>(serviceChannels.Where(s => s.Type.Code == webpageChannel).ToList()));

            return resultList;
        }

        private Guid GetServiceChannelTypeId(ServiceChannelTypeEnum serviceChannelType, IUnitOfWork unitOfWork)
        {
            var typeRep = unitOfWork.CreateRepository<IServiceChannelTypeRepository>();
            var type = serviceChannelType.ToString();
            var serviceChannel = typeRep.All().FirstOrDefault(t => t.Code == type);
            if (serviceChannel == null)
                return Guid.Empty;
            return serviceChannel.Id;
        }

        private TVmChannelOut AddServiceChannel<TVmChannelIn, TVmChannelOut>(TVmChannelIn vm, bool allowAnonymous, string userName = null)
            where TVmChannelOut : class, IVmOpenApiServiceChannel, new()
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            var result = new TVmChannelOut();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var useOtherEndPoint = false;
            var serviceChannel = new ServiceChannel();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<ServiceChannel>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannel>(vm, unitOfWork);
                    var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
                    serviceChannelRep.Add(serviceChannel);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(serviceChannel, vm.SourceId, userId, unitOfWork);
                    }

                    unitOfWork.Save(saveMode, userName: userName);
                    result = TranslationManagerToVm.Translate<ServiceChannel, TVmChannelOut>(serviceChannel);
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }

            // Update the map coordinates for addresses
            if (serviceChannel?.ServiceLocationChannels?.FirstOrDefault() != null)
            {
                var addresses = serviceChannel.ServiceLocationChannels.FirstOrDefault().Addresses?.Select(x => x.AddressId);
                addressService.UpdateAddress(addresses.ToList());
            }
            else if (serviceChannel?.PrintableFormChannels?.FirstOrDefault() != null && serviceChannel.PrintableFormChannels.FirstOrDefault().DeliveryAddressId.HasValue)
            {
                addressService.UpdateAddress(serviceChannel.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select(x => x.DeliveryAddressId.Value).ToList());
            }

            return result;
        }

        private IVmOpenApiServiceChannel SaveServiceChannel<TVmChannelIn>(TVmChannelIn vm, bool allowAnonymous, string userName = null)
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var serviceChannel = new ServiceChannel();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                vm.Id = vm.Id ?? GetPTVId<ServiceChannel>(vm.SourceId, userId, unitOfWork);
                serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannel>(vm, unitOfWork);

                if (vm.Languages != null && vm.Languages.Count> 0)
                {
                    serviceChannel.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Languages,
                            query => query.ServiceChannelId == serviceChannel.Id, language => language.LanguageId);
                }
                if (vm.DeleteAllWebPages || (vm.WebPages != null && vm.WebPages.Count > 0))
                {
                    serviceChannel.WebPages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.WebPages,
                    query => query.ServiceChannelId == serviceChannel.Id, webPage => webPage.WebPage != null ? webPage.WebPage.Id : webPage.WebPageId);
                }
                if (vm.DeleteAllServiceHours || (vm.ServiceHours != null && vm.ServiceHours.Count > 0))
                {
                    // Remove the ones that does not exist in viewmodel list
                    serviceChannel.ServiceHours = dataUtils.UpdateCollectionWithRemove(unitOfWork, serviceChannel.ServiceHours,
                    query => query.ServiceChannelId == serviceChannel.Id);
                }
                if (vm.DeleteAllSupportEmails || (vm.SupportEmails != null && vm.SupportEmails.Count > 0))
                {
                    // Remove the ones that does not exist in viewmodel list
                    var repository = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();
                    var currentEmails = unitOfWork.ApplyIncludes(
                        repository.All().Where(p => p.ServiceChannelId == serviceChannel.Id),
                        p => p.Include(i => i.Email).Include(l => l.Email.Localization)).ToList();

                    var existingLanguages = vm.SupportEmails.Select(e => e.Language).Distinct().ToList();
                    var emailsByLanguageToDelete = currentEmails.Where(e => !existingLanguages.Contains(e.Email.Localization.Code)).ToList();
                    emailsByLanguageToDelete.ForEach(e => repository.Remove(e));

                    // Remove from email
                    var emailRepository = unitOfWork.CreateRepository<IEmailRepository>();
                    emailsByLanguageToDelete.ForEach(e => emailRepository.Remove(e.Email));
                }
                if (vm.DeleteAllSupportPhones || (vm.SupportPhones != null && vm.SupportPhones.Count > 0))
                {
                    // Remove the phone numbers that does not exist in viewmodel list
                    var repository = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                    var currentPhones = unitOfWork.ApplyIncludes(
                        repository.All().Where(p => p.ServiceChannelId == serviceChannel.Id),
                        p => p.Include(i => i.Phone).Include(l => l.Phone.Localization)).ToList();

                    var existingLanguages = vm.SupportPhones.Select(p => p.Language).Distinct().ToList();
                    var phonesByLanguageToDelete = currentPhones.Where(p => !existingLanguages.Contains(p.Phone.Localization.Code)).ToList();
                    phonesByLanguageToDelete.ForEach(p => repository.Remove(p));

                    // Remove from phone
                    var phoneRepository = unitOfWork.CreateRepository<IPhoneRepository>();
                    phonesByLanguageToDelete.ForEach(p => phoneRepository.Remove(p.Phone));
                }

                if (vm is V2VmOpenApiPhoneChannelInBase)
                {
                    SetCollections(vm as V2VmOpenApiPhoneChannelInBase, serviceChannel, unitOfWork);
                }
                else if (vm is V2VmOpenApiWebPageChannelInBase)
                {
                    SetCollections(vm as V2VmOpenApiWebPageChannelInBase, serviceChannel, unitOfWork);
                }
                else if (vm is V2VmOpenApiPrintableFormChannelInBase)
                {
                    SetCollections(vm as V2VmOpenApiPrintableFormChannelInBase, serviceChannel, unitOfWork);
                }
                else if (vm is V2VmOpenApiElectronicChannelInBase)
                {
                    SetCollections(vm as V2VmOpenApiElectronicChannelInBase, serviceChannel, unitOfWork);
                }
                else if (vm is V2VmOpenApiServiceLocationChannelInBase)
                {
                    SetCollections(vm as V2VmOpenApiServiceLocationChannelInBase, serviceChannel, unitOfWork);
                }

                // Update the mapping between external source id and PTV id
                if (!string.IsNullOrEmpty(vm.SourceId))
                {
                    UpdateExternalSource(serviceChannel, vm.SourceId, userId, unitOfWork);
                }

                unitOfWork.Save(saveMode, serviceChannel, userName);
            });

            // Update the map coordinates for addresses
            if (serviceChannel?.ServiceLocationChannels?.FirstOrDefault() != null)
            {
                var addresses = serviceChannel.ServiceLocationChannels.FirstOrDefault().Addresses?.Select(x => x.AddressId);
                addressService.UpdateAddress(addresses.ToList());
            }
            else if (serviceChannel?.PrintableFormChannels?.FirstOrDefault() != null && serviceChannel.PrintableFormChannels.FirstOrDefault().DeliveryAddressId.HasValue)
            {
                addressService.UpdateAddress(serviceChannel.PrintableFormChannels.Where(x => x.DeliveryAddressId.HasValue).Select(x => x.DeliveryAddressId.Value).ToList());
            }

            return V2GetServiceChannel(vm.Id.Value, false);
        }

        private void SetCollections(V2VmOpenApiPhoneChannelInBase vmPhoneChannel, ServiceChannel serviceChannel, IUnitOfWorkWritable unitOfWork)
        {
            if (vmPhoneChannel.PhoneNumbers != null && vmPhoneChannel.PhoneNumbers.Count > 0)
            {
                var repository = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                var currentPhones = unitOfWork.ApplyIncludes(
                    repository.All().Where(p => p.ServiceChannelId == serviceChannel.Id),
                    p => p.Include(i => i.Phone).Include(cht => cht.Phone.ServiceChargeType).Include(l => l.Phone.Localization)).ToList();

                var existingLanguages = vmPhoneChannel.PhoneNumbers.Select(p => p.Language).Distinct().ToList();
                var phonesByLanguageToDelete = currentPhones.Where(p => !existingLanguages.Contains(p.Phone.Localization.Code)).ToList();
                phonesByLanguageToDelete.ForEach(p => repository.Remove(p));
            }
        }

        private void SetCollections(V2VmOpenApiWebPageChannelInBase vmWebPageChannel, ServiceChannel serviceChannel, IUnitOfWorkWritable unitOfWork)
        {
            var webPageChannel = serviceChannel.WebpageChannels.FirstOrDefault();
            if (webPageChannel == null)
            {
                return;
            }

            if (vmWebPageChannel.DeleteAllWebPages || (vmWebPageChannel.Urls != null && vmWebPageChannel.Urls.Count > 0))
            {
                var rep = unitOfWork.CreateRepository<IWebpageChannelUrlRepository>();
                var currentUrls = unitOfWork.ApplyIncludes(rep.All().Where(u => u.WebpageChannelId == webPageChannel.Id), q =>
                    q.Include(i => i.Localization)).ToList();
                var existingLanguages = vmWebPageChannel.Urls != null ? vmWebPageChannel.Urls.Select(u => u.Language).ToList() : new List<string>();
                // Delete items that were not in vm.
                currentUrls.Where(u => !existingLanguages.Contains(u.Localization.Code)).ForEach(u => rep.Remove(u));
            }
        }

        private void SetCollections(V2VmOpenApiPrintableFormChannelInBase vmPrintableFormChannel, ServiceChannel serviceChannel, IUnitOfWorkWritable unitOfWork)
        {
            var printableFormChannel = serviceChannel.PrintableFormChannels.FirstOrDefault();
            if (printableFormChannel == null)
            {
                return;
            }

            if (vmPrintableFormChannel.DeleteAllChannelUrls || (vmPrintableFormChannel.ChannelUrls != null && vmPrintableFormChannel.ChannelUrls.Count > 0))
            {
                printableFormChannel.ChannelUrls = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, printableFormChannel.ChannelUrls,
                    query => query.PrintableFormChannelId == printableFormChannel.Id, url => url.Id);
            }
            if (vmPrintableFormChannel.DeleteDeliveryAddress && vmPrintableFormChannel.DeliveryAddress == null)
            {
                var rep = unitOfWork.CreateRepository<IPrintableFormChannelRepository>();
                var channel = unitOfWork.ApplyIncludes(rep.All().Where(c => c.Id == printableFormChannel.Id), q =>
                    q.Include(i => i.DeliveryAddress)).FirstOrDefault();
                var addressRep = unitOfWork.CreateRepository<IAddressRepository>();
                var address = addressRep.All().Where(i => i.Id == channel.DeliveryAddress.Id).FirstOrDefault();
                if (address != null && channel.DeliveryAddress != null)
                {
                    addressRep.Remove(address);
                }
            }
            if (vmPrintableFormChannel.DeleteAllAttachments || (vmPrintableFormChannel.Attachments != null && vmPrintableFormChannel.Attachments.Count > 0))
            {
                serviceChannel.Attachments = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Attachments,
                    query => query.ServiceChannelId == serviceChannel.Id, attachment => attachment.Attachment != null ? attachment.Attachment.Id : attachment.AttachmentId);
            }
        }

        private void SetCollections(V2VmOpenApiElectronicChannelInBase vmEChannel, ServiceChannel serviceChannel, IUnitOfWorkWritable unitOfWork)
        {
            var eChannel = serviceChannel.ElectronicChannels.FirstOrDefault();
            if (eChannel == null)
            {
                return;
            }
            if (vmEChannel.DeleteAllWebPages || (vmEChannel.Urls != null && vmEChannel.Urls.Count > 0))
            {
                var rep = unitOfWork.CreateRepository<IElectronicChannelUrlRepository>();
                var currentUrls = unitOfWork.ApplyIncludes(rep.All().Where(u => u.ElectronicChannelId == eChannel.Id), q =>
                    q.Include(i => i.Localization)).ToList();
                var existingLanguages = vmEChannel.Urls != null ? vmEChannel.Urls.Select(u => u.Language).ToList() : new List<string>();
                // Delete items that were not in vm.
                currentUrls.Where(u => !existingLanguages.Contains(u.Localization.Code)).ForEach(u => rep.Remove(u));
            }
            if (vmEChannel.DeleteAllAttachments || (vmEChannel.Attachments != null && vmEChannel.Attachments.Count > 0))
            {
                serviceChannel.Attachments = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, serviceChannel.Attachments,
                    query => query.ServiceChannelId == serviceChannel.Id, attachment => attachment.Attachment != null ? attachment.Attachment.Id : attachment.AttachmentId);
            }
        }

        private void SetCollections(V2VmOpenApiServiceLocationChannelInBase vmLocationChannel, ServiceChannel serviceChannel, IUnitOfWorkWritable unitOfWork)
        {
            var locationChannel = serviceChannel.ServiceLocationChannels.FirstOrDefault();
            if (locationChannel == null)
            {
                return;
            }

            if (vmLocationChannel.ServiceAreas != null && vmLocationChannel.ServiceAreas.Count > 0)
            {
                var rep = unitOfWork.CreateRepository<IServiceLocationChannelServiceAreaRepository>();
                var currentAreas = unitOfWork.ApplyIncludes(rep.All().Where(a => a.ServiceLocationChannelId == locationChannel.Id), q =>
                    q.Include(i => i.Municipality)).ToList();
                // Delete items that were not in vm.
                currentAreas.Where(a => !vmLocationChannel.ServiceAreas.Contains(a.Municipality.Code)).ForEach(a => rep.Remove(a));
            }

            if (vmLocationChannel.DeleteAllFaxNumbers || vmLocationChannel.FaxNumbers?.Count > 0)
            {
                RemoveServiceChannelPhoneNumbers(serviceChannel, unitOfWork, typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()));
            }

            if (vmLocationChannel.DeleteAllPhoneNumbers || vmLocationChannel.PhoneNumbers?.Count > 0)
            {
                RemoveServiceChannelPhoneNumbers(serviceChannel, unitOfWork, typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()));
            }

            if (vmLocationChannel.Addresses?.Count > 0)
            {
                locationChannel.Addresses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, locationChannel.Addresses,
                    query => query.ServiceLocationChannelId == locationChannel.Id, address => address.Address != null ? address.Address.Id : address.AddressId);
            }
        }

        private void RemoveServiceChannelPhoneNumbers(ServiceChannel serviceChannel, IUnitOfWorkWritable unitOfWork, Guid typeId)
        {
            var updatedNumbers = serviceChannel.Phones.Select(f => f.PhoneId).ToList();
            var rep = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
            var repPhone = unitOfWork.CreateRepository<IPhoneRepository>();
            var currentNumbers = unitOfWork.ApplyIncludes(rep.All().Where(f => f.ServiceChannelId == serviceChannel.Id && f.Phone.TypeId == typeId), q =>
                q.Include(i => i.Phone)).ToList();
            // Delete items that were in db but not in updated ones
            currentNumbers.Where(f => !updatedNumbers.Contains(f.PhoneId)).ForEach(f => repPhone.Remove(f.Phone));
        }

        private IVmOpenApiElectronicChannel TranslateToVersion1(IV2VmOpenApiElectronicChannel v2Channel)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiElectronicChannel, VmOpenApiElectronicChannel>(v2Channel as V2VmOpenApiElectronicChannel);
        }

        private IVmOpenApiPhoneChannel TranslateToVersion1(IV2VmOpenApiPhoneChannel v2Channel)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiPhoneChannel, VmOpenApiPhoneChannel>(v2Channel as V2VmOpenApiPhoneChannel);
        }

        private IVmOpenApiWebPageChannel TranslateToVersion1(IV2VmOpenApiWebPageChannel v2Channel)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiWebPageChannel, VmOpenApiWebPageChannel>(v2Channel as V2VmOpenApiWebPageChannel);
        }

        private IVmOpenApiServiceLocationChannel TranslateToVersion1(IV2VmOpenApiServiceLocationChannel v2Channel)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiServiceLocationChannel, VmOpenApiServiceLocationChannel>(v2Channel as V2VmOpenApiServiceLocationChannel);
        }

        private IVmOpenApiPrintableFormChannel TranslateToVersion1(IV2VmOpenApiPrintableFormChannel v2Channel)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiPrintableFormChannel, VmOpenApiPrintableFormChannel>(v2Channel as V2VmOpenApiPrintableFormChannel);
        }

        private IVmOpenApiServiceChannel TranslateToVersion1(IVmOpenApiServiceChannel v2Channel)
        {
            // Set v1 view model data
            if (v2Channel is V2VmOpenApiElectronicChannel)
            {
                return TranslateToVersion1(v2Channel as V2VmOpenApiElectronicChannel);
            }
            if (v2Channel is V2VmOpenApiPhoneChannel)
            {
                return TranslateToVersion1(v2Channel as V2VmOpenApiPhoneChannel);
            }
            if (v2Channel is V2VmOpenApiServiceLocationChannel)
            {
                return TranslateToVersion1(v2Channel as V2VmOpenApiServiceLocationChannel);
            }
            if (v2Channel is V2VmOpenApiPrintableFormChannel)
            {
                return TranslateToVersion1(v2Channel as V2VmOpenApiPrintableFormChannel);
            }
            if (v2Channel is V2VmOpenApiWebPageChannel)
            {
                return TranslationManagerToVm.Translate<V2VmOpenApiWebPageChannel, VmOpenApiWebPageChannel>(v2Channel as V2VmOpenApiWebPageChannel);
            }

            return v2Channel;
        }

        private IList<IVmOpenApiServiceChannel> TranslateToVersion1(IList<IVmOpenApiServiceChannel> v2Channels)
        {
            IList<IVmOpenApiServiceChannel> result = new List<IVmOpenApiServiceChannel>();

            // Set v1 view model data
            v2Channels.ForEach(channel => result.Add(TranslateToVersion1(channel)));

            return result;
        }

        #endregion

        private IEnumerable<Guid> PrefilterServiceChannelIds(List<VmServiceChannelRelation> serviceChannelRelations)
        {
            var connectedChannelsIds = new List<List<Guid>>();
            foreach (var service in serviceChannelRelations)
            {
                connectedChannelsIds.Add(service.ChannelRelations.Where(x => !x.isNew)
                                                                 .Select(y => y.ConnectedChannel.Id)
                                                                 .ToList()
                                        );
            }

            return connectedChannelsIds.Any() ? connectedChannelsIds.Cast<IEnumerable<Guid>>().Aggregate((x, y) => x.Intersect(y)) : new List<Guid>();
        }

        private List<VmListItem> GetMunicipalities(IUnitOfWork unitOfWork)
        {
            var municipalityRep = unitOfWork.CreateRepository<IMunicipalityRepository>();
            return TranslationManagerToVm.TranslateAll<Municipality, VmListItem>(municipalityRep.All().OrderBy(x => x.Name)).ToList();
        }

        public IVmEntityBase PublishChannel(Guid? entityId)
        {
            ServiceChannel result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = PublishChannel(unitOfWork, entityId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = result.PublishingStatusId };
        }

        private ServiceChannel PublishChannel(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Published.ToString(), unitOfWork);

            var channelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            var channel = channelRep.All().Single(x => x.Id == entityId.Value);
            channel.PublishingStatus = publishStatus;
            return channel;
        }

        public List<IVmEntityBase> PublishChannels(List<Guid> channelIds)
        {
            var result = new List<IVmEntityBase>();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                foreach (var id in channelIds)
                {
                    var channel = PublishChannel(unitOfWork, id);
                    result.Add(new VmEntityStatusBase { Id = channel.Id, PublishingStatus = channel.PublishingStatus.Id });
                }
                unitOfWork.Save();

            });
            return result;
        }

        public IVmEntityBase DeleteChannel(Guid? entityId)
        {
            ServiceChannel result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteChannel(unitOfWork, entityId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = result.PublishingStatusId };
        }

        private ServiceChannel DeleteChannel(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);
            var channelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            var channel = channelRep.All().Single(x => x.Id == entityId.Value);
            channel.PublishingStatus = publishStatus;
            return channel;
        }

        public IVmEntityBase GetChannelStatus(Guid? entityId)
        {
            VmPublishingStatus result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = GetChannelStatus(unitOfWork, entityId);
            });
            return new VmEntityStatusBase() { PublishingStatus = result.Id };
        }

        private VmPublishingStatus GetChannelStatus(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var channelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            var channel = channelRep.All()
                            .Include(x => x.PublishingStatus)
                            .Single(x => x.Id == entityId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(channel.PublishingStatus);
        }

        /// <summary>
        /// Checks if a service channel with given identifier exists in the system.
        /// </summary>
        /// <param name="channelId">guid of the channe</param>
        /// <returns>true if a channel exists otherwise false</returns>
        public bool ChannelExists(Guid channelId)
        {
            bool chExists = false;

            if (Guid.Empty == channelId)
            {
                return chExists;
            }

            contextManager.ExecuteReader(unitOfWork =>
            {
                var chRepo = unitOfWork.CreateRepository<IServiceChannelRepository>().All();

                if (chRepo.FirstOrDefault(s => s.Id.Equals(channelId)) != null)
                {
                    chExists = true;
                }
            });

            return chExists;
        }
    }
}
