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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.Model.Models;
using PTV.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using PTV.Framework.Logging;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(IAccessibilityRegisterService), RegisterType.Transient)]
    internal class AccessibilityRegisterService : ServiceBase, IAccessibilityRegisterService
    {
        private readonly AccessibilityRegisterSettings accessibilityRegisterSettings;
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ICloningManager cloningManager;
        private readonly IContextManager contextManager;
        private readonly ILogger<AccessibilityRegisterService> jobLogger;

        private VmJobLogEntry BatchJobLogEntry;

//        private const string UiLanguageCode = "<%UI_LANGUAGE_CODE%>";
        private const string UiLanguageCode = "___UI_LANGUAGE_CODE___";

        private const int ImportBatchCount = 100;
        private const int ImportWriteCount = 10;

        public string UiLanguageCodeReplacement => UiLanguageCode;

        public AccessibilityRegisterService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IOptions<AccessibilityRegisterSettings> accessibilityRegisterSettings,
            IPahaTokenAccessor pahaTokenAccessor,
            ICacheManager cacheManager,
            ICloningManager cloningManager,
            IVersioningManager versioningManager,
            IContextManager contextManager,
            ILogger<AccessibilityRegisterService> jobLogger,
            ILanguageOrderCache languageOrderCache
        )
            : base(translationManagerToVm, translationManagerToEntity,
                publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.accessibilityRegisterSettings = accessibilityRegisterSettings.Value;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.cloningManager = cloningManager;
            this.contextManager = contextManager;
            this.jobLogger = jobLogger;
            this.languageOrderCache = languageOrderCache;
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        private string GenerateAccessibilityRegisterUrl(Guid serviceChannelId, string serviceName, VmAccessibilityRegisterAddress address)
        {
            if (address == null) return null;
            var userName = pahaTokenAccessor.UserName;
            var validUntil = DateTime.Today.AddDays(accessibilityRegisterSettings.UrlCreateLinkValidityInDays).ToString("s");

            var checksumSecret = accessibilityRegisterSettings.ChecksumSecret;
            var systemId = accessibilityRegisterSettings.SystemId;
            var servicePointId = serviceChannelId;

            var values = checksumSecret + systemId + servicePointId + userName + validUntil + address.StreetAddress + address.PostOffice + serviceName + address.Northing + address.Easting;
            var hash = Sha256(values);

            var url = CreateUri(string.Format(accessibilityRegisterSettings.CreateAccessibilityRegisterUrl,
                systemId,
                servicePointId,
                WebUtility.UrlEncode(userName),
                WebUtility.UrlEncode(validUntil),
                WebUtility.UrlEncode(serviceName),
                WebUtility.UrlEncode(address.StreetAddress),
                WebUtility.UrlEncode(address.PostOffice),
                address.Northing,
                address.Easting,
                hash.ToUpper(),
                UiLanguageCodeReplacement));
            return url?.AbsoluteUri;
        }

        private VmAccessibilityRegisterSetOut GetAccessibilityRegisterInfo(AccessibilityRegister accessibilityRegister,
            ServiceChannelVersioned serviceChannelVersioned,
            ClsAddressPoint addressPoint,
            Guid languageId)
        {
            if (serviceChannelVersioned == null) return null;

            // parse address
            var parsedAddress = ParseAddress(addressPoint, languageId);
            if (parsedAddress == null) return null;

            // get channel name
            var chanelNames = GetChannelNamesByLanguage(serviceChannelVersioned.ServiceChannelNames);
            var serviceName = GetLanguageName(chanelNames, parsedAddress.AddressLanguageId);
            if (serviceName == null) return null;

            // get url
            var url = GenerateAccessibilityRegisterUrl(serviceChannelVersioned.UnificRootId, serviceName, parsedAddress);

            // return model
            return new VmAccessibilityRegisterSetOut
            {
                Id = addressPoint.AddressId,
                AddressLanguageId = parsedAddress.AddressLanguageId,
                AccessibilityRegister = new VmAccessibilityRegisterUI
                {
                    Id = accessibilityRegister.Id,
                    IsValid = accessibilityRegister.Entrances.Any(),
                    SetAt = null,
                    Url = url,
                    LanguageCodeReplacement = UiLanguageCode,
                    IsMainEntrance = true,
                    Groups = null
                }
            };
        }

        public VmAccessibilityRegisterSetOut SetAccessibilityRegister(IUnitOfWorkWritable unitOfWork, VmAccessibilityRegisterSetIn model)
        {
            if (!model.ServiceChannelVersionedId.IsAssigned()) throw new PtvAccessibilityRegisterException("Set failed! ServiceChannelVersionedId is not set.");
            if (!model.AddressId.IsAssigned()) throw new PtvAccessibilityRegisterException("Set failed! AddressId is not set.");
            if (model.LanguageCode.IsNullOrEmpty()) throw new Exception("No language code provided!");

            var dataLanguageId = languageCache.Get(model.LanguageCode);

            var accessibilityRegister = model.Id.IsAssigned()
                ? GetEntity<AccessibilityRegister>(model.Id, unitOfWork,
                    q => q.Include(x => x.Address)
                        .ThenInclude(x => x.ClsAddressPoints)
                        .ThenInclude(x => x.AddressStreet)
                        .ThenInclude(x => x.StreetNames)
                        .Include(x => x.Address)
                        .ThenInclude(x => x.ClsAddressPoints)
                        .ThenInclude(i => i.PostalCode)
                        .ThenInclude(i => i.PostalCodeNames)
                        .Include(x => x.Address)
                        .ThenInclude(x => x.Coordinates))
                : null; // new AccessibilityRegister();

            var channelEntity = GetEntity<ServiceChannelVersioned>(model.ServiceChannelVersionedId, unitOfWork, q => q.Include(x => x.ServiceChannelNames));
            if (channelEntity == null)
                throw new PtvAccessibilityRegisterException($"Set failed! No channel has been found for ChannelId: '{model.Id}'");

            // get address entity
            var addressEntity = GetEntity<Address>(model.AddressId, unitOfWork,
                q => q.Include(x => x.ClsAddressPoints)
                    .Include(x => x.ClsAddressPoints)
                    .ThenInclude(i => i.AddressStreet)
                    .ThenInclude(i => i.StreetNames)
                    .Include(x => x.ClsAddressPoints)
                    .ThenInclude(i => i.PostalCode)
                    .ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Coordinates));

            if (addressEntity == null)
                throw new PtvAccessibilityRegisterException($"Set failed! No address has been found for AddressId: '{model.AddressId}'");

            // accessibility register is already set -> check address
            //if (accessibilityRegister.Id.IsAssigned())
            if (accessibilityRegister != null)
            {
                // check, that it is the same channel
                if (channelEntity.UnificRootId != accessibilityRegister.ServiceChannelId)
                    throw new PtvAccessibilityRegisterException($"Set failed! Entered Channel.UnificRootId [{channelEntity.UnificRootId}] does not match saved Channel.UnificRootId [{accessibilityRegister.ServiceChannelId}].");

                // check, that selected address comes from the same address as is used for current AR
                if (accessibilityRegister.Address?.UniqueId != addressEntity.UniqueId)
                    throw new PtvAccessibilityRegisterException($"Set failed! Entered Address.UniqueId [{addressEntity.UniqueId}] does not match saved Address.UniqueId [{accessibilityRegister.Address?.UniqueId}].");

                // address point of address
                var addressAddressPoint = addressEntity.ClsAddressPoints?.SingleOrDefault();
                if (addressAddressPoint == null) throw new PtvAccessibilityRegisterException($"Set failed! No address point has been found for address with AddressId: '{addressEntity.Id}'");

                // address point of accessibility register address
                var arAddressPoint = accessibilityRegister.Address.ClsAddressPoints.SingleOrDefault();
                if (arAddressPoint == null) throw new PtvAccessibilityRegisterException($"Set failed! No address point has been found for address with AddressId: '{accessibilityRegister.Address?.Id}'");

                if (addressAddressPoint.PostalCodeId == arAddressPoint.PostalCodeId &&
                    addressAddressPoint.AddressStreetId == arAddressPoint.AddressStreetId &&
                    addressAddressPoint.StreetNumber == arAddressPoint.StreetNumber &&
                    accessibilityRegister.ServiceChannelId == channelEntity.UnificRootId)
                {
                    return GetAccessibilityRegisterInfo(accessibilityRegister, channelEntity, arAddressPoint, accessibilityRegister.AddressId);
                }
            }

            // accessibilityRegister == null => create new accessibility register
            if (accessibilityRegister == null)
            {
                var arRepo = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
                accessibilityRegister = new AccessibilityRegister {ServiceChannelId = channelEntity.UnificRootId};
                arRepo.Add(accessibilityRegister);
            }

            // clone address
            var accessibilityAddress = cloningManager.CloneEntity(addressEntity, unitOfWork);

            // get address point
            var addressPoint = accessibilityAddress.ClsAddressPoints.SingleOrDefault();
            if (addressPoint == null)
                throw new PtvAccessibilityRegisterException($"Set failed! No address point has been found for cloned address of AddressId: '{model.AddressId}'");

            var arModel = GetAccessibilityRegisterInfo(accessibilityRegister, channelEntity, addressPoint, dataLanguageId);

            // save accessibility register
            accessibilityRegister.AddressLanguageId = arModel.AddressLanguageId;
            accessibilityRegister.AddressId = arModel.Id;
            unitOfWork.Save();

            arModel.Id = addressEntity.Id;
            return arModel;
        }

        public void LoadAccessibilityRegister(IUnitOfWorkWritable unitOfWork, Guid accessibilityRegisterId)
        {
            if (!accessibilityRegisterId.IsAssigned())
                throw new PtvAccessibilityRegisterException("Load failed!. AccessibilityRegisterId is not set.");

            var arEntity = unitOfWork.CreateRepository<IRepository<AccessibilityRegister>>()
                .All()
                .Include(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNumbers).ThenInclude(i => i.PostalCode)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreetNumber)
                .SingleOrDefault(x => x.Id == accessibilityRegisterId);

            if (arEntity == null)
                throw new PtvAccessibilityRegisterException($"Load failed!. AccessibilityRegister entity with id = '{accessibilityRegisterId}' not found.");

            arEntity.Entrances = unitOfWork.CreateRepository<IRepository<AccessibilityRegisterEntrance>>()
                .All()
                .Include(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.SentenceGroups).ThenInclude(i => i.Values)
                .Include(i => i.SentenceGroups).ThenInclude(i => i.Sentences).ThenInclude(i => i.Values)
                .Where(x => x.AccessibilityRegisterId == accessibilityRegisterId)
                .ToList();

            var vmAccessibilityRegister = GetModel<AccessibilityRegister, VmAccessibilityRegister>(arEntity, unitOfWork);
            LoadAccessibilityRegister(unitOfWork, vmAccessibilityRegister, false);
            unitOfWork.Save();
        }

        private Uri GenerateServiceEntrancesUri(Guid? serviceChannelId)
        {
            return CreateUri(string.Format(accessibilityRegisterSettings.ServicePointEntrancesUrl,
                accessibilityRegisterSettings.SystemId,
                serviceChannelId));
        }

        private Uri GenerateServiceSentencesUri(Guid? serviceChannelId)
        {
            return CreateUri(string.Format(accessibilityRegisterSettings.ServicePointSentencesUrl,
                accessibilityRegisterSettings.SystemId,
                serviceChannelId));
        }

        private void LoadAccessibilityRegister(IUnitOfWorkWritable unitOfWork, VmAccessibilityRegister vmAccessibilityRegister, bool isBatchJob, string serviceEntrances = null, string serviceSentences = null)
        {
            var entranceAddresses = vmAccessibilityRegister.Entrances.ToDictionary(e => e.EntranceId, e => e.Address);
            var systemId = Guid.Parse(accessibilityRegisterSettings.SystemId);

            var entranceJson = isBatchJob
                ? serviceEntrances
                : GetJsonData(GenerateServiceEntrancesUri(vmAccessibilityRegister.ServiceChannelId));

            // sentences has been deleted on AR side
            if (entranceJson == null)
            {
                if (isBatchJob)
                {
                    // do not handle empty entrances for batch job
                    return;
                }

                var arRepository = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
                var ar = arRepository.All().SingleOrDefault(x => x.Id == vmAccessibilityRegister.Id);
                if (ar == null) return;

                if (entranceAddresses.Any()) arRepository.Remove(ar);
                else
                {
                    ar.IsValid = false;
                    ar.Modified = DateTime.UtcNow;
                }

                unitOfWork.Save();
                return;
            }

            var entranceOrderNumber = 0;
            var loadedEntrances = JsonConvert.DeserializeObject<List<VmJsonAccessibilityRegisterEntrance>>(entranceJson).ToList();

            vmAccessibilityRegister.Entrances = loadedEntrances
                .Where(e => e.SystemId == systemId)
                .Select(e => new VmAccessibilityRegisterEntrance
                {
                    EntranceId = e.EntranceId,
                    IsMain = e.IsMainEntrance,
                    PhotoUrl = e.PhotoUrl,
                    StreetviewUrl = e.StreetviewUrl,
                    OrderNumber = ++entranceOrderNumber,
                    Latitude = e.LocNorthing ?? 0,
                    Longitude = e.LocEasting ?? 0,
                    Names = e.Names.ToDictionary(key => key.Language, value => value.Value)
                }).ToList();

            var mainEntrance = vmAccessibilityRegister.Entrances.SingleOrDefault(e => e.IsMain);
            if (mainEntrance == null) throw new PtvAccessibilityRegisterException("Load failed!. No main entrance has been loaded.");

            var sentenceJson = isBatchJob
                ? serviceSentences
                : GetJsonData(GenerateServiceSentencesUri(vmAccessibilityRegister.ServiceChannelId));

            var loadedSentences = JsonConvert.DeserializeObject<List<VmJsonAccessibilityRegisterSentence>>(sentenceJson)
                .Where(e => e.SystemId == systemId)
                .OrderBy(s => s.SentenceOrderText).ToList();
            var sentencesByEntrance = loadedSentences.GroupBy(g => g.EntranceId, g => g).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var entrance in vmAccessibilityRegister.Entrances)
            {
                // handle main entrance address
                if (entrance.IsMain)
                {
                    entrance.AddressId = vmAccessibilityRegister.AddressId;
                    entrance.Address = vmAccessibilityRegister.Address;
                    HandleCoordinate(entrance);
                }
                else
                {
                    if (entrance.Latitude == 0 || entrance.Longitude == 0)
                    {
                        entrance.Latitude = mainEntrance.Latitude;
                        entrance.Longitude = mainEntrance.Longitude;
                    }

                    // handle existing additional entrance address
                    if (entranceAddresses.ContainsKey(entrance.EntranceId) && entranceAddresses[entrance.EntranceId].Id.HasValue)
                    {
                        entrance.AddressId = entranceAddresses[entrance.EntranceId].Id.Value;
                        entrance.Address = entranceAddresses[entrance.EntranceId];
                    }
                    else
                    {
                        entrance.Address = CreateAdditionalAddress(mainEntrance.Address);
                    }

                    HandleCoordinate(entrance);
                    entrance.Address.AdditionalInformation = entrance.Names;
                }

                var sentenceGroups = new List<VmAccessibilityRegisterGroup>();
                if (!sentencesByEntrance.ContainsKey(entrance.EntranceId)) continue;

                foreach (var item in sentencesByEntrance[entrance.EntranceId])
                {
                    var itemGroups = item.SentenceGroups.ToDictionary(
                        key => key.Language,
                        value => value.Value);

                    if (itemGroups.IsNullOrEmpty())
                        throw new PtvAppException($"Entrance {item.EntranceId}: There is no sentence groups defined.'");

                    var group = sentenceGroups.SingleOrDefault(sg => sg.SentenceGroups.SequenceEqual(itemGroups));
                    if (group == null)
                    {
                        sentenceGroups.Add(group = new VmAccessibilityRegisterGroup
                        {
                            OrderNumber = sentenceGroups.Count + 1,
                            SentenceGroups = itemGroups,
                            Sentences = new List<VmAccessibilityRegisterSentence>()
                        });
                    }

                    var itemSentences = item.Sentences.ToDictionary(
                        key => key.Language,
                        value => value.Value);

                    if (itemGroups.IsNullOrEmpty())
                        throw new PtvAccessibilityRegisterException($"Entrance {item.EntranceId}: There is no sentences defined.'");

                    group.Sentences.Add(new VmAccessibilityRegisterSentence
                    {
                        OrderNumber = group.Sentences.Count + 1,
                        Sentences = itemSentences
                    });
                }

                entrance.Groups = sentenceGroups;
            }

            // try to get AR contact info
            if (!isBatchJob)
            {
                var urlService = CreateUri(string.Format(accessibilityRegisterSettings.ServicePointUrl, accessibilityRegisterSettings.SystemId, vmAccessibilityRegister.ServiceChannelId));
                var serviceJson = GetJsonData(urlService);
                if (serviceJson != null)
                {
                    var loadedContactInfo = JsonConvert.DeserializeObject<VmJsonAccessibilityRegisterService>(serviceJson);
                    vmAccessibilityRegister.ContactEmail = loadedContactInfo.AccessibilityEmail;
                    vmAccessibilityRegister.ContactPhone = loadedContactInfo.AccessibilityPhone;
                    vmAccessibilityRegister.ContactUrl = loadedContactInfo.AccessibilityWww;
                }
            }

            vmAccessibilityRegister.SetAt = DateTime.UtcNow;
            TranslationManagerToEntity.Translate<VmAccessibilityRegister, AccessibilityRegister>(vmAccessibilityRegister, unitOfWork);
        }

        private void HandleCoordinate(VmAccessibilityRegisterEntrance entrance/*, Guid userCoordinateTypeId*/)
        {
            if (entrance.Address == null) return;

            var userCoordinateTypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.User.ToString());
            var arCoordinateTypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.AccessibilityRegister.ToString());

            if (entrance.Address.Coordinates == null) entrance.Address.Coordinates = new List<VmCoordinate>();
            var userCoordinate = entrance.Address.Coordinates.FirstOrDefault(c => c.TypeId == userCoordinateTypeId);

            #region SFIPTV-1797 workaround
            // if UC does not exist but ARC exist -> set ARC as UC
            if (userCoordinate == null)
            {
                userCoordinate = entrance.Address.Coordinates.SingleOrDefault(c => c.TypeId == arCoordinateTypeId);
                if (userCoordinate != null)
                {
                    userCoordinate.CoordinateState = CoordinateStates.EnteredByUser.ToString();
                    userCoordinate.TypeId = userCoordinateTypeId;
                }
            }
            #endregion SFIPTV-1797 workaround

            if (userCoordinate == null)
            {
                entrance.Address.Coordinates.Add(
                    new VmCoordinate
                    {
                        OwnerReferenceId = entrance.Address.Id,
                        CoordinateState = CoordinateStates.EnteredByUser.ToString(),
                        TypeId = userCoordinateTypeId,
                        Latitude = entrance.Latitude,
                        Longitude = entrance.Longitude
                    });
            }
            else
            {
                userCoordinate.Latitude = entrance.Latitude;
                userCoordinate.Longitude = entrance.Longitude;
            }
        }

        private static VmAddressSimple CreateAdditionalAddress(VmAddressSimple mainEntranceAddress)
        {
            return new VmAddressSimple
            {
                OrderNumber = 1,
                AddressCharacter = AddressCharacterEnum.Visiting,
                StreetType = AddressTypeEnum.Other.ToString(),
                UniqueId = mainEntranceAddress.UniqueId,
                PostalCode = mainEntranceAddress.PostalCode
            };
        }

        public VmServiceLocationChannel DeleteAccessibilityRegister(IUnitOfWorkWritable unitOfWork, Guid accessibilityRegisterId, bool save = true)
        {
            if (!accessibilityRegisterId.IsAssigned()) return null;
            var arRepository = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
            var ar = arRepository.All().SingleOrDefault(x => x.Id == accessibilityRegisterId);
            if (ar == null) throw new PtvAccessibilityRegisterException($"Delete failed! Accessibility register with Id '{accessibilityRegisterId}' not found.");

            var checksumSecret = accessibilityRegisterSettings.ChecksumSecret;
            var systemId = accessibilityRegisterSettings.SystemId;
            var servicePointId = ar.ServiceChannelId;
            var userName = pahaTokenAccessor.UserName;
            var validUntil = DateTime.UtcNow.AddMinutes(accessibilityRegisterSettings.UrlDeleteLinkValidityInMinutes).ToString("s");

            var values = checksumSecret + systemId + servicePointId + userName + validUntil;
            var hash = Sha256(values);

            var uri = CreateUri(string.Format(accessibilityRegisterSettings.DeleteServicePointUrl,
                systemId,
                servicePointId,
                WebUtility.UrlEncode(userName),
                WebUtility.UrlEncode(validUntil),
                hash.ToUpper()
            ));

            DeleteAccessibilityRegister(uri?.AbsoluteUri);
            arRepository.Remove(ar);
            if (save) unitOfWork.Save();
            return null;
        }

        private VmAccessibilityRegisterAddress ParseAddress(ClsAddressPoint address, Guid dataLanguageId)
        {
            if (!dataLanguageId.IsAssigned()) throw new Exception("No language is set!");
            if (address == null) return null;
            if (!address.PostalCodeId.IsAssigned()) return null;

            // street names
            var streetNames = address.AddressStreet.Names.ToDictionary(key => key.LocalizationId, value => value.Name);
            if (streetNames.IsNullOrEmpty()) throw new PtvAccessibilityRegisterException("No address street names has been found for channels address!");
            var streetName = GetLanguageStreetName(streetNames, dataLanguageId);
            if (streetName == null) throw new PtvAccessibilityRegisterException("No address street name has been found for channels address!");

            // if user coordinates doesnt exists -> return null, because address has no proper coordinates
            var coordinates = GetCoordinatesForUrl(address.Address);
            if (coordinates == null) return null;

            // get coordinates values
            var northing = coordinates.Latitude;
            var easting = coordinates.Longitude;

            // postal office
            var postalOfficeNames = address.PostalCode.PostalCodeNames.ToDictionary(key => key.LocalizationId, value => value.Name);
            if (postalOfficeNames.IsNullOrEmpty()) throw new PtvAccessibilityRegisterException("No postal office names has been found for channels address!");
            var postalCodeName = GetLanguageName(postalOfficeNames, streetName.Value.Key);

            return new VmAccessibilityRegisterAddress
            {
                StreetAddress = streetName.Value.Value + " " + address.StreetNumber,
                PostOffice = postalCodeName,
                Northing = Convert.ToInt32(northing),
                Easting = Convert.ToInt32(easting),
                AddressLanguageId = streetName.Value.Key
            };
        }

        private Coordinate<Address> GetCoordinatesForUrl(Address address)
        {
            if (address?.Coordinates == null) return null;

            var mainCoordinateId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());
            var userCoordinateId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.User.ToString());
            var arCoordinateId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.AccessibilityRegister.ToString());

            // SFIPTV-1671: take user coordinates at first, if user coordinates does not exists, take main
            var coordinates = address.Coordinates.FirstOrDefault(c => c.TypeId == userCoordinateId && c.CoordinateState == CoordinateStates.EnteredByUser.ToString());
            if (coordinates != null) return coordinates;

            // SFIPTV-1797: The coordinates that the user manually added should be sent to AR
            // workaround - AR coordinates shouldn't be used any more
            // in case that address contains AR coordinates (but no user coordinates), return AR coordinates as a user coordinates
            coordinates = address.Coordinates.FirstOrDefault(c => c.TypeId == arCoordinateId && c.CoordinateState == CoordinateStates.EnteredByAR.ToString());
            if (coordinates != null)
            {
                return coordinates;
            }

            // if user coordinates does not exists -> take main coordinates
            // if main coordinates doesnt exists -> return null, because address has no proper coordinates
            coordinates = address.Coordinates.FirstOrDefault(c => c.TypeId == mainCoordinateId && c.CoordinateState == CoordinateStates.Ok.ToString());
            return coordinates;
        }

        private string GetJsonData(Uri uri)
        {
            //var proxy = proxySettings.OverrideBy(optionalConfiguration?.ProxyServerSettings);
            return Asyncs.HandleAsyncInSync(() => PtvHttpClient.UseAsync(async httpClient =>
            {
                using (var response = await httpClient.GetAsync(uri))
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        if (response.Content == null) return null;
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception e)
                    {
                        if (BatchJobLogEntry != null && jobLogger != null)
                        {
                            jobLogger.LogSchedulerError(BatchJobLogEntry, $"Unable download data from: {uri}", null);
                        }

                        if (!(e is HttpRequestException)) throw;

                        // handling of deleted AR
                        if (response.StatusCode == HttpStatusCode.NotFound) return null;
                        throw new PtvAccessibilityRegisterException(e.Message, e);
                    }
                }
            }));
        }

        private void DeleteAccessibilityRegister(string url)
        {
            if (url.IsNullOrEmpty()) return;
            PtvHttpClient.UseAsync(async httpClient =>
            {
                try
                {
                    using (var result = await httpClient.DeleteAsync(url))
                    {
                        if (result.StatusCode == HttpStatusCode.NotFound)
                        {
                            return;
                        }

                        if (!result.IsSuccessStatusCode)
                        {
                            throw new PtvAccessibilityRegisterException(
                                $"{result.ReasonPhrase}{Environment.NewLine}{result.RequestMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PtvAccessibilityRegisterException(ex.Message, ex);
                }
            });
        }

        private Uri CreateUri(string urlParams)
        {
            return new Uri(new Uri(accessibilityRegisterSettings.BaseUrl), urlParams);
        }

        private static string Sha256(string input)
        {
            return input.GetSha256Hash(Encoding.UTF8, false);
        }

        public void HandleAccessibilityRegisterSave(IUnitOfWorkWritable unitOfWork, VmServiceLocationChannel vm)
        {
            // no AR has been set for location yet
            if (vm.AccessibilityMeta == null) return;

            // delete AR (main entrance address has been removed)
            if (vm.AccessibilityMeta.IsDeleted && !vm.AccessibilityMeta.IsChanged)
            {
                if (vm.AccessibilityMeta.Id.HasValue)
                {
                    DeleteAccessibilityRegister(unitOfWork, vm.AccessibilityMeta.Id.Value);
                }

                return;
            }

            // load AR for the location channel
            var arEntity = GetEntity<AccessibilityRegister>(vm.AccessibilityMeta.Id, unitOfWork,
                q => q.Include(i => i.Address).ThenInclude(i => i.Coordinates));
            if (arEntity == null) return;
            var mainEntranceAddress = vm.VisitingAddresses.SingleOrDefault(va => va.UniqueId == arEntity.Address.UniqueId && va.AccessibilityRegister != null && va.AccessibilityRegister.IsMainEntrance);

            // no address with the same UniqueId in VisitingAddress list
            // OR VisitingAddress list is empty
            // => delete AR
            if (mainEntranceAddress == null || vm.VisitingAddresses.IsNullOrEmpty())
            {
                DeleteAccessibilityRegister(unitOfWork, arEntity.Id);
                return;
            }

            // get EnteredByAR enum values
            var coordinateStateEnteredByAR = CoordinateStates.EnteredByAR.ToString();
            var coordinateTypeEnteredByAR = CoordinateTypeEnum.AccessibilityRegister.ToString();

            // handle AR coordinate for service channel address
            // (AR coordinate must be removed from service channel address, it stays for AR address only)
            mainEntranceAddress.Coordinates?.RemoveWhere(c => c.CoordinateState == coordinateStateEnteredByAR);

            var addressesToHandle = new List<VmAddressSimple>();
            foreach (var visitingAddress in vm.VisitingAddresses)
            {
                if (visitingAddress.AccessibilityRegister == null) continue;
                if (visitingAddress.AccessibilityRegister.IsMainEntrance) continue;
                addressesToHandle.Add(visitingAddress);
            }

            // handle additional addresses / other addresses
            var coordinateTypeAccessibilityRegisterId = typesCache.Get<CoordinateType>(coordinateTypeEnteredByAR);
            foreach (var address in addressesToHandle)
            {
                address.Coordinates
                    .Where(c => c.CoordinateState == coordinateStateEnteredByAR && !c.TypeId.IsAssigned())
                    .ForEach(c => c.TypeId = coordinateTypeAccessibilityRegisterId);

                TranslationManagerToEntity.Translate<VmAddressSimple, Address>(address, unitOfWork);
                vm.VisitingAddresses.Remove(address);
            }

            // main entrance address has been changed, but it is still the same building => re-create AR url
            if (vm.AccessibilityMeta.IsChanged && !vm.AccessibilityMeta.IsDeleted)
            {
                if (arEntity.ServiceChannelId != vm.UnificRootId || !vm.Id.HasValue || !mainEntranceAddress.Id.HasValue)
                {
                    DeleteAccessibilityRegister(unitOfWork, arEntity.Id);
                }
            }

            // main entrance address has been changed, the new address is different building => remove AR
            if (vm.AccessibilityMeta.IsChanged && vm.AccessibilityMeta.IsDeleted)
            {
                DeleteAccessibilityRegister(unitOfWork, arEntity.Id);
            }

            // handle change of coordinates of main entrance address
            if (mainEntranceAddress.Coordinates != null && arEntity.Address?.Coordinates != null)
            {
                var mainCoordinateState = CoordinateStates.Ok.ToString();
                var mainCoordinateTypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());
                var userCoordinateState = CoordinateStates.EnteredByUser.ToString();
                var userCoordinateTypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.User.ToString());
                SynchronizeCoordinates(unitOfWork, mainEntranceAddress.Coordinates, arEntity.Address, mainCoordinateState, mainCoordinateTypeId);
                SynchronizeCoordinates(unitOfWork, mainEntranceAddress.Coordinates, arEntity.Address, userCoordinateState, userCoordinateTypeId);
            }
        }

        public string GetAccessibilityRegisterUrl(IUnitOfWork unitOfWork, Guid accessibilityRegisterId, Guid? serviceChannelVersionedId)
        {
            if (!accessibilityRegisterId.IsAssigned()) throw new PtvAccessibilityRegisterException("AccessibilityRegisterId is not set.");
            if (!serviceChannelVersionedId.IsAssigned()) throw new PtvAccessibilityRegisterException("ServiceChannelVersionedId is not set.");
            var accessibilityRegister = GetEntity<AccessibilityRegister>(accessibilityRegisterId, unitOfWork,
                q => q.Include(x => x.Address)
                    .ThenInclude(x => x.ClsAddressPoints)
                    .ThenInclude(x => x.AddressStreet)
                    .ThenInclude(x => x.StreetNames)
                    .Include(x => x.Address)
                    .ThenInclude(x => x.ClsAddressPoints)
                    .ThenInclude(i => i.PostalCode)
                    .ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Address)
                    .ThenInclude(x => x.Coordinates));

            if (accessibilityRegister == null)
                throw new PtvAccessibilityRegisterException($"AccessibilityRegister with Id '{accessibilityRegisterId}' has not been found.");

            var channelEntity = GetEntity<ServiceChannelVersioned>(serviceChannelVersionedId, unitOfWork,
                q => q.Include(x => x.ServiceChannelNames));

            if (channelEntity == null)
                throw new PtvAccessibilityRegisterException($"ChannelVersioned with Id '{accessibilityRegisterId}' has not been found.");

            // get address point
            var addressPoint = accessibilityRegister.Address.ClsAddressPoints.SingleOrDefault();
            if (addressPoint == null)
                throw new PtvAccessibilityRegisterException($"Set failed! No address point has been found for cloned address of AddressId: '{accessibilityRegister.AddressId}'");

            // parse address
            var parsedAddress = ParseAddress(addressPoint, accessibilityRegister.AddressId);
            if (parsedAddress == null) return null;

            // get channel name
            var chanelNames = GetChannelNamesByLanguage(channelEntity.ServiceChannelNames);
            var serviceName = GetLanguageName(chanelNames, parsedAddress.AddressLanguageId);
            if (serviceName == null) return null;

            // get url
            return GenerateAccessibilityRegisterUrl(channelEntity.UnificRootId, serviceName, parsedAddress);
        }

        private KeyValuePair<Guid, string>? GetLanguageStreetName(IReadOnlyDictionary<Guid, string> names, Guid dataLanguageId)
        {
            if (names == null) throw new PtvAccessibilityRegisterException("No names has been found.");
            if (!names.Any()) return null;
            var defaultLanguageId = languageCache.Get(DomainConstants.DefaultLanguage);
            if (!defaultLanguageId.IsAssigned()) throw new PtvAccessibilityRegisterException("No default language has been found.");

            // get name for default language
            if (names.ContainsKey(defaultLanguageId))
            {
                return new KeyValuePair<Guid, string>(defaultLanguageId, names[defaultLanguageId]);
            }

            // if name is not provided for default language, try to get name for data language
            if (!dataLanguageId.IsAssigned()) return null;
            if (names.ContainsKey(dataLanguageId))
            {
                return new KeyValuePair<Guid, string>(dataLanguageId, names[dataLanguageId]);
            }

            // if street name is not provided neither in default language nor data language, get street in first language
            var (languageId, name) = names.First();
            return new KeyValuePair<Guid, string>(languageId, name);
        }

        private string GetLanguageName(IReadOnlyDictionary<Guid, string> names, Guid languageId)
        {
            if (names == null) throw new PtvAccessibilityRegisterException("Names list is not set.");
            if (!languageId.IsAssigned()) throw new PtvAccessibilityRegisterException("LanguageId is not set.");
            if (!names.Any()) return null;

            // get name for address language
            if (names.ContainsKey(languageId))
            {
                return names[languageId];
            }

            // get name for default language
            var defaultLanguageId = languageCache.Get(DomainConstants.DefaultLanguage);
            if (!defaultLanguageId.IsAssigned()) return null;
            if (names.ContainsKey(defaultLanguageId))
            {
                return names[defaultLanguageId];
            }

            // if name is not provided neither in default language nor data language, get name in first language
            return names.First().Value;
        }

        private Dictionary<Guid, string> GetChannelNamesByLanguage(ICollection<ServiceChannelName> channelNames)
        {
            var result = new Dictionary<Guid, string>();
            if (channelNames.IsNullOrEmpty()) return result;
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            return channelNames.Where(n => n.TypeId == nameTypeId).ToDictionary(key => key.LocalizationId, value => value.Name);
        }

        public void ImportAccessibilityRegisterData(ICollection<VmJsonAccessibilityRegisterService> servicePoints, AccessibilityRegisterSettings settings, VmJobLogEntry logInfo)
        {
            BatchJobLogEntry = logInfo;
            var importWatch = new Stopwatch();
            var importBatchWatch = new Stopwatch();

            // set accessibility register settings
            accessibilityRegisterSettings.SystemId = settings.SystemId;
            accessibilityRegisterSettings.BaseUrl = settings.BaseUrl;
            accessibilityRegisterSettings.ServicePointEntrancesUrl = settings.ServicePointEntrancesUrl;
            accessibilityRegisterSettings.ServicePointSentencesUrl = settings.ServicePointSentencesUrl;

            // get needed ids
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var visitingAddressId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString());
            var mainCoordinateId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());

            // list with service points which could not be imported (either they are not published or do not have correct visiting address)
            var notImportedServiceLocationIds = new List<Guid>();

            // start batch
            var countOfBatchRuns = Math.Ceiling((decimal)servicePoints.Count / ImportBatchCount);
            var batchRunCounter = 0;
            servicePoints.Batch(ImportBatchCount).ForEach(servicePointsBatch =>
            {
                importBatchWatch.Reset();
                importBatchWatch.Start();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"---------> Processing batch {++batchRunCounter} ({countOfBatchRuns}) has been started");

                // IDs of loaded service points in the batch
                var batchServicePointsDict = servicePointsBatch.ToDictionary(key => key.ServicePointId, value => value);
                var batchServicePointsId = batchServicePointsDict.Keys.ToList();

                // AR models for processing
                var arModelsForProcessing = new List<VmAccessibilityRegister>();

                // get existing ARs without entrances
                importWatch.Reset();
                importWatch.Start();
                var existingArWithoutEntranceEntities = contextManager.ExecuteReader(unitOfWork =>
                {
                    return unitOfWork.CreateRepository<IAccessibilityRegisterRepository>()
                        .All()
                        .Include(i => i.Entrances)
                        .Where(ar => batchServicePointsId.Contains(ar.ServiceChannelId) && !ar.Entrances.Any())
                        .Include(i => i.Address).ThenInclude(i => i.Coordinates)
                        .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                        .ToList();
                });
                importWatch.Stop();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"Existing AR without entrances has been loaded from db. Loaded {existingArWithoutEntranceEntities.Count} records in {importWatch.Elapsed}");

                // check duplicity
                var duplicityCheck = DuplicityCheck(existingArWithoutEntranceEntities, x => x.ServiceChannelId);
                if (duplicityCheck.Any())
                {
                    jobLogger.LogSchedulerError(BatchJobLogEntry, $"Import of batch {batchRunCounter} failed! There are duplicities in existing AR list: [{string.Join(", ", duplicityCheck)}]", null);
                    return;
                }

                // translate ar entities to models
                arModelsForProcessing.AddRange(TranslationManagerToVm.TranslateAll<AccessibilityRegister, VmAccessibilityRegister>(existingArWithoutEntranceEntities));

                // check, that all models has ServiceChannelId
                if (arModelsForProcessing.Any(ar => !ar.ServiceChannelId.HasValue))
                {
                    jobLogger.LogSchedulerError(BatchJobLogEntry, $"Import of batch {batchRunCounter} failed! There are some nullable ServiceChannelIds in existing AR list", null);
                    return;
                }

                // get all existing AR IDs from db
                var existingArIds = contextManager.ExecuteReader(unitOfWork =>
                {
                    return unitOfWork.CreateRepository<IAccessibilityRegisterRepository>()
                        .All()
                        .Select(ar => ar.ServiceChannelId)
                        .ToList();
                });

                // get only service points for which does not exist AR
                var servicePointIdsWithoutAr = batchServicePointsId
                    .Where(spId => !existingArIds.Contains(spId))
                    .ToList();

                // get published locations, which does not have AR yet (visiting addresses)
                importWatch.Reset();
                importWatch.Start();
                var publishedLocationsData = contextManager.ExecuteReader(unitOfWork =>
                {
                    return unitOfWork.CreateRepository<IServiceChannelAddressRepository>()
                        .All()
                        .Where(a => a.CharacterId == visitingAddressId
                                    && a.ServiceChannelVersioned.PublishingStatusId == publishedId
                                    && a.Address.Coordinates.Any(c => c.TypeId == mainCoordinateId && c.CoordinateState == CoordinateStates.Ok.ToString())
                                    && servicePointIdsWithoutAr.Contains(a.ServiceChannelVersioned.UnificRootId))
                        .Include(x => x.ServiceChannelVersioned)
                        .ToList();
                });
                importWatch.Stop();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"Existing published locations has been loaded from db. Loaded {publishedLocationsData.Count} records in {importWatch.Elapsed}");

                // check duplicity
                duplicityCheck = DuplicityCheck(publishedLocationsData, x => x.ServiceChannelVersioned.UnificRootId);
                if (duplicityCheck.Any())
                {
                    jobLogger.LogSchedulerError(BatchJobLogEntry, $"Import of batch {batchRunCounter} failed! There are duplicities in published location list: [{string.Join(", ", duplicityCheck)}]", null);
                    return;
                }

                // log service points, which are not published, or does not have correct visiting address
                var publishedLocationIds = publishedLocationsData.Select(sl => sl.ServiceChannelVersioned.UnificRootId).ToList();
                notImportedServiceLocationIds.AddRange(servicePointIdsWithoutAr.Where(sp => !publishedLocationIds.Contains(sp)));

                // create AR
                var createdAccessibilityRegisters = CreateAccessibilityRegistersBatch(publishedLocationsData);

                // check duplicity of just created ARs
                duplicityCheck = DuplicityCheck(createdAccessibilityRegisters, x => x.ServiceChannelId);
                if (duplicityCheck.Any())
                {
                    jobLogger.LogSchedulerError(BatchJobLogEntry, $"Import of batch {batchRunCounter} failed! There are duplicities in just created accessibility registers: [{string.Join(", ", duplicityCheck)}]", null);
                    return;
                }

                // check, if just created ARs are not in the existing ARs
                var existingARs = createdAccessibilityRegisters.Where(ar => arModelsForProcessing.Select(x => x.ServiceChannelId).Contains(ar.ServiceChannelId)).ToList();
                if (existingARs.Any())
                {
                    jobLogger.LogSchedulerError(BatchJobLogEntry, $"Import of batch {batchRunCounter} failed! Just created accessibility registers already exists! [{string.Join(", ", existingARs)}]", null);
                    return;
                }

                // get list of AR models for importing
                arModelsForProcessing.AddRange(TranslationManagerToVm.TranslateAll<AccessibilityRegister, VmAccessibilityRegister>(createdAccessibilityRegisters));

                // download entrances and sentences for existing AR with no entrances
                var serviceEntrances = new Dictionary<Guid, string>();
                var serviceSentences = new Dictionary<Guid, string>();
                importWatch.Reset();
                importWatch.Start();
                foreach (var ar in arModelsForProcessing)
                {
                    if (!ar.ServiceChannelId.HasValue) continue;

                    // download entrances for channel
                    var entrances = GetJsonData(GenerateServiceEntrancesUri(ar.ServiceChannelId));
                    if (entrances.IsNullOrEmpty())
                    {
                        jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"No entrance data has been downloaded for ServiceChannelId {ar.ServiceChannelId}");
                    }
                    else
                    {
                        serviceEntrances.Add(ar.ServiceChannelId.Value, entrances);
                    }

                    // download sentences for channel
                    var sentences = GetJsonData(GenerateServiceSentencesUri(ar.ServiceChannelId));
                    if (sentences.IsNullOrEmpty())
                    {
                        jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"No sentence data has been downloaded for ServiceChannelId {ar.ServiceChannelId}");
                    }
                    else
                    {
                        serviceSentences.Add(ar.ServiceChannelId.Value, sentences);
                    }
                }
                importWatch.Stop();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"{serviceEntrances.Count} entrances; {serviceSentences.Count} sentences has been downloaded in {importWatch.Elapsed}");

                var writerWatch = new Stopwatch();
                var importedServicePointIds = new List<Guid>();

                if (arModelsForProcessing.Any())
                {
                    var countOfWriterBatchRuns = Math.Ceiling((decimal) arModelsForProcessing.Count / ImportWriteCount);
                    var writerBatchRunCounter = 0;
                    importWatch.Reset();
                    importWatch.Start();
                    arModelsForProcessing.Batch(ImportWriteCount).ForEach(batchData =>
                    {
                        var partialImportedServicePointIds = new List<Guid>();
                        jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"Saving accessibility register data {++writerBatchRunCounter} ({countOfWriterBatchRuns})");

                        writerWatch.Reset();
                        writerWatch.Start();
                        contextManager.ExecuteWriter(unitOfWork =>
                        {
                            foreach (var arModel in batchData)
                            {
                                if (!arModel.ServiceChannelId.HasValue) continue;
                                var serviceChannelId = arModel.ServiceChannelId.Value;
                                if (!batchServicePointsDict.ContainsKey(serviceChannelId))
                                {
                                    jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"Service point Id has not been found in service point dictionary: [{serviceChannelId}]");
                                    continue;
                                }

                                if (!serviceEntrances.ContainsKey(serviceChannelId) || serviceEntrances[serviceChannelId].IsNullOrEmpty())
                                {
                                    jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"Importing of ServicePointId '{serviceChannelId}' has been skipped, no entrance data has been found");
                                    continue;
                                }

                                if (!serviceSentences.ContainsKey(serviceChannelId) || serviceSentences[serviceChannelId].IsNullOrEmpty())
                                {
                                    jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"Importing of ServicePointId '{serviceChannelId}' has been skipped, no sentence data has been found");
                                    continue;
                                }

                                // update contact info
                                var sp = batchServicePointsDict[serviceChannelId];
                                arModel.ContactEmail = sp.AccessibilityEmail;
                                arModel.ContactPhone = sp.AccessibilityPhone;
                                arModel.ContactUrl = sp.AccessibilityWww;

                                // update ar info
                                LoadAccessibilityRegister(unitOfWork, arModel, true, serviceEntrances[serviceChannelId], serviceSentences[serviceChannelId]);
                                partialImportedServicePointIds.Add(serviceChannelId);
                            }

                            unitOfWork.Save(SaveMode.AllowAnonymous, userName: logInfo.UserName);
                        });

                        writerWatch.Stop();
                        jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"Accessibility register data has been imported ({partialImportedServicePointIds.Count} service points) in {writerWatch.Elapsed}. ServiceChannelIds of just processed accessibility registers: {string.Join(", ", partialImportedServicePointIds)}");
                        importedServicePointIds.AddRange(partialImportedServicePointIds);
                    });
                }

                importWatch.Stop();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"Accessibility register data has been imported for {importedServicePointIds.Count} service points in {importWatch.Elapsed}");

                // log info about one batch run
                importBatchWatch.Stop();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"---------> Processing batch {batchRunCounter} ({countOfBatchRuns}) has been finished in {importBatchWatch.Elapsed}");
            });

            if (notImportedServiceLocationIds.Any())
            {
                jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"Importing of ServicePointIds has been skipped, no published channel or no visiting address has been found (total {notImportedServiceLocationIds.Count}): [{string.Join(", ", notImportedServiceLocationIds)}]");
            }

            BatchJobLogEntry = null;
        }

        private static List<Guid> DuplicityCheck<T>(IEnumerable<T> list, Func<T, Guid> groupBy)
        {
            return list.GroupBy(groupBy)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();
        }

        private List<AccessibilityRegister> CreateAccessibilityRegistersBatch(ICollection<ServiceChannelAddress> publishedLocations)
        {
            var createdAccessibilityRegisters = new List<AccessibilityRegister>();
            if (publishedLocations.IsNullOrEmpty()) return createdAccessibilityRegisters;

            var defaultLanguageId = languageCache.Get(DomainConstants.DefaultLanguage);

            var partialWatch = new Stopwatch();
            var totalWatch = new Stopwatch();

            var countOfBatchRuns = Math.Ceiling((decimal)publishedLocations.Count / ImportWriteCount);
            var batchRunCounter = 0;
            totalWatch.Start();
            publishedLocations.Batch(ImportWriteCount).ForEach(batchData =>
            {
                var serviceChannelList = batchData.ToList();
                if (serviceChannelList.IsNullOrEmpty()) return;

                partialWatch.Reset();
                partialWatch.Start();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"Saving accessibility registers {++batchRunCounter} ({countOfBatchRuns})");

                var partialCreatedAccessibilityRegisters = new List<AccessibilityRegister>();
                var addressIds = serviceChannelList.Select(sl => sl.AddressId).ToList();
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var accessibilityRegisterRepo = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();

                    // get addresses for the batch
                    var addressesForBatch = unitOfWork.CreateRepository<IAddressRepository>()
                        .All()
                        .Where(a => addressIds.Contains(a.Id))
                        .Include(x => x.Coordinates)
                        .Include(x => x.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                        .Include(x => x.ClsAddressPoints).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .ToDictionary(key => key.Id, value => value);

                    foreach (var serviceLocation in serviceChannelList)
                    {
                        if (!addressesForBatch.ContainsKey(serviceLocation.AddressId))
                        {
                            jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"Import failed! No address has been found for AddressId: '{serviceLocation.AddressId}'");
                            continue;
                        }

                        var addressEntity = addressesForBatch[serviceLocation.AddressId];

                        // clone address
                        var accessibilityAddress = cloningManager.CloneEntity(addressEntity, unitOfWork);

                        // get address point
                        var addressPoint = accessibilityAddress.ClsAddressPoints.SingleOrDefault();
                        if (addressPoint == null)
                        {
                            jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"Import failed! No address point has been found for address point of address id: '{addressEntity.Id}' [ServiceChannelId: {serviceLocation.ServiceChannelVersioned.UnificRootId}]");
                            continue;
                        }

                        // get address language id
                        var addressLanguageId = GetLanguageIdForAddress(addressPoint, defaultLanguageId, out var outputMessage);
                        if (addressLanguageId == Guid.Empty)
                        {
                            jobLogger.LogSchedulerWarn(BatchJobLogEntry, $"Import failed! No address language id has been found for service point of address id: '{addressEntity.Id}' [ServiceChannelId: {serviceLocation.ServiceChannelVersioned.UnificRootId}]. {outputMessage}");
                            continue;
                        }

                        // create AR
                        var accessibilityRegister = new AccessibilityRegister
                        {
                            ServiceChannelId = serviceLocation.ServiceChannelVersioned.UnificRootId,
                            AddressId = accessibilityAddress.Id,
                            AddressLanguageId = addressLanguageId,
                            IsValid = true
                        };

                        accessibilityRegisterRepo.Add(accessibilityRegister);
                        partialCreatedAccessibilityRegisters.Add(accessibilityRegister);
                    }

                    unitOfWork.Save(SaveMode.AllowAnonymous, userName: BatchJobLogEntry.UserName);
                });

                partialWatch.Stop();
                jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"Accessibility registers has been saved (total {partialCreatedAccessibilityRegisters.Count}) in {partialWatch.Elapsed}. ServiceChannelIds of just created accessibility registers: {string.Join(", ", partialCreatedAccessibilityRegisters.Select(ar => ar.ServiceChannelId))}");
                createdAccessibilityRegisters.AddRange(partialCreatedAccessibilityRegisters);
            });

            // log info about one batch run
            totalWatch.Stop();
            jobLogger.LogSchedulerInfo(BatchJobLogEntry, $"{createdAccessibilityRegisters.Count} accessibility register entities has been created in {totalWatch.Elapsed}");
            return createdAccessibilityRegisters;
        }

        private Guid GetLanguageIdForAddress(ClsAddressPoint addressPoint, Guid defaultLanguageId, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!defaultLanguageId.IsAssigned())
            {
                errorMessage = "No default language is set";
                return Guid.Empty;
            }

            if (addressPoint?.AddressStreet == null)
            {
                errorMessage = "AddressPoint or AddressStreet is not set";
                return Guid.Empty;
            }

            if (addressPoint.AddressStreet.StreetNames.IsNullOrEmpty())
            {
                errorMessage = $"No address street names for addressPoint {addressPoint.Id}";
                return Guid.Empty;
            }

            // street names
            var streetNameIds = addressPoint.AddressStreet.StreetNames
                .OrderBy(l => languageOrderCache.Get(l.LocalizationId))
                .Select(n => n.LocalizationId).ToList();

            // check if street name does exist in default language
            // if no default language found -> return first found language (ordered by OrderNumber)
            return streetNameIds.Contains(defaultLanguageId)
                ? defaultLanguageId
                : streetNameIds.First();
        }

        private void SynchronizeCoordinates(IUnitOfWorkWritable unitOfWork, IEnumerable<VmCoordinate> fromCoordinates, Address toAddress, string coordinateState, Guid coordinateTypeId)
        {
            if (toAddress?.Coordinates == null) return;

            var to = toAddress.Coordinates.FirstOrDefault(c => c.CoordinateState == coordinateState);
            var from = fromCoordinates?.FirstOrDefault(c => c.CoordinateState == coordinateState);

            if (from == null && to == null) return;

            if (from == null && to != null)
            {
                var acRepo = unitOfWork.CreateRepository<IAddressCoordinateRepository>();
                acRepo.Remove(to);
                return;
            }

            if (from != null && to == null)
            {
                to = unitOfWork
                    .CreateRepository<IAddressCoordinateRepository>()
                    .Add(new AddressCoordinate
                    {
                        CoordinateState = coordinateState,
                        TypeId = coordinateTypeId,
                        RelatedToId = toAddress.Id
                    });
            }

            if (to.Latitude != from.Latitude) to.Latitude = from.Latitude;
            if (to.Longitude != from.Longitude) to.Longitude = from.Longitude;
        }
    }
}
