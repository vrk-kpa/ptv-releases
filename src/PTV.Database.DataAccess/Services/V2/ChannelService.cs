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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
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
using PTV.Framework.ServiceManager;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IChannelService), RegisterType.Transient)]
    internal class ChannelService : EntityServiceBase<ServiceChannelVersioned, ServiceChannel>, IChannelService
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
            IVersioningManager versioningManager
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
        }

        #region Electronic
        public VmElectronicChannel GetElectronicChannel(IVmEntityGet model)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetElectronicChannel(model, unitOfWork));
        }

        private VmElectronicChannel GetElectronicChannel(IVmEntityGet model, IUnitOfWork unitOfWork)
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
            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            return result;
        }


        public VmElectronicChannel SaveElectronicChannel(VmElectronicChannel model)
        {
            return ExecuteSave(
                unitOfWork => SaveElectronicChannel(unitOfWork, model),
                (unitOfWork, entity) => GetElectronicChannel(new VmChannelBasic {Id = entity.Id}, unitOfWork)
            );
        }

        private ServiceChannelVersioned SaveElectronicChannel(IUnitOfWorkWritable unitOfWork, VmElectronicChannel vm)
        {
            //            SetTranslatorLanguage(vm);
            //            channelLogic.PrefilterViewModel(vm);
            //            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var serviceChannel = TranslationManagerToEntity.Translate<VmElectronicChannel, ServiceChannelVersioned>(vm, unitOfWork);

            return serviceChannel;
        }
        #endregion

        #region Web page
        public Domain.Model.Models.V2.Channel.VmWebPageChannel GetWebPageChannel(IVmEntityGet model)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetWebPageChannel(model, unitOfWork));
        }

        private Domain.Model.Models.V2.Channel.VmWebPageChannel GetWebPageChannel(IVmEntityGet model, IUnitOfWork unitOfWork)
        {
            ServiceChannelVersioned entity = null;
            Domain.Model.Models.V2.Channel.VmWebPageChannel result = null;
            result = GetModel<ServiceChannelVersioned, Domain.Model.Models.V2.Channel.VmWebPageChannel>(entity = GetEntity<ServiceChannelVersioned>(model.Id, unitOfWork,
                q => q.Include(x => x.LanguageAvailabilities)
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

            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            return result;
        }

        public Domain.Model.Models.V2.Channel.VmWebPageChannel SaveWebPageChannel(Domain.Model.Models.V2.Channel.VmWebPageChannel model)
        {
            return ExecuteSave(
                unitOfWork => SaveWebPageChannel(unitOfWork, model),
                (unitOfWork, entity) => GetWebPageChannel(new VmChannelBasic { Id = entity.Id }, unitOfWork)
            );
        }

        private ServiceChannelVersioned SaveWebPageChannel(IUnitOfWorkWritable unitOfWork, Domain.Model.Models.V2.Channel.VmWebPageChannel vm)
        {
//            SetTranslatorLanguage(vm);
//            channelLogic.PrefilterViewModel(vm);
//            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var serviceChannel = TranslationManagerToEntity.Translate<Domain.Model.Models.V2.Channel.VmWebPageChannel, ServiceChannelVersioned>(vm, unitOfWork);

            return serviceChannel;
        }
        #endregion

        #region PrintableForm
        public VmPrintableFormOutput GetPrintableFormChannel(IVmEntityGet vm)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetPrintableFormChannel(vm, unitOfWork));
        }
        private VmPrintableFormOutput GetPrintableFormChannel(IVmEntityGet vm, IUnitOfWork unitOfWork)
        {
            var result = new VmPrintableFormOutput();
            ServiceChannelVersioned entity = null;
            result = GetModel<ServiceChannelVersioned, VmPrintableFormOutput>(entity = GetEntity<ServiceChannelVersioned>(vm.Id, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames)
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
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(i => i.AddressStreets)
                        .ThenInclude(x => x.StreetNames)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(i => i.AddressPostOfficeBoxes)
                        .ThenInclude(x => x.PostOfficeBoxNames)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(i => i.AddressPostOfficeBoxes)
                        .ThenInclude(x => x.PostalCode)
                        .ThenInclude(x => x.PostalCodeNames)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(i => i.AddressForeigns)
                        .ThenInclude(x => x.ForeignTextNames)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(i => i.AddressPostOfficeBoxes)
                        .ThenInclude(x => x.PostOfficeBoxNames)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(i => i.AddressStreets)
                        .ThenInclude(x => x.PostalCode)
                        .ThenInclude(x => x.PostalCodeNames)
                    .Include(x => x.Attachments)
                        .ThenInclude(x => x.Attachment)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.ChannelUrls)
                        .ThenInclude(x => x.Type)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(x => x.AddressAdditionalInformations)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.DeliveryAddress)
                        .ThenInclude(x => x.Coordinates)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.FormIdentifiers)
                    .Include(x => x.PrintableFormChannels)
                        .ThenInclude(x => x.FormReceivers)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations)
                        .ThenInclude(j => j.DigitalAuthorization)
            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            return result;
        }
        public VmPrintableFormOutput SavePrintableFormChannel(VmPrintableFormInput model)
        {
            SetTranslatorLanguage(model);
            return ExecuteSave(
                unitOfWork => SavePrintableFormChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPrintableFormChannel(new VmChannelBasic { Id = entity.Id }, unitOfWork)
            );
        }

        private ServiceChannelVersioned SavePrintableFormChannel(IUnitOfWork unitOfWork, VmPrintableFormInput vm)
        {
            return TranslationManagerToEntity.Translate<VmPrintableFormInput, ServiceChannelVersioned>(vm, unitOfWork);
        }
        #endregion

        #region Phone
        public Domain.Model.Models.V2.Channel.VmPhoneChannel GetPhoneChannel(IVmEntityGet model)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetPhoneChannel(model, unitOfWork));
        }

        private Domain.Model.Models.V2.Channel.VmPhoneChannel GetPhoneChannel(IVmEntityGet model, IUnitOfWork unitOfWork)
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
            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            return result;
        }

        public Domain.Model.Models.V2.Channel.VmPhoneChannel SavePhoneChannel(Domain.Model.Models.V2.Channel.VmPhoneChannel model)
        {
            return ExecuteSave(
                unitOfWork => SavePhoneChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPhoneChannel(new VmChannelBasic { Id = entity.Id }, unitOfWork)
            );
        }

        private ServiceChannelVersioned SavePhoneChannel(IUnitOfWorkWritable unitOfWork, Domain.Model.Models.V2.Channel.VmPhoneChannel vm)
        {
            //            SetTranslatorLanguage(vm);
            //            channelLogic.PrefilterViewModel(vm);
            //            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var serviceChannel = TranslationManagerToEntity.Translate<Domain.Model.Models.V2.Channel.VmPhoneChannel, ServiceChannelVersioned>(vm, unitOfWork);

            return serviceChannel;
        }
        #endregion

        #region Service location
        public VmServiceLocationChannel GetServiceLocationChannel(IVmEntityGet model)
        {
            return ContextManager.ExecuteReader(unitOfWork => GetServiceLocationChannel(model, unitOfWork));
        }

        private VmServiceLocationChannel GetServiceLocationChannel(IVmEntityGet model, IUnitOfWork unitOfWork)
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

                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Character)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.Coordinates)
                    .Include(x => x.ServiceLocationChannels).ThenInclude(x => x.Addresses).ThenInclude(x => x.Address).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDescriptions)
                    .Include(x => x.UnificRoot)
                        .ThenInclude(j => j.ServiceServiceChannels)
                        .ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations)
                        .ThenInclude(j => j.DigitalAuthorization)
            ), unitOfWork);

            AddAdditionalInfo(result, unitOfWork);
            AddConnectionsInfo(result, entity.UnificRoot.ServiceServiceChannels, unitOfWork);
            return result;
        }

        public VmServiceLocationChannel SaveServiceLocationChannel(VmServiceLocationChannel model)
        {
            return ExecuteSave(
                unitOfWork => SaveServiceLocationChannel(unitOfWork, model),
                (unitOfWork, entity) => GetServiceLocationChannel(new VmChannelBasic { Id = entity.Id }, unitOfWork)
            );
        }

        private ServiceChannelVersioned SaveServiceLocationChannel(IUnitOfWorkWritable unitOfWork, VmServiceLocationChannel vm)
        {
            //            SetTranslatorLanguage(vm);
            //            channelLogic.PrefilterViewModel(vm);
            //            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var serviceChannel = TranslationManagerToEntity.Translate<VmServiceLocationChannel, ServiceChannelVersioned>(vm, unitOfWork);

            return serviceChannel;
        }
        #endregion

        public VmEntityHeaderBase PublishChannel(IVmPublishingModel model)
        {
            return model.Id.IsAssigned() ? ContextManager.ExecuteWriter(unitOfWork => PublishChannel(unitOfWork, model)) : null;
        }

        private VmEntityHeaderBase PublishChannel(IUnitOfWorkWritable unitOfWork, IVmPublishingModel model)
        {
            Guid? channelId = model.Id;
            //Validate mandatory values
            var validationMessages = ValidationManager.CheckEntity<ServiceChannelVersioned>(channelId.Value, unitOfWork, model);
            if (validationMessages.Any())
            {
                throw new PtvValidationException(validationMessages, null);
            }

            //Publishing
            var affected = CommonService.PublishEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model);

            return GetChannelHeader(affected.Id, unitOfWork);
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
            ), unitOfWork);
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var channelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var numberOfConnectedServices = channelRep.All()
                .Where(x =>
                    x.ServiceChannelId == entity.UnificRootId &&
                    x.Service.Versions.Any(o =>
                        o.PublishingStatusId == draftStatusId ||
                        o.PublishingStatusId == publishingStatusId ||
                        o.PublishingStatusId == modifiedStatusId
                    )
                ).Count();
            result.NumberOfConnections = numberOfConnectedServices;
            result.PreviousInfo = channelId.IsAssigned() ? Utilities.CheckIsEntityEditable<ServiceChannelVersioned, ServiceChannel>(channelId.Value, unitOfWork) : null;
            return result;
        }

        public VmChannelHeader DeleteChannel(Guid? entityId)
        {
            VmChannelHeader result = null;
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                var deletedChannel = DeleteChannel(unitOfWork, entityId);
                unitOfWork.Save();
                result = GetChannelHeader(deletedChannel.Id, unitOfWork);
            });
            UnLockChannel(result.Id.Value);
            return result;
        }

        private ServiceChannelVersioned DeleteChannel(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            DeleteServiceChannelConnections(unitOfWork, entityId);
            return CommonService.ChangeEntityToDeleted<ServiceChannelVersioned>(unitOfWork, entityId.Value);
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
            var result = CommonService.WithdrawEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(channelId);
            UnLockChannel(result.Id.Value);
            return GetChannelHeader(result.Id);
        }

        public VmChannelHeader RestoreChannel(Guid channelId)
        {
            var result = CommonService.RestoreEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(channelId);
            UnLockChannel(result.Id.Value);
            return GetChannelHeader(result.Id);
        }

        public VmChannelHeader ArchiveLanguage(VmEntityBasic model)
        {
            var entity = CommonService.ArchiveLanguage<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(model);
            UnLockChannel(entity.Id);
            return GetChannelHeader(entity.Id);
        }

        public VmChannelHeader RestoreLanguage(VmEntityBasic model)
        {
            var entity = CommonService.RestoreLanguage<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(model);
            UnLockChannel(entity.Id);
            return GetChannelHeader(entity.Id);
        }
        public VmChannelHeader WithdrawLanguage(VmEntityBasic model)
        {
            var entity = CommonService.WithdrawLanguage<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(model);
            UnLockChannel(entity.Id);
            return GetChannelHeader(entity.Id);
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
            var channelUnificId = result.UnificRootId;
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var connectedServicesUnificIds = result.ConnectedServicesUnific.Select(i => i.ServiceId).ToList();
            var channelRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var servicesWithGeneralDescription = new List<ServiceVersioned>();
            var connectedChannels = channelRep.All().Where(i =>
                    (connectedServicesUnificIds.Contains(i.UnificRootId)) &&
                    (i.PublishingStatusId == psPublished || i.PublishingStatusId == psDraft || i.PublishingStatusId == psModified))
                .Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities)
                .ToList()
                // could be done on db when it will be implemented in EF
                .GroupBy(i => i.UnificRootId)
                .Select(i => i.FirstOrDefault(x => x.PublishingStatusId == psPublished) ?? i.FirstOrDefault())
                .Select(i =>
                {
                    if (i.StatutoryServiceGeneralDescriptionId.IsAssigned())
                    {
                        servicesWithGeneralDescription.Add(i);
                    }
                    var modifiedData = result.ConnectedServicesUnific.FirstOrDefault(m => m.ServiceId == i.UnificRootId);
                    var originalConnection =
                        connections.FirstOrDefault(x => x.ServiceId == i.UnificRootId &&
                                                        x.ServiceChannelId == channelUnificId);
                    return new ServiceServiceChannel()
                    {
                        ServiceId = i.UnificRootId,
                        ServiceChannelId = channelUnificId,
                        ServiceServiceChannelDescriptions = originalConnection?.ServiceServiceChannelDescriptions ?? new List<ServiceServiceChannelDescription>(),
                        ServiceServiceChannelDigitalAuthorizations = originalConnection?.ServiceServiceChannelDigitalAuthorizations ?? new List<ServiceServiceChannelDigitalAuthorization>(),
                        IsASTIConnection = originalConnection?.IsASTIConnection ?? false,
                        ServiceServiceChannelExtraTypes = originalConnection?.ServiceServiceChannelExtraTypes ?? new List<ServiceServiceChannelExtraType>(),
                        ChargeTypeId = originalConnection?.ChargeTypeId,

                        Service = new Service() {Id = i.UnificRootId, Versions = new List<ServiceVersioned>() {i}},
                        Modified = modifiedData?.Modified ?? DateTime.MinValue,
                        ModifiedBy = modifiedData?.ModifiedBy ?? string.Empty
                    };
                }).ToList();
            var generalDescriptionsToLoad = servicesWithGeneralDescription.Select(i => i.StatutoryServiceGeneralDescriptionId.Value).ToList();
            if (generalDescriptionsToLoad.Any())
            {
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var generalDescriptions = generalDescriptionRep.All().Where(i =>
                    generalDescriptionsToLoad.Contains(i.UnificRootId) &&
                    (i.PublishingStatusId == psPublished || i.PublishingStatusId == psDraft || i.PublishingStatusId == psModified)).ToList().ToDictionary(i => i.UnificRootId, i => i);
                servicesWithGeneralDescription.ForEach(i =>
                {
                    var gd = generalDescriptions.TryGetOrDefault(i.StatutoryServiceGeneralDescriptionId.Value, null);
                    i.StatutoryServiceGeneralDescription = new StatutoryServiceGeneralDescription() { Versions = new List<StatutoryServiceGeneralDescriptionVersioned>() { gd }};
                    i.TypeId = gd?.TypeId;
                });
            }

            result.Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(connectedChannels).InclusiveToList();

            result.NumberOfConnections = result.Connections.Count;
        }

        private void DeleteServiceChannelConnections(IUnitOfWork unitOfWork, Guid? serviceChannelVersionedId)
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
                model.Id,
                unitOfWork => SaveElectronicChannel(unitOfWork, model),
                (unitOfWork, entity) => GetElectronicChannel(new VmServiceBasic() { Id = entity.Id }, unitOfWork)
            );

            return result;
        }

        public Domain.Model.Models.V2.Channel.VmWebPageChannel SaveAndValidateWebPageChannel(Domain.Model.Models.V2.Channel.VmWebPageChannel model)
        {
            var result = ExecuteSaveAndValidate
            (
                model.Id,
                unitOfWork => SaveWebPageChannel(unitOfWork, model),
                (unitOfWork, entity) => GetWebPageChannel(new VmServiceBasic() { Id = entity.Id }, unitOfWork)
            );

            return result;
        }

        public VmPrintableFormOutput SaveAndValidatePrintableFormChannel(VmPrintableFormInput model)
        {
            var result = ExecuteSaveAndValidate
            (
                model.Id,
                unitOfWork => SavePrintableFormChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPrintableFormChannel(new VmServiceBasic() { Id = entity.Id }, unitOfWork)
            );

            return result;
        }

        public Domain.Model.Models.V2.Channel.VmPhoneChannel SaveAndValidatePhoneChannel(Domain.Model.Models.V2.Channel.VmPhoneChannel model)
        {
            var result = ExecuteSaveAndValidate
            (
                model.Id,
                unitOfWork => SavePhoneChannel(unitOfWork, model),
                (unitOfWork, entity) => GetPhoneChannel(new VmServiceBasic() { Id = entity.Id }, unitOfWork)
            );

            return result;
        }

        public VmServiceLocationChannel SaveAndValidateServiceLocationChannel(VmServiceLocationChannel model)
        {
            var result = ExecuteSaveAndValidate
            (
                model.Id,
                unitOfWork => SaveServiceLocationChannel(unitOfWork, model),
                (unitOfWork, entity) => GetServiceLocationChannel(new VmServiceBasic() { Id = entity.Id }, unitOfWork)
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
                var languageCode = SetTranslatorLanguage(search);
                var selectedLanguageId = languageCache.Get(languageCode.ToString());
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
                    })
                    .OrderByDescending(x => x.IsFromGD)
                    .ThenByDescending(i => i.Modified)
                    .ApplyPaging(pageNumber);
                var serviceChannelIds = resultTempData.Data.Select(i => i.Id).ToList();
                var serviceChannelNameRep = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                var serviceChannelNames = serviceChannelNameRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber).Select(i => new { i.ServiceChannelVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.ServiceChannelVersionedId)
                    .ToDictionary(i => i.Key, i => i.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
                var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var channelLangAvailabilities = channelLangAvailabilitiesRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId)).ToList()
                    .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.ToList());

                var result = resultTempData.Data.Select(i => new VmConnectableChannel
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
                    ModifiedBy = i.ModifiedBy
                })
                .ToList();
                return new VmConnectableChannelSearchResult()
                {
                    Data = result,
                    MoreAvailable = resultTempData.MoreAvailable,
                    Count = rowCount,
                    PageNumber = pageNumber
                };
            });
        }


        public VmChannelConnectionsOutput SaveRelations(VmConnectionsInput model)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
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

        private VmChannelConnectionsOutput GetRelations(VmConnectionsInput model)
        {
            return ContextManager.ExecuteReader(unitOfWork =>
            {
                return GetRelations(unitOfWork, model);
            });
        }

        private VmChannelConnectionsOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelRepository>();
            var unificRootId = versioningManager.GetUnificRootId<ServiceChannelVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var serviceChannel = serviceChannelRep.All()
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.Service).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceNames)
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.Service).ThenInclude(j => j.Versions).ThenInclude(j => j.LanguageAvailabilities)
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceServiceChannelDescriptions)
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.ServiceServiceChannelDigitalAuthorizations).ThenInclude(j => j.DigitalAuthorization)
                                .Include(j => j.ServiceServiceChannels).ThenInclude(j => j.Service).ThenInclude(j => j.Versions).ThenInclude(j => j.StatutoryServiceGeneralDescription).ThenInclude(j => j.Versions)
                                .Single(x => x.Id == unificRootId.Value);
                var result = TranslationManagerToVm.Translate<ServiceChannel, VmChannelConnectionsOutput>(serviceChannel);
                result.Id = model.Id;
                return result;
            }
            return null;
        }
    }
}
