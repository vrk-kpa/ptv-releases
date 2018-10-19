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
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using IChannelService = PTV.Database.DataAccess.Interfaces.Services.V2.IChannelService;
using VmElectronicChannel = PTV.Domain.Model.Models.V2.Channel.VmElectronicChannel;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Enums.Security;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IChannelService), RegisterType.Transient)]
    [Framework.RegisterService(typeof(IChannelServiceInternal), RegisterType.Transient)]
    internal class ChannelService : EntityServiceBase<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>, IChannelServiceInternal
    {
        private readonly IUserIdentification userIdentification;
        private ILogger logger;
        private const string invalidElectronicChannelUrl = "Electronic channel url '{0}'";
        private const string invalidWebPageChannelUrl = "Web page channel url '{0}'";
        private const string invalidElectronicChannelAttachmentUrl = "Electronic channel attachment url '{0}'";
        private ServiceChannelLogic channelLogic;
        private VmListItemLogic listItemLogic;
        private readonly DataUtils dataUtils;
        private VmOwnerReferenceLogic ownerReferenceLogic;
        //private IAddressService addressService;
        //private IUrlService urlService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;
        private IUserOrganizationService userOrganizationService;
        private ServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private IConnectionsServiceInternal connectionsService;
        private ILanguageOrderCache languageOrderCache;
        private ITranslationService translationService;
        private readonly IAccessibilityRegisterService accessibilityRegisterService;
        private ITasksServiceInternal tasksService;
        private ServiceUtilities serviceUtilities;
        
        public ICollection<ServiceChannelLanguageAvailability> LanguageAvailabilities
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ChannelService(
            IContextManager contextManager,
            IUserIdentification userIdentification,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<Services.ChannelService> logger,
            ServiceChannelLogic channelLogic,
            ServiceUtilities utilities,
            ICommonServiceInternal commonService,
            VmListItemLogic listItemLogic,
            DataUtils dataUtils,
            VmOwnerReferenceLogic ownerReferenceLogic,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IValidationManager validationManager,
            IUserOrganizationService userOrganizationService,
            ITranslationService translationService,
            IVersioningManager versioningManager,
            IConnectionsServiceInternal connectionsService,
            IAccessibilityRegisterService accessibilityRegisterService,
            ITasksServiceInternal tasksService,
            ServiceUtilities serviceUtilities
            ) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, contextManager, utilities, commonService, validationManager)
        {
            this.logger = logger;
            this.channelLogic = channelLogic;
            this.userIdentification = userIdentification;
            this.listItemLogic = listItemLogic;
            this.dataUtils = dataUtils;
            this.ownerReferenceLogic = ownerReferenceLogic;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.userOrganizationService = userOrganizationService;
            this.versioningManager = versioningManager;
            this.utilities = utilities;
            this.commonService = commonService;
            this.connectionsService = connectionsService;
            this.translationService = translationService;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.accessibilityRegisterService = accessibilityRegisterService;
            this.tasksService = tasksService;
            this.serviceUtilities = serviceUtilities;
        }

        #region Electronic
        public VmElectronicChannel GetElectronicChannel(IVmEntityGet model)
        {
            return ExecuteGet(model, GetElectronicChannel);
        }

        private void UpdateExpiration(IUnitOfWork unitOfWork, VmEntityHeaderBase result)
        {
            var expirationInformation =
                tasksService.GetExpirationInformation<ServiceChannelVersioned>(unitOfWork, result.UnificRootId, result.PublishingStatus);

            if (expirationInformation == null || !expirationInformation.ContainsKey(result.UnificRootId)) return;
            
            result.ExpireOn = expirationInformation[result.UnificRootId].ExpireOn.ToEpochTime();
            result.IsWarningVisible = expirationInformation[result.UnificRootId].IsWarningVisible;
        }
        
        private VmElectronicChannel GetElectronicChannel(IUnitOfWork unitOfWork, IVmEntityGet model)
        {
            VmElectronicChannel result;
            //                SetTranslatorLanguage(model);
            ServiceChannelVersioned entity = null;
            result = GetModel<ServiceChannelVersioned, VmElectronicChannel>(entity = GetEntity<ServiceChannelVersioned>(
                model.Id, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames)
                    .Include(x => x.ServiceChannelDescriptions)
                    .Include(x => x.Versioning)
                    .Include(x => x.PublishingStatus)
                    .Include(x => x.AreaMunicipalities)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Areas)
                        .ThenInclude(x => x.Area)
                    .Include(x => x.Attachments)
                        .ThenInclude(x => x.Attachment)
                    .Include(x => x.ElectronicChannels)
                        .ThenInclude(x => x.LocalizedUrls)
                    .Include(i => i.ServiceChannelServiceHours)
                        .ThenInclude(i => i.ServiceHours)
                        .ThenInclude(i => i.DailyOpeningTimes)
                    .Include(i => i.ServiceChannelServiceHours)
                        .ThenInclude(i => i.ServiceHours)
                        .ThenInclude(i => i.AdditionalInformations)
                    .Include(x => x.Phones)
                        .ThenInclude(x => x.Phone)
                        .ThenInclude(x => x.PrefixNumber)
                        .ThenInclude(x => x.Country)
                        .ThenInclude(x => x.CountryNames)
                    .Include(x => x.Emails)
                    .ThenInclude(x => x.Email)
                    .Include(x => x.ConnectionType)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations)
                        .ThenInclude(j => j.DigitalAuthorization)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypes)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions)
            ), unitOfWork);
            UpdateExpiration(unitOfWork, result);
            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            return result;
        }


        public VmElectronicChannel SaveElectronicChannel(VmElectronicChannel model)
        {
            return ExecuteSave(
                unitOfWork => SaveElectronicChannel(unitOfWork, model),
                (unitOfWork, entity) => GetElectronicChannel(unitOfWork, new VmChannelBasic { Id = entity.Id })
            );
        }

        private ServiceChannelVersioned SaveElectronicChannel(IUnitOfWorkWritable unitOfWork, VmElectronicChannel vm)
        {
            //            SetTranslatorLanguage(vm);
            //            channelLogic.PrefilterViewModel(vm);
            //            vm.PublishingStatusId = commonService.GetDraftStatusId();
            CheckAndConfirmChannelDeliveredTranslation(unitOfWork, vm.Id, vm.LanguagesAvailabilities);
            var serviceChannel = TranslationManagerToEntity.Translate<VmElectronicChannel, ServiceChannelVersioned>(vm, unitOfWork);
            commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel);
            return serviceChannel;
        }
        #endregion

        #region Web page
        public Domain.Model.Models.V2.Channel.VmWebPageChannel GetWebPageChannel(IVmEntityGet model)
        {
            return ExecuteGet(model, GetWebPageChannel);
        }

        private VmWebPageChannel GetWebPageChannel(IUnitOfWork unitOfWork, IVmEntityGet model)
        {
            ServiceChannelVersioned entity = null;
            Domain.Model.Models.V2.Channel.VmWebPageChannel result = null;
            result = GetModel<ServiceChannelVersioned, Domain.Model.Models.V2.Channel.VmWebPageChannel>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(j => j.ServiceChannelDescriptions)
                    .Include(j => j.ServiceChannelNames)
                    .Include(j => j.WebpageChannels).ThenInclude(j => j.LocalizedUrls)
                    .Include(j => j.Languages).ThenInclude(j => j.Language)
                    .Include(x => x.Emails).ThenInclude(x => x.Email)
                    .Include(x => x.Phones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations)
                        .ThenInclude(j => j.DigitalAuthorization)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypes)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions)

            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            UpdateExpiration(unitOfWork, result);
            return result;
        }

        public Domain.Model.Models.V2.Channel.VmWebPageChannel SaveWebPageChannel(Domain.Model.Models.V2.Channel.VmWebPageChannel model)
        {
            return ExecuteSave(
                unitOfWork => SaveWebPageChannel(unitOfWork, model),
                (unitOfWork, entity) => GetWebPageChannel(unitOfWork, new VmChannelBasic { Id = entity.Id })
            );
        }

        private ServiceChannelVersioned SaveWebPageChannel(IUnitOfWorkWritable unitOfWork, Domain.Model.Models.V2.Channel.VmWebPageChannel vm)
        {
            //            SetTranslatorLanguage(vm);
            //            channelLogic.PrefilterViewModel(vm);
            //            vm.PublishingStatusId = commonService.GetDraftStatusId();
            CheckAndConfirmChannelDeliveredTranslation(unitOfWork, vm.Id, vm.LanguagesAvailabilities);
            var serviceChannel = TranslationManagerToEntity.Translate<Domain.Model.Models.V2.Channel.VmWebPageChannel, ServiceChannelVersioned>(vm, unitOfWork);
            commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel);

            return serviceChannel;
        }
        #endregion

        #region PrintableForm
        public VmPrintableFormOutput GetPrintableFormChannel(IVmEntityGet vm)
        {
            return ExecuteGet(vm, GetPrintableFormChannel);
        }
        private VmPrintableFormOutput GetPrintableFormChannel(IUnitOfWork unitOfWork, IVmEntityGet vm)
        {
            var result = new VmPrintableFormOutput();
            ServiceChannelVersioned entity = null;
            result = GetModel<ServiceChannelVersioned, VmPrintableFormOutput>(entity = GetEntity<ServiceChannelVersioned>(vm.Id, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames)
                    .Include(x => x.Versioning)
                    .Include(x => x.ConnectionType)
                    .Include(x => x.ServiceChannelDescriptions)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Emails)
                        .ThenInclude(x => x.Email)
                    .Include(x => x.Phones)
                        .ThenInclude(x => x.Phone)
                        .ThenInclude(x => x.PrefixNumber)
                        .ThenInclude(x => x.Country)
                        .ThenInclude(x => x.CountryNames)
                    .Include(x => x.PublishingStatus)
                    .Include(x => x.AreaMunicipalities)
                    .Include(x => x.Areas).ThenInclude(x => x.Area)
                    .Include(x => x.Addresses).ThenInclude(i => i.Character)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                    .Include(x => x.Addresses).ThenInclude(i => i.Address).ThenInclude(i => i.Receivers)
                    .Include(x => x.Attachments)
                        .ThenInclude(x => x.Attachment)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.ChannelUrls)
                        .ThenInclude(x => x.Type)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.FormIdentifiers)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations)
                        .ThenInclude(j => j.DigitalAuthorization)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypes)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions)
            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            UpdateExpiration(unitOfWork, result);
            return result;
        }
        public VmPrintableFormOutput SavePrintableFormChannel(VmPrintableFormInput model)
        {
            return ExecuteSave(
                unitOfWork => SavePrintableFormChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPrintableFormChannel(unitOfWork, new VmChannelBasic { Id = entity.Id })
            );
        }

        private ServiceChannelVersioned SavePrintableFormChannel(IUnitOfWorkWritable unitOfWork, VmPrintableFormInput vm)
        {
            CheckAndConfirmChannelDeliveredTranslation(unitOfWork, vm.Id, vm.LanguagesAvailabilities);
            var serviceChannel = TranslationManagerToEntity.Translate<VmPrintableFormInput, ServiceChannelVersioned>(vm, unitOfWork);
            commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel);

            return serviceChannel;
        }
        #endregion

        #region Phone
        public Domain.Model.Models.V2.Channel.VmPhoneChannel GetPhoneChannel(IVmEntityGet model)
        {
            return ExecuteGet(model, GetPhoneChannel);
        }

        private VmPhoneChannel GetPhoneChannel(IUnitOfWork unitOfWork, IVmEntityGet model)
        {
            ServiceChannelVersioned entity = null;
            Domain.Model.Models.V2.Channel.VmPhoneChannel result = null;
            result = GetModel<ServiceChannelVersioned, Domain.Model.Models.V2.Channel.VmPhoneChannel>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(x => x.PublishingStatus)
                    .Include(j => j.ServiceChannelDescriptions)
                    .Include(j => j.ServiceChannelNames)
                    .Include(x => x.AreaMunicipalities)
                    .Include(x => x.Areas).ThenInclude(x => x.Area)
                    .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                    .Include(j => j.Languages).ThenInclude(j => j.Language)
                    .Include(x => x.Emails).ThenInclude(x => x.Email)
                    .Include(x => x.Phones)
                        .ThenInclude(x => x.Phone)
                        .ThenInclude(x => x.PrefixNumber)
                        .ThenInclude(x => x.Country)
                        .ThenInclude(x => x.CountryNames)
                    .Include(i => i.ServiceChannelServiceHours)
                        .ThenInclude(i => i.ServiceHours)
                        .ThenInclude(i => i.DailyOpeningTimes)
                    .Include(i => i.ServiceChannelServiceHours)
                        .ThenInclude(i => i.ServiceHours)
                        .ThenInclude(i => i.AdditionalInformations)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations)
                        .ThenInclude(j => j.DigitalAuthorization)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypes)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions)
            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            UpdateExpiration(unitOfWork, result);
            return result;
        }

        public Domain.Model.Models.V2.Channel.VmPhoneChannel SavePhoneChannel(Domain.Model.Models.V2.Channel.VmPhoneChannel model)
        {
            return ExecuteSave(
                unitOfWork => SavePhoneChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPhoneChannel(unitOfWork, new VmChannelBasic { Id = entity.Id })
            );
        }

        private ServiceChannelVersioned SavePhoneChannel(IUnitOfWorkWritable unitOfWork, Domain.Model.Models.V2.Channel.VmPhoneChannel vm)
        {
            //            SetTranslatorLanguage(vm);
            //            channelLogic.PrefilterViewModel(vm);
            //            vm.PublishingStatusId = commonService.GetDraftStatusId();
            CheckAndConfirmChannelDeliveredTranslation(unitOfWork, vm.Id, vm.LanguagesAvailabilities);
            var serviceChannel = TranslationManagerToEntity.Translate<Domain.Model.Models.V2.Channel.VmPhoneChannel, ServiceChannelVersioned>(vm, unitOfWork);
            commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel);

            return serviceChannel;
        }
        #endregion

        #region Service location
        public VmServiceLocationChannel GetServiceLocationChannel(IVmEntityGet model)
        {
            return ExecuteGet(model, GetServiceLocationChannel);
        }

        public VmAccessibilityRegisterSetOut SetServiceLocationChannelAccessibility(VmAccessibilityRegisterSetIn model)
        {
            return ContextManager.ExecuteWriter(unitOfWork => accessibilityRegisterService.SetAccessibilityRegister(unitOfWork, model));
        }

        public VmServiceLocationChannel LoadServiceLocationChannelAccessibility(VmAccessibilityRegisterLoad model)
        {
            ContextManager.ExecuteWriter(unitOfWork => { accessibilityRegisterService.LoadAccessibilityRegister(unitOfWork, model.AccessibilityRegisterId); });
            return ContextManager.ExecuteReader(unitOfWork => GetServiceLocationChannel(unitOfWork, new VmChannelBasic {Id = model.ServiceChannelVersionedId}));
        }

        private VmServiceLocationChannel GetServiceLocationChannel(IUnitOfWork unitOfWork, IVmEntityGet model)
        {
            ServiceChannelVersioned entity = null;
            VmServiceLocationChannel result = null;
            result = GetModel<ServiceChannelVersioned, VmServiceLocationChannel>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(x => x.PublishingStatus)
                    .Include(j => j.ServiceChannelDescriptions)
                    .Include(j => j.ServiceChannelNames)
                    .Include(x => x.AreaMunicipalities)
                    .Include(x => x.Areas).ThenInclude(x => x.Area)
                    .Include(j => j.WebPages).ThenInclude(j => j.WebPage)
                    .Include(j => j.Languages)
                    .Include(x => x.Emails).ThenInclude(x => x.Email)
                    .Include(x => x.Phones)
                        .ThenInclude(x => x.Phone)
                        .ThenInclude(x => x.PrefixNumber)
                        .ThenInclude(x => x.Country)
                        .ThenInclude(x => x.CountryNames)
                    .Include(i => i.ServiceChannelServiceHours)
                        .ThenInclude(i => i.ServiceHours)
                        .ThenInclude(i => i.DailyOpeningTimes)
                    .Include(i => i.ServiceChannelServiceHours)
                        .ThenInclude(i => i.ServiceHours)
                        .ThenInclude(i => i.AdditionalInformations)

                    .Include(x => x.Addresses).ThenInclude(x => x.Character)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressOthers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.Coordinates)
                    .Include(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                    //.Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Accessibilities)//.ThenInclude(x => x.Accessibility)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations)
                        .ThenInclude(j => j.DigitalAuthorization)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypes)
                        .ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.SocialHealthCenters)

                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Address)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Values)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences).ThenInclude(i => i.Values)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
//                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.AddressOthers).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.AccessibilityRegisters).ThenInclude(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                    
                    .Include(x => x.DisplayNameTypes)

            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            AddAccessibilityRegisterInfo(result, entity.UnificRoot?.AccessibilityRegisters.SingleOrDefault(), unitOfWork);
            UpdateExpiration(unitOfWork, result);
            return result;
        }

        public VmServiceLocationChannel SaveServiceLocationChannel(VmServiceLocationChannel model)
        {
            return ExecuteSave(
                unitOfWork => SaveServiceLocationChannel(unitOfWork, model),
                (unitOfWork, entity) => GetServiceLocationChannel(unitOfWork, new VmChannelBasic { Id = entity.Id })
            );
        }

        private ServiceChannelVersioned SaveServiceLocationChannel(IUnitOfWorkWritable unitOfWork, VmServiceLocationChannel vm)
        {
            //            SetTranslatorLanguage(vm);
            //            channelLogic.PrefilterViewModel(vm);
            //            vm.PublishingStatusId = commonService.GetDraftStatusId();
            CheckAddressTypes(vm.VisitingAddresses);
            CheckAndConfirmChannelDeliveredTranslation(unitOfWork, vm.Id, vm.LanguagesAvailabilities);
            accessibilityRegisterService.HandleAccessibilityRegisterSave(unitOfWork, vm);
            var serviceChannel = TranslationManagerToEntity.Translate<VmServiceLocationChannel, ServiceChannelVersioned>(vm, unitOfWork);
            HandleSocialHealthCenter(serviceChannel.UnificRootId, vm, unitOfWork);
            commonService.AddHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel);
            return serviceChannel;
        }
        #endregion

        public VmEntityHeaderBase PublishChannel( IVmLocalizedEntityModel model)
        {
            if (!model.Id.IsAssigned()) return null; 
            Guid? channelId = model.Id;
            var affected = ContextManager.ExecuteWriter(unitOfWork =>
            {
                //Validate mandatory values
                var validationMessages = ValidationManager.CheckEntity<ServiceChannelVersioned>(channelId.Value, unitOfWork, model);
                if (validationMessages.Any())
                {
                    throw new PtvValidationException(validationMessages, null);
                }

                //Publishing
                var result = CommonService.PublishEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model);
                RemoveNotCommonConnections(new List<Guid>{result.Id}, unitOfWork);                
                unitOfWork.Save();
                return result;
            });
            
            return ContextManager.ExecuteReader(unitOfWork => GetChannelHeader(affected.Id, unitOfWork));
        }
        public void RemoveNotCommonConnections(IEnumerable<Guid> versionedIds, IUnitOfWorkWritable unitOfWork)
        {
            var notCommonType = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            
            var connectionRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            
            var channels = serviceChannelRep.All()
                .Where(x => versionedIds.Contains(x.Id) && x.ConnectionTypeId == notCommonType);
            var channelUnificRootIds = channels.Select(x => x.UnificRootId);
            
            var connections = connectionRep.All().Where(x => channelUnificRootIds.Contains(x.ServiceChannelId));
            
            var serviceUnificRootIds = connections.Select(x => x.ServiceId);
            var services = serviceRep.All().Where(x => serviceUnificRootIds.Contains(x.UnificRootId)).GroupBy(x => x.UnificRootId).Select(x => x.OrderBy(y => y.PublishingStatusId == publishingStatusId ? 0 : y.PublishingStatusId == draftStatusId ? 1 : y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault());
  
            connections.ForEach(connection =>
            {
                var channel = channels.First(x => x.UnificRootId == connection.ServiceChannelId);
                var service = services.First(x => x.UnificRootId == connection.ServiceId);
                if (service.OrganizationId != channel.OrganizationId)
                {
                    connectionRep.Remove(connection);
                }
            });               
        }

        public VmEntityHeaderBase ScheduleChannel(IVmLocalizedEntityModel model)
        {
            if (!model.Id.IsAssigned()) return null; 
            Guid? channelId = model.Id;

            var result = ContextManager.ExecuteWriter(unitOfWork =>
            {
                //Validate mandatory values
                if (model.PublishAction == PublishActionTypeEnum.SchedulePublish)
                {
                    var validationMessages =
                        ValidationManager.CheckEntity<ServiceChannelVersioned>(channelId.Value, unitOfWork, model);
                    if (validationMessages.Any())
                    {
                        throw new SchedulePublishException();
                    }
                }

                //Schedule publish/archive
                return CommonService.SchedulePublishArchiveEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model);
            });
            return ContextManager.ExecuteReader(unitOfWork => GetChannelHeader(result.Id, unitOfWork));
        }

        public VmChannelHeader GetChannelHeader(Guid? channelId)
        {
            var result = new VmChannelHeader();
            ContextManager.ExecuteReader(unitOfWork =>
            {
                result = GetChannelHeader(channelId, unitOfWork);
            });
            return result;
        }

        public VmChannelHeader GetChannelHeader(Guid? channelId, IUnitOfWork unitOfWork)
        {
            var result = new VmChannelHeader();
            ServiceChannelVersioned entity = null;
            result = GetModel<ServiceChannelVersioned, VmChannelHeader>(entity = GetEntity<ServiceChannelVersioned>(channelId, unitOfWork,
                q => q
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.ServiceChannelNames)
                    .Include(x => x.Versioning)
                    .Include(x => x.UnificRoot).ThenInclude(x => x.SocialHealthCenters)
            ), unitOfWork);
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var channelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var numberOfConnectedServices = channelRep.All()
                .Count(x =>
                    x.ServiceChannelId == entity.UnificRootId &&
                    x.Service.Versions.Any(o =>
                        o.PublishingStatusId == draftStatusId ||
                        o.PublishingStatusId == publishingStatusId ||
                        o.PublishingStatusId == modifiedStatusId
                    )
                );
            result.NumberOfConnections = numberOfConnectedServices;
            result.PreviousInfo = channelId.IsAssigned() ? Utilities.CheckIsEntityEditable<ServiceChannelVersioned, ServiceChannel>(channelId.Value, unitOfWork) : null;

            if (channelId.HasValue)
            {
                var unificRootId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, channelId.Value);
                if (unificRootId.HasValue)
                {
                    var relations = connectionsService.GetAllServiceChannelRelations(unitOfWork, new List<Guid> { unificRootId.Value });
                    var connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                        relations.TryGetOrDefault(unificRootId.Value, new List<ServiceServiceChannel>()).OrderBy(x => x.OrderNumber)).ToList();

                    result.TranslationAvailability = translationService.GetChannelTranslationAvailabilities(unitOfWork, channelId.Value, unificRootId.Value, connections);
                }
            }

            return result;
        }

        public VmChannelHeader DeleteChannel(Guid entityId)
        {
            return ExecuteDelete(entityId, GetChannelHeader, unitOfWork => OnDeletingChannel(unitOfWork, entityId));
//            VmChannelHeader result = null;
//            ContextManager.ExecuteWriter(unitOfWork =>
//            {
//                var deletedChannel = DeleteChannel(unitOfWork, entityId);
//                unitOfWork.Save();
//                result = GetChannelHeader(deletedChannel.Id, unitOfWork);
//            });
//            UnLockChannel(result.Id.Value);
//            return result;
        }

        private void OnDeletingChannel(IUnitOfWorkWritable unitOfWork, Guid entityId)
        {
            DeleteServiceChannelConnections(unitOfWork, entityId);
//            return CommonService.ChangeEntityToDeleted<ServiceChannelVersioned>(unitOfWork, entityId.Value);
        }

        public IVmEntityBase LockChannel(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<ServiceChannelVersioned, ServiceChannel>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockChannel(Guid id)
        {
            return Utilities.UnLockEntityVersioned<ServiceChannelVersioned, ServiceChannel>(id);
        }

        public VmChannelHeader WithdrawChannel(Guid channelId)
        {
            return ExecuteWithdraw(channelId, GetChannelHeader);
//            var result = CommonService.WithdrawEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(channelId);
//            UnLockChannel(result.Id.Value);
//            return GetChannelHeader(result.Id);
        }

        public VmChannelHeader RestoreChannel(Guid channelId)
        {
            return ExecuteRestore(channelId, GetChannelHeader);
//            var result = CommonService.RestoreEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(channelId);
//            UnLockChannel(result.Id.Value);
//            return GetChannelHeader(result.Id);
        }

        public VmChannelHeader ArchiveLanguage(VmEntityBasic model)
        {
            return ExecuteArchiveLanguage(model, GetChannelHeader);
//            var entity = CommonService.ArchiveLanguage<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(model);
//            UnLockChannel(entity.Id);
//            return GetChannelHeader(entity.Id);
        }

        public VmChannelHeader RestoreLanguage(VmEntityBasic model)
        {
            return ExecuteRestoreLanguage(model, GetChannelHeader);
//            var entity = CommonService.RestoreLanguage<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(model);
//            UnLockChannel(entity.Id);
//            return GetChannelHeader(entity.Id);
        }
        public VmChannelHeader WithdrawLanguage(VmEntityBasic model)
        {
            return ExecuteWithdrawLanguage(model, GetChannelHeader);
//            var entity = CommonService.WithdrawLanguage<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(model);
//            UnLockChannel(entity.Id);
//            return GetChannelHeader(entity.Id);
        }

        public VmChannelHeader GetValidatedEntity(VmEntityBasic model)
        {
            return ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<ServiceChannelVersioned, ServiceChannel>(model.Id.Value, true),
                unitOfWork => GetChannelHeader(model.Id, unitOfWork)
            );
        }

        private void AddAdditionalInfo(VmChannelHeader result, IUnitOfWork unitOfWork)
        {
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var channelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var numberOfConnectedServices = channelRep.All()
                .Where(x =>
                    x.ServiceChannelId == result.UnificRootId &&
                    x.Service.Versions.Any(o =>
                        o.PublishingStatusId == draftStatusId ||
                        o.PublishingStatusId == publishingStatusId ||
                        o.PublishingStatusId == modifiedStatusId
                    )
                ).Count();
            result.NumberOfConnections = numberOfConnectedServices;
            result.PreviousInfo = result.Id.IsAssigned() ? Utilities.CheckIsEntityEditable<ServiceChannelVersioned, ServiceChannel>(result.Id.Value, unitOfWork) : null;
        }

        private void AddConnectionsInfo(PTV.Domain.Model.Models.V2.Channel.VmServiceChannel result, ICollection<ServiceServiceChannel> connections, IUnitOfWork unitOfWork)
        {
            //var channelUnificId = result.UnificRootId;
            //var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            //var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            //var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            //var connectedServicesUnificIds = result.ConnectedServicesUnific.Select(i => i.ServiceId).ToList();
            //var channelRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            //var servicesWithGeneralDescription = new List<ServiceVersioned>();
            //var connectedChannels = channelRep.All().Where(i =>
            //        (connectedServicesUnificIds.Contains(i.UnificRootId)) &&
            //        (i.PublishingStatusId == psPublished || i.PublishingStatusId == psDraft || i.PublishingStatusId == psModified))
            //    .Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities)
            //    .ToList()
            //    // could be done on db when it will be implemented in EF
            //    .GroupBy(i => i.UnificRootId)
            //    .Select(i => i.FirstOrDefault(x => x.PublishingStatusId == psPublished) ?? i.FirstOrDefault())
            //    .Select(i =>
            //    {
            //        if (i.StatutoryServiceGeneralDescriptionId.IsAssigned())
            //        {
            //            servicesWithGeneralDescription.Add(i);
            //        }
            //        var modifiedData = result.ConnectedServicesUnific.FirstOrDefault(m => m.ServiceId == i.UnificRootId);
            //        var originalConnection =
            //            connections.FirstOrDefault(x => x.ServiceId == i.UnificRootId &&
            //                                            x.ServiceChannelId == channelUnificId);
            //        return new ServiceServiceChannel()
            //        {
            //            ServiceId = i.UnificRootId,
            //            ServiceChannelId = channelUnificId,
            //            ServiceServiceChannelDescriptions = originalConnection?.ServiceServiceChannelDescriptions ?? new List<ServiceServiceChannelDescription>(),
            //            ServiceServiceChannelDigitalAuthorizations = originalConnection?.ServiceServiceChannelDigitalAuthorizations ?? new List<ServiceServiceChannelDigitalAuthorization>(),
            //            IsASTIConnection = originalConnection?.IsASTIConnection ?? false,
            //            ServiceServiceChannelExtraTypes = originalConnection?.ServiceServiceChannelExtraTypes ?? new List<ServiceServiceChannelExtraType>(),
            //            ChargeTypeId = originalConnection?.ChargeTypeId,

            //            Service = new Service() { Id = i.UnificRootId, Versions = new List<ServiceVersioned>() { i } },
            //            Modified = modifiedData?.Modified ?? DateTime.MinValue,
            //            ModifiedBy = modifiedData?.ModifiedBy ?? string.Empty
            //        };
            //    }).ToList();
            //var generalDescriptionsToLoad = servicesWithGeneralDescription.Select(i => i.StatutoryServiceGeneralDescriptionId.Value).ToList();
            //if (generalDescriptionsToLoad.Any())
            //{
            //    var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            //    var generalDescriptions = generalDescriptionRep.All().Where(i =>
            //        generalDescriptionsToLoad.Contains(i.UnificRootId) &&
            //        (i.PublishingStatusId == psPublished || i.PublishingStatusId == psDraft || i.PublishingStatusId == psModified)).ToList()
            //        .GroupBy(i => i.UnificRootId)
            //        .Select(i => i.FirstOrDefault(x => x.PublishingStatusId == psPublished) ?? i.FirstOrDefault())
            //        .ToDictionary(i => i.UnificRootId, i => i);
            //    servicesWithGeneralDescription.ForEach(i =>
            //    {
            //        var gd = generalDescriptions.TryGetOrDefault(i.StatutoryServiceGeneralDescriptionId.Value, null);
            //        i.StatutoryServiceGeneralDescription = new StatutoryServiceGeneralDescription() { Versions = new List<StatutoryServiceGeneralDescriptionVersioned>() { gd } };
            //        i.TypeId = gd?.TypeId;
            //    });
            //}

            var relations = connectionsService.GetAllServiceChannelRelations(unitOfWork, new List<Guid> { result.UnificRootId });
            result.Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                            relations.TryGetOrDefault(result.UnificRootId, new List<ServiceServiceChannel>()).OrderBy(x => x.OrderNumber)).ToList();
            //result.Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(connectedChannels).InclusiveToList();

            result.NumberOfConnections = result.Connections.Count;
            result.TranslationAvailability = translationService.GetChannelTranslationAvailabilities(unitOfWork, result.Id.Value, result.UnificRootId, result.Connections);
            
            FillEnumEntities(result, () =>
            {
                var ids = new List<Guid>(result.Connections.Where(c => c.OrganizationId.HasValue).Select(c => c.OrganizationId.Value))
                {
                    result.OrganizationId
                };
                return GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizations(ids.Distinct()));
            });

        }

        private void AddAccessibilityRegisterInfo(VmServiceLocationChannel result, AccessibilityRegister accessibilityRegister, IUnitOfWork unitOfWork)
        {
            if (result.VisitingAddresses.IsNullOrEmpty()) return;
            var accessibilityIsSet = accessibilityRegister != null && !accessibilityRegister.Entrances.IsNullOrEmpty();
            
            // AR has not been set yet => 
            if (accessibilityRegister == null || !accessibilityIsSet)
            {
                result.AccessibilityMeta = null;
                //result.VisitingAddresses.ForEach(a => a.AccessibilityRegister = null);
            }

            // AR has been set, but not loaded
            if (accessibilityRegister != null && !accessibilityIsSet)
            {
                var visitingAddress = result.VisitingAddresses.SingleOrDefault(a => a.UniqueId == accessibilityRegister.Address.UniqueId);
                if (visitingAddress == null) return;

                visitingAddress.AccessibilityRegister = new VmAccessibilityRegisterUI
                {
                    Id = accessibilityRegister.Id,
                    Url = accessibilityRegister.Url,
                    SetAt = accessibilityRegister.Modified.ToEpochTime(),
                    Groups = null,
                    IsValid = false,
                    IsMainEntrance = true // SFIPTV-514
                };
            }

            // AR has been loaded
            if (accessibilityRegister != null && accessibilityIsSet)
            {
                var visitingAddress = result.VisitingAddresses.SingleOrDefault(a => a.UniqueId == accessibilityRegister.Address.UniqueId);
                if (visitingAddress == null) return;

                var vmAR = TranslationManagerToVm.Translate<AccessibilityRegister, VmAccessibilityRegister>(accessibilityRegister);

                var mainEntrance = vmAR.Entrances.Single(e => e.IsMain);
                visitingAddress.AccessibilityRegister = new VmAccessibilityRegisterUI
                {
                    Id = accessibilityRegister.Id,
                    EntranceId = mainEntrance.Id,
                    Url = accessibilityRegister.Url,
                    SetAt = accessibilityRegister.Modified.ToEpochTime(),
                    IsMainEntrance = true,
                    Groups = mainEntrance.Groups,
                    IsValid = true,
                    ContactEmail = accessibilityRegister.ContactEmail,
                    ContactPhone = accessibilityRegister.ContactPhone,
                    ContactUrl = accessibilityRegister.ContactUrl
                };
                
                // handle AR coordinate for main entrance
                var arCoordinate = visitingAddress.Coordinates.SingleOrDefault(c => c.CoordinateState == CoordinateStates.EnteredByAR.ToString());
                var arCoordinateOfMainEntrance = mainEntrance.Address.Coordinates.SingleOrDefault(c => c.CoordinateState == CoordinateStates.EnteredByAR.ToString());
                if (arCoordinate == null && arCoordinateOfMainEntrance != null)
                {
                    visitingAddress.Coordinates.Add(arCoordinateOfMainEntrance);
                }

                var additionalEntrances = vmAR.Entrances.Where(e => !e.IsMain).ToList();
                if (!additionalEntrances.IsNullOrEmpty())
                {
                    var idx = result.VisitingAddresses.IndexOf(visitingAddress);

                    foreach (var additionalEntrance in additionalEntrances)
                    {
                        var a = additionalEntrance.Address;
                        result.VisitingAddresses.Insert(++idx, a);
                        a.AccessibilityRegister = new VmAccessibilityRegisterUI
                        {
                            Id = accessibilityRegister.Id,
                            IsMainEntrance = false,
                            EntranceId = additionalEntrance.Id,
//                            Url = accessibilityRegister.Url,
//                            SetAt = accessibilityRegister.Created,
                            Groups = additionalEntrance.Groups,
                            IsValid = true
                        };
                    }
                }
            }
        }

        private void DeleteServiceChannelConnections(IUnitOfWork unitOfWork, Guid serviceChannelVersionedId)
        {
            var serviceServiceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var gdServiceChannelRep = unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
            var unificRootId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, serviceChannelVersionedId);
            serviceServiceChannelRep.All().Where(x => x.ServiceChannelId == unificRootId).ForEach(item => serviceServiceChannelRep.Remove(item));
            gdServiceChannelRep.All().Where(x => x.ServiceChannelId == unificRootId).ForEach(item => gdServiceChannelRep.Remove(item));
        }

        public VmElectronicChannel SaveAndValidateElectronicChannel(VmElectronicChannel model)
        {
            var result = ExecuteSaveAndValidate
            (
                model,
                unitOfWork => SaveElectronicChannel(unitOfWork, model),
                (unitOfWork, entity) => GetElectronicChannel(unitOfWork, new VmServiceBasic() { Id = entity.Id })
            );

            return result;
        }

        public Domain.Model.Models.V2.Channel.VmWebPageChannel SaveAndValidateWebPageChannel(Domain.Model.Models.V2.Channel.VmWebPageChannel model)
        {
            var result = ExecuteSaveAndValidate
            (
                model,
                unitOfWork => SaveWebPageChannel(unitOfWork, model),
                (unitOfWork, entity) => GetWebPageChannel(unitOfWork, new VmServiceBasic() { Id = entity.Id })
            );

            return result;
        }

        public VmPrintableFormOutput SaveAndValidatePrintableFormChannel(VmPrintableFormInput model)
        {
            var result = ExecuteSaveAndValidate
            (
                model,
                unitOfWork => SavePrintableFormChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPrintableFormChannel(unitOfWork, new VmServiceBasic() { Id = entity.Id })
            );

            return result;
        }

        public Domain.Model.Models.V2.Channel.VmPhoneChannel SaveAndValidatePhoneChannel(Domain.Model.Models.V2.Channel.VmPhoneChannel model)
        {
            var result = ExecuteSaveAndValidate
            (
                model,
                unitOfWork => SavePhoneChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPhoneChannel(unitOfWork, new VmServiceBasic() { Id = entity.Id })
            );

            return result;
        }

        public VmServiceLocationChannel SaveAndValidateServiceLocationChannel(VmServiceLocationChannel model)
        {
            var result = ExecuteSaveAndValidate
            (
                model,
                unitOfWork => SaveServiceLocationChannel(unitOfWork, model),
                (unitOfWork, entity) => GetServiceLocationChannel(unitOfWork, new VmServiceBasic() { Id = entity.Id })
            );

            return result;
        }

        public VmConnectableChannelSearchResult GetConnectableChannels(VmConnectableChannelSearch search)
        {
            search.Name = search.Name != null
                ? search.Name.Trim()
                : search.Name;

            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var selectedLanguageId = languageCache.Get(search.Language);
                var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                //                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var resultTemp = channelRep.All();
                var languagesIds = new List<Guid>() { selectedLanguageId };

                #region FilteringData

                if (search.Type == DomainEnum.GeneralDescriptions)
                {
                    var languageAvaliabilitiesRep = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
                    languagesIds = languageAvaliabilitiesRep.All()
                    .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == search.Id)
                    .Select(x => x.LanguageId).ToList();
                }

                if (search.Type == DomainEnum.Services)
                {
                    var languageAvaliabilitiesRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
                    languagesIds = languageAvaliabilitiesRep.All()
                    .Where(x => x.ServiceVersionedId == search.Id)
                    .Select(x => x.LanguageId).ToList();
                }

                if (!string.IsNullOrEmpty(search.ChannelType))
                {
                    var channelTypeId = typesCache.Get<ServiceChannelType>(search.ChannelType);
                    resultTemp = resultTemp.Where(sc => sc.TypeId == channelTypeId);
                }
                if (!string.IsNullOrEmpty(search.Name))
                {
                    var rootId = GetRootIdFromString(search.Name);
                    if (!rootId.HasValue)
                    {
                        resultTemp = resultTemp
                            .Where(sc => sc.ServiceChannelNames
                            .Any(y => y.Name.ToLower().Contains(search.Name.ToLower())));
                    }
                    else
                    {
                        resultTemp = resultTemp
                            .Where(channel => channel.UnificRootId == rootId);
                    }
                }
                else
                {
                    resultTemp = resultTemp
                        .Where(sc => sc.ServiceChannelNames.Any(y => !string.IsNullOrEmpty(y.Name)));
                }

                if (search.OrganizationId != null)
                {
                    resultTemp = resultTemp.Where(sc => sc.OrganizationId != null && sc.OrganizationId == search.OrganizationId);
                }
                if (utilities.UserHighestRole() != UserRoleEnum.Eeva)
                {
                    var psCommonForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    var userOrgs = utilities.GetAllUserOrganizations();
                    if (userOrgs.Any())
                    {
                        resultTemp = resultTemp
                            .Where(sc => userOrgs.Contains(sc.OrganizationId) || sc.ConnectionTypeId == psCommonForAll);
                    }
                }
                
                if (search.Type == DomainEnum.GeneralDescriptions)
                {
                    var psCommonForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    resultTemp = resultTemp.Where(sc => sc.ConnectionTypeId == psCommonForAll);                    
                }

                resultTemp = resultTemp.WherePublishingStatusIn(new List<Guid>() {
                    PublishingStatusCache.Get(PublishingStatus.Published),
                    PublishingStatusCache.Get(PublishingStatus.Draft)
                });


                resultTemp = resultTemp.Where(x => x.LanguageAvailabilities.Select(y => y.LanguageId).Any(l => languagesIds.Contains(l)));


                //                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                //                    q.Include(i => i.ServiceChannelNames)
                //                    .Include(i => i.PublishingStatus)
                //                    .Include(j => j.LanguageAvailabilities)
                //                    .Include(j => j.Versioning)
                //                    .Include(i => i.UnificRoot)
                //                );

                #endregion FilteringData

                //                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                IQueryable<Guid> generalDescriptionIds = null;
                if (search.Type == DomainEnum.Services)
                {
                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var generalDescriptionIdoOfService = serviceRep.All().First(s => s.Id == search.Id)
                        .StatutoryServiceGeneralDescriptionId;


                    if (generalDescriptionIdoOfService.HasValue)
                    {
                        var gdConnectionsRep =
                            unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
                        generalDescriptionIds = gdConnectionsRep.All()
                            .Where(gdc =>
                                gdc.StatutoryServiceGeneralDescriptionId == generalDescriptionIdoOfService.Value)
                            .Select(gdc => gdc.ServiceChannelId);
                    }
                }
                var rowCount = resultTemp.Count();
                var pageNumber = search.PageNumber.PositiveOrZero();
                var resultTempData = resultTemp.Select(i => new
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    IsFromGD = generalDescriptionIds != null && generalDescriptionIds.Contains(i.UnificRootId),
                    //                    Name = i.ServiceChannelNames
                    //                        .OrderBy(x => x.Localization.OrderNumber)
                    //                        .FirstOrDefault(x => x.LocalizationId == selectedLanguageId && x.TypeId == nameTypeId).Name,
                    TypeId = i.TypeId,
                    //                    AllNames = i.ServiceChannelNames.Where(x => x.TypeId == nameType).Select(x => new { x.LocalizationId, x.Name }),
                    //                    LanguageAvailabilities = i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber),
                    OrganizationId = i.OrganizationId,
                    //                    Versioning = i.Versioning,
                    //                    VersionMajor = i.Versioning.VersionMajor,
                    //                    VersionMinor = i.Versioning.VersionMinor,
                    Modified = i.Modified,
                    ModifiedBy = i.ModifiedBy,
                    ConnectionType = i.ConnectionTypeId
                })
                    .OrderByDescending(x => x.IsFromGD)
                    .ThenByDescending(i => i.Modified)
                    .ThenByDescending(i => i.Id)
                    .ApplyPaging(pageNumber);
                var serviceChannelIds = resultTempData.SearchResult.Select(i => i.Id).ToList();
                var serviceChannelNameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                var serviceChannelNames = serviceChannelNameRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber).Select(i => new { i.ServiceChannelVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceChannelVersionedId)
                    .ToDictionary(i => i.Key, i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
                var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var channelLangAvailabilities = channelLangAvailabilitiesRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId)).ToList()
                    .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());

                var result = resultTempData.SearchResult.Select(i => new VmConnectableChannel
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    Name = serviceChannelNames.TryGetOrDefault(i.Id, new Dictionary<string, string>()),
                    ChannelTypeId = i.TypeId,
                    ChannelType = typesCache.GetByValue<ServiceChannelType>(i.TypeId),
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        channelLangAvailabilities.TryGetOrDefault(i.Id, new List<ServiceChannelLanguageAvailability>())),
                    OrganizationId = i.OrganizationId,
                    Modified = i.Modified.ToEpochTime(),
                    ModifiedBy = i.ModifiedBy,
                    ConnectionTypeId = i.ConnectionType
                })
                .ToList();
                var returnData = new VmConnectableChannelSearchResult
                {
                    SearchResult = result,
                    MoreAvailable = resultTempData.MoreAvailable,
                    Count = rowCount,
                    PageNumber = pageNumber
                };
                FillEnumEntities(returnData, () => GetEnumEntityCollectionModel("Organizations", CommonService.GetOrganizations(result.Where(org => org.OrganizationId.HasValue).Select(org => org.OrganizationId.Value))));
                return returnData;
            });
        }

        public VmConnectionsChannelSearchResult GetConnectionsChannels(VmConnectionsChannelSearch search)
        {
            search.Fulltext = search.Fulltext != null
                ? search.Fulltext.Trim()
                : search.Fulltext;

            return ContextManager.ExecuteReader(unitOfWork =>
            {
                var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var resultTemp = channelRep.All();

                #region FilteringData
                if (search.ChannelIds != null && search.ChannelIds.Any())
                {
                    resultTemp = resultTemp.Where(channel => search.ChannelIds.Contains(channel.UnificRootId));
                }
                if (!string.IsNullOrEmpty(search.ChannelType))
                {
                    var channelTypeId = typesCache.Get<ServiceChannelType>(search.ChannelType);
                    resultTemp = resultTemp.Where(sc => sc.TypeId == channelTypeId);
                }
                if (!string.IsNullOrEmpty(search.Fulltext))
                {
                    var rootId = GetRootIdFromString(search.Fulltext);
                    if (!rootId.HasValue)
                    {
                        resultTemp = resultTemp
                            .Where(sc => sc.ServiceChannelNames
                            .Any(y => y.Name.ToLower().Contains(search.Fulltext.ToLower())));
                    }
                    else
                    {
                        resultTemp = resultTemp
                            .Where(channel => channel.UnificRootId == rootId);
                    }
                }
                else
                {
                    resultTemp = resultTemp
                        .Where(sc => sc.ServiceChannelNames.Any(y => !string.IsNullOrEmpty(y.Name)));
                }

                if (search.OrganizationId != null)
                {
                    resultTemp = resultTemp
                        .Where(sc => sc.OrganizationId != null && sc.OrganizationId == search.OrganizationId);
                }
                if (utilities.UserHighestRole() != UserRoleEnum.Eeva)
                {
                    var psCommonForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    var userOrgs = utilities.GetAllUserOrganizations();
                    if (userOrgs.Any())
                    {
                        resultTemp = resultTemp
                            .Where(sc => userOrgs.Contains(sc.OrganizationId) || sc.ConnectionTypeId == psCommonForAll);
                    }
                }

                if (search.AreaInformationTypes != null && search.AreaInformationTypes.Any())
                {
                    resultTemp = resultTemp.Where(x => x.AreaInformationTypeId.HasValue && search.AreaInformationTypes.Contains(x.AreaInformationTypeId.Value));
                }

                if (search.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesByEquivalents(search.SelectedPublishingStatuses);
                    resultTemp = resultTemp.WherePublishingStatusIn(search.SelectedPublishingStatuses);
                }

                if (search.Languages != null && search.Languages.Any())
                {
                    var languagesIds = search.Languages.Select(code => languageCache.Get(code.ToString()));
                    resultTemp = resultTemp
                        .Where(x => x.LanguageAvailabilities.Select(y => y.LanguageId)
                        .Any(l => languagesIds.Contains(l)));
                }
                #endregion FilteringData

                var rowCount = resultTemp.Count();
                var pageNumber = search.PageNumber.PositiveOrZero();
                var resultTempData = resultTemp.Select(i => new
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    TypeId = i.TypeId,
                    OrganizationId = i.OrganizationId,
                    Modified = i.Modified,
                    PublishingStatusId = i.PublishingStatusId
                })
                .OrderByDescending(i => i.Modified)
                .ThenByDescending(i => i.Id)
                .ApplyPaging(pageNumber);

                var serviceChannelIds = resultTempData.SearchResult.Select(i => i.Id).ToList();
                var serviceChannelNameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                var serviceChannelNames = serviceChannelNameRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber).Select(i => new { i.ServiceChannelVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceChannelVersionedId)
                    .ToDictionary(i => i.Key, i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));

                var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var channelLangAvailabilities = channelLangAvailabilitiesRep
                    .All()
                    .Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId))
                    .ToList()
                    .GroupBy(i => i.ServiceChannelVersionedId)
                    .ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

                var relations = connectionsService.GetAllServiceChannelRelations(unitOfWork, resultTempData.SearchResult.Select(i => i.UnificRootId).Distinct().ToList());

                var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
                var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
                var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

                var distinctData = resultTempData.SearchResult.GroupBy(x => x.UnificRootId).Select(x => x.OrderBy(y => y.PublishingStatusId == publishingStatusId ? 0 : y.PublishingStatusId == draftStatusId ? 1 : y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault());

                var result = distinctData.Select(i => new VmConnectionsChannelItem
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    Name = serviceChannelNames.TryGetOrDefault(i.Id, new Dictionary<string, string>()),
                    ChannelTypeId = i.TypeId,
                    ChannelType = typesCache.GetByValue<ServiceChannelType>(i.TypeId),
                    OrganizationId = i.OrganizationId,
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        channelLangAvailabilities.TryGetOrDefault(i.Id, new List<ServiceChannelLanguageAvailability>())),
                     Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                            relations.TryGetOrDefault(i.UnificRootId, new List<ServiceServiceChannel>()).OrderBy(x => x.OrderNumber)).ToList()
                })
                .ToList();
                var returnData = new VmConnectionsChannelSearchResult()
                {
                    SearchResult = result,
                    MoreAvailable = resultTempData.MoreAvailable,
                    Count = rowCount,
                    PageNumber = pageNumber
                };
                FillEnumEntities(returnData, () => GetEnumEntityCollectionModel("Organizations", CommonService.GetOrganizations(result.Where(org => org.OrganizationId.HasValue).Select(org => org.OrganizationId.Value))));
                return returnData;
            });
        }

        public VmChannelConnectionsOutput SaveRelations(VmConnectionsInput model)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<ServiceChannelVersioned>(model.Id, unitOfWork);
                SaveRelations(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations(model);
        }

        private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmConnectionsInput model)
        {
            var unificRootId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                model.UnificRootId = unificRootId.Value;
                TranslationManagerToEntity.Translate<VmConnectionsInput, ServiceChannel>(model, unitOfWork);
            }
        }

        public VmChannelConnectionsOutput GetRelations(VmConnectionsInput model)
        {
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                return GetRelations(unitOfWork, model);
            });
        }

        private VmChannelConnectionsOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
            var unificRootId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var relations = connectionsService.GetAllServiceChannelRelations(unitOfWork, new List<Guid> { unificRootId.Value });
                var result = new VmChannelConnectionsOutput()
                {
                    Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                           relations.TryGetOrDefault(unificRootId.Value, new List<ServiceServiceChannel>()).OrderBy(x => x.OrderNumber)).ToList(),
                    LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(channelLangAvailabilitiesRep.All().Where(x => model.Id == x.ServiceChannelVersionedId).OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList()),
                    Id = model.Id
                };
                result.TranslationAvailability = translationService.GetChannelTranslationAvailabilities(unitOfWork, result.Id, unificRootId.Value, result.Connections);
                return result;
            }
            return null;
        }

        private void CheckAndConfirmChannelDeliveredTranslation(IUnitOfWorkWritable unitOfWork, Guid? id, IReadOnlyList<VmLanguageAvailabilityInfo> languages)
        {
            //confirm delivered translations
            if (id.IsAssigned())
            {
                foreach (var language in languages)
                {
                    translationService.ConfirmChannelDeliveredTranslation(unitOfWork, id.Value, language.LanguageId);
                }
            }
        }
        
        private void CheckAddressTypes(List<VmAddressSimple> addresses)
        {
            if (addresses.Any(a => a.StreetType == AddressTypeEnum.Foreign.ToString()) && addresses.Any(a => a.StreetType != AddressTypeEnum.Foreign.ToString()))
            {
                throw new PtvAppException("When address type has value \"Foreign\" no other types of Visitng addresses can be defined.", "Channel.Save.AddressTypeMismatch.MessageFailed");
            }
        }

        public IVmEntityBase IsConnectable(Guid id)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<ServiceChannelVersioned>(id, unitOfWork);
            });
            return null;
            }

        public VmTranslationOrderStateSaveOutputs SendChannelEntityToTranslation(VmTranslationOrderInput model)
        {
            Guid entityId = Guid.Empty;

            ContextManager.ExecuteWriter(unitOfWork =>
            {
                translationService.CheckChannelOrderUpdate(model, unitOfWork);
                entityId = translationService.SaveChannelTranslationOrder(unitOfWork, model);
                unitOfWork.Save();
            });

            return GetChannelTranslationSaveData(entityId, model.SourceLanguage);
        }

        private VmTranslationOrderStateSaveOutputs GetChannelTranslationSaveData(Guid entityId, Guid languageId)
        {
            return ContextManager.ExecuteReader(unitOfWork => new VmTranslationOrderStateSaveOutputs
            {
                Id = entityId,
                Channels = new List<IVmChannel>() { GetServiceChannelVmOutput(unitOfWork, entityId) },
                Translations = new List<VmTranslationOrderStateOutputs>
                {
                    translationService.GetChannelTranslationOrderStates(unitOfWork, entityId, languageId)
                }
            });
        }

        private IVmChannel GetServiceChannelVmOutput(IUnitOfWork unitOfWork, Guid entityId)
        {
            var channelVersioned = GetEntity<ServiceChannelVersioned>(entityId, unitOfWork, q => q);
            if (channelVersioned == null)
            {
                return null;
            }

            var type = typesCache.GetByValue<ServiceChannelType>(channelVersioned.TypeId);
            
            if (type == ServiceChannelTypeEnum.EChannel.ToString())
            {
                return GetElectronicChannel(unitOfWork, new VmServiceBasic() {Id = entityId });
            }

            if (type == ServiceChannelTypeEnum.Phone.ToString())
            {
                return GetPhoneChannel(unitOfWork, new VmServiceBasic() { Id = entityId });
            }

            if (type == ServiceChannelTypeEnum.PrintableForm.ToString())
            {
                return GetPrintableFormChannel(unitOfWork, new VmServiceBasic() { Id = entityId });
            }

            if (type == ServiceChannelTypeEnum.ServiceLocation.ToString())
            {
                return GetServiceLocationChannel(unitOfWork, new VmServiceBasic() { Id = entityId });
            }

            if (type == ServiceChannelTypeEnum.WebPage.ToString())
            {
                return GetWebPageChannel(unitOfWork, new VmServiceBasic() { Id = entityId });
            }

            return null;
        }

        public VmTranslationOrderStateOutputs GetChannelTranslationData(VmTranslationDataInput model)
        {
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                return translationService.GetChannelTranslationOrderStates(unitOfWork, model.EntityId, model.SourceLanguage);
            });
        }
        
        private void HandleSocialHealthCenter(Guid unificRootId, VmChannelHeader vm, IUnitOfWork unitOfWork)
        {
            if (vm == null) return;
            if (!unificRootId.IsAssigned()) return;
            
            vm.UnificRootId = unificRootId;
            if (vm.Oid.IsNullOrEmpty())
            {
                var rep = unitOfWork.CreateRepository<IServiceChannelSocialHealthCenterRepository>();
                rep.BatchDelete(x => x.ServiceChannelId, vm.UnificRootId);
            }
            else
            {
                TranslationManagerToEntity.Translate<VmChannelHeader, ServiceChannelSocialHealthCenter>(vm, unitOfWork);    
            }
        }        
    }
}
