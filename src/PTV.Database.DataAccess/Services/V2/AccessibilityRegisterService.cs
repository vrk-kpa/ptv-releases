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
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(IAccessibilityRegisterService), RegisterType.Transient)]
    internal class AccessibilityRegisterService : ServiceBase, IAccessibilityRegisterService
    {
        private readonly AccessibilityRegisterSettings accessibilityRegisterSettings;
        private readonly ProxyServerSettings proxySettings;
        private readonly IUserIdentification userIdentification;
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly ICloningManager cloningManager;
        private readonly IPostalCodeService postalCodeService;

        public AccessibilityRegisterService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IOptions<AccessibilityRegisterSettings> accessibilityRegisterSettings,
            IOptions<ProxyServerSettings> proxySettings,
            IUserIdentification userIdentification,
            ICacheManager cacheManager,
            ICloningManager cloningManager,
            IPostalCodeService postalCodeService) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.accessibilityRegisterSettings = accessibilityRegisterSettings.Value;
            this.proxySettings = proxySettings.Value;
            this.userIdentification = userIdentification;
            this.cloningManager = cloningManager;
            this.postalCodeService = postalCodeService;
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }
       
        private string GenerateAccessibilityRegisterUrl(Guid serviceChannelId, string serviceName, VmAccessibilityRegisterAddress address)
        {
            
            if (address == null) return null;
            var userName = userIdentification.UserName;
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
                hash.ToUpper()));

            return url?.AbsoluteUri;
        }

        public VmAccessibilityRegisterSetOut SetAccessibilityRegister(IUnitOfWorkWritable unitOfWork, VmAccessibilityRegisterSetIn model)
        {
            if (!model.ServiceChannelVersionedId.IsAssigned()) throw new PtvAccessibilityRegisterException("Set failed! ServiceChannelVersionedId is not set.");
            if (!model.AddressId.IsAssigned()) throw new PtvAccessibilityRegisterException("Set failed! AddressId is not set.");

            var vmAccessibilityRegister =  model.Id.IsAssigned()
                ? GetModel<AccessibilityRegister, VmAccessibilityRegister>(GetEntity<AccessibilityRegister>(model.Id, unitOfWork, q => q.Include(a => a.Address)), unitOfWork)
                : null;
            
            var channelEntity = GetEntity<ServiceChannelVersioned>(model.ServiceChannelVersionedId, unitOfWork, q => q.Include(x => x.ServiceChannelNames));

            var addressEntity = GetEntity<Address>(model.AddressId, unitOfWork,
                q => q.Include(x => x.AddressStreets).ThenInclude(i => i.StreetNames)
                    .Include(x => x.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(x => x.Coordinates));
            if (addressEntity == null) throw new PtvAccessibilityRegisterException($"Set failed! No address has been found for AddressId: '{model.AddressId}'");
            
            if (vmAccessibilityRegister != null && vmAccessibilityRegister.Address?.UniqueId != addressEntity.UniqueId)
                throw new PtvAccessibilityRegisterException($"Set failed! Entered Address.UniqueId [{addressEntity.UniqueId}] does not match saved Address.UniqueId [{vmAccessibilityRegister.Address?.UniqueId}].");
            
            var accessibilityAddress = cloningManager.CloneEntity(addressEntity, unitOfWork);
            var vmAddress = TranslationManagerToVm.Translate<Address, VmAddressSimple>(accessibilityAddress);

            var parsedAddress = ParseAddress(vmAddress);
            if (parsedAddress == null) return null;

            var languageId = languageCache.Get(parsedAddress.AddressLanguage);
            var serviceName = channelEntity.ServiceChannelNames.SingleOrDefault(n => n.LocalizationId == languageId && n.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()))?.Name;
            if (serviceName == null) return null;

            var url = GenerateAccessibilityRegisterUrl(channelEntity.UnificRootId, serviceName, parsedAddress);

            vmAccessibilityRegister = vmAccessibilityRegister ?? new VmAccessibilityRegister();
            vmAccessibilityRegister.ServiceChannelId = channelEntity.UnificRootId;
            vmAccessibilityRegister.AddressLanguage = parsedAddress.AddressLanguage;
            vmAccessibilityRegister.Url = url;
            vmAccessibilityRegister.AddressId = accessibilityAddress.Id;
            
            var ar = TranslationManagerToEntity.Translate<VmAccessibilityRegister, AccessibilityRegister>(vmAccessibilityRegister, unitOfWork);
            unitOfWork.Save();

            return new VmAccessibilityRegisterSetOut
            {
                Id = model.AddressId,
                AccessibilityRegister = new VmAccessibilityRegisterUI
                {
                    Id = ar.Id,
                    IsValid = ar.Entrances.Any(),
                    SetAt = null,
                    Url = url,
                    IsMainEntrance = true,
                    Groups = null
                }
            };
        }

        public void LoadAccessibilityRegister(IUnitOfWorkWritable unitOfWork, Guid accessibilityRegisterId)
        {
            if (!accessibilityRegisterId.IsAssigned()) 
                throw new PtvAccessibilityRegisterException("Load failed!. AccessibilityRegisterId is not set.");
            
            var vmAccessibilityRegister = GetModel<AccessibilityRegister, VmAccessibilityRegister>(GetEntity<AccessibilityRegister>(accessibilityRegisterId, unitOfWork,
                q => q.Include(i => i.Address).ThenInclude(i =>  i.Coordinates)
                    .Include(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode) 
                    .Include(i => i.Entrances).ThenInclude(i =>  i.Address)
                    .Include(i => i.Entrances).ThenInclude(i =>  i.Address).ThenInclude(i => i.Coordinates)
                    .Include(i => i.Entrances).ThenInclude(i =>  i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                    .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Values)
                    .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences)
                    .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences).ThenInclude(i => i.Values)
            ), unitOfWork);
            
            var entranceAddresses = vmAccessibilityRegister.Entrances.ToDictionary(e => e.EntranceId, e => e.Address);
            
            var urlEntrances = CreateUri(string.Format(accessibilityRegisterSettings.ServicePointEntrancesUrl,
                accessibilityRegisterSettings.SystemId,
                vmAccessibilityRegister.ServiceChannelId));

            var entranceJson = GetJsonData(urlEntrances);
            
            // sentences has been deleted on AR side
            if (entranceJson == null)
            {
                var arRepository = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
                var ar = arRepository.All().SingleOrDefault(x => x.Id == accessibilityRegisterId);
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
            vmAccessibilityRegister.Entrances = loadedEntrances.Select(e => new VmAccessibilityRegisterEntrance
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

            var urlSentences = CreateUri(string.Format(accessibilityRegisterSettings.ServicePointSentencesUrl,
                accessibilityRegisterSettings.SystemId,
                vmAccessibilityRegister.ServiceChannelId));

            var sentenceJson = GetJsonData(urlSentences);
            var loadedSentences = JsonConvert.DeserializeObject<List<VmJsonAccessibilityRegisterSentence>>(sentenceJson).OrderBy(s => s.SentenceOrderText).ToList();
            var sentencesByEntrance = loadedSentences.GroupBy(g => g.EntranceId, g => g).ToDictionary(g => g.Key, g => g.ToList());

            var coordinateTypeAccessibilityRegisterId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.AccessibilityRegister.ToString());
            
            foreach (var entrance in vmAccessibilityRegister.Entrances)
            {
                // handle main entrance address
                if (entrance.IsMain)
                {
                    entrance.AddressId = vmAccessibilityRegister.AddressId;
                    entrance.Address = vmAccessibilityRegister.Address;
                    HandleCoordinate(entrance, coordinateTypeAccessibilityRegisterId);
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
                    
                    HandleCoordinate(entrance, coordinateTypeAccessibilityRegisterId);
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
            var urlService = CreateUri(string.Format(accessibilityRegisterSettings.ServicePointUrl, accessibilityRegisterSettings.SystemId, vmAccessibilityRegister.ServiceChannelId));
            var serviceJson = GetJsonData(urlService);
            if (serviceJson != null)
            {
                var loadedContactInfo = JsonConvert.DeserializeObject<VmJsonAccessibilityRegisterService>(serviceJson);
                vmAccessibilityRegister.ContactEmail = loadedContactInfo.AccessibilityEmail;
                vmAccessibilityRegister.ContactPhone = loadedContactInfo.AccessibilityPhone;
                vmAccessibilityRegister.ContactUrl = loadedContactInfo.AccessibilityWww;
            }

            vmAccessibilityRegister.SetAt = DateTime.UtcNow;
            TranslationManagerToEntity.Translate<VmAccessibilityRegister, AccessibilityRegister>(vmAccessibilityRegister, unitOfWork);
            unitOfWork.Save();
        }

        private static void HandleCoordinate(VmAccessibilityRegisterEntrance entrance, Guid arCoordinateTypeId)
        {
            if (entrance.Address == null) return;
            if (entrance.Address.Coordinates == null) entrance.Address.Coordinates = new List<VmCoordinate>();
            var arCoordinate = entrance.Address.Coordinates.SingleOrDefault(c => c.TypeId == arCoordinateTypeId);
            if (arCoordinate == null)
            {
                entrance.Address.Coordinates.Add(
                    new VmCoordinate
                    {
                        OwnerReferenceId = entrance.Address.Id,
                        CoordinateState = CoordinateStates.EnteredByAR.ToString(),
                        TypeId = arCoordinateTypeId,
                        Latitude = entrance.Latitude,
                        Longitude = entrance.Longitude
                    });
            }
            else
            {
                arCoordinate.Latitude = entrance.Latitude;
                arCoordinate.Longitude = entrance.Longitude;
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
        
        public VmServiceLocationChannel DeleteAccessibilityRegister(IUnitOfWorkWritable unitOfWork, Guid accessibilityRegisterId)
        {
            if (!accessibilityRegisterId.IsAssigned()) return null;
            var arRepository = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
            var ar = arRepository.All().SingleOrDefault(x => x.Id == accessibilityRegisterId);
            if (ar == null) throw new PtvAccessibilityRegisterException($"Delete failed! Accessibility register with Id '{accessibilityRegisterId}' not found.");

            var checksumSecret = accessibilityRegisterSettings.ChecksumSecret;
            var systemId = accessibilityRegisterSettings.SystemId;
            var servicePointId = ar.ServiceChannelId;
            var userName = userIdentification.UserName;
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
            unitOfWork.Save();
            return null;
        }
        
        private VmAccessibilityRegisterAddress ParseAddress(VmAddressSimple address)
        {
            if (address?.PostalCode == null) return null;
            string addressLanguage;
            string streetAddress;

            var fi = DomainConstants.DefaultLanguage;
            if (address.Street.ContainsKey(fi))
            {
                addressLanguage = fi;
                streetAddress = address.Street[fi];
            }
            else
            {
                var street = address.Street.First();
                addressLanguage = street.Key;
                streetAddress = street.Value;
            }

            int northing;
            int easting;
            var coordinates = address.Coordinates.FirstOrDefault(c => c.IsMain && c.CoordinateState == CoordinateStates.Ok.ToString());
            if (coordinates != null)
            {
                northing = Convert.ToInt32(coordinates.Latitude);
                easting = Convert.ToInt32(coordinates.Longitude);
            }
            else
            {
                coordinates = address.Coordinates.FirstOrDefault(c => c.CoordinateState == CoordinateStates.EnteredByUser.ToString());
                if (coordinates == null) return null;
                northing = Convert.ToInt32(coordinates.Latitude);
                easting = Convert.ToInt32(coordinates.Longitude);
            }

            streetAddress += " " + address.StreetNumber;
            var postOffice = address.PostalCode.PostOffice.IsNullOrEmpty()
                ? postalCodeService.GetPostalCode(address.PostalCode.Id)?.PostOffice
                : address.PostalCode.PostOffice;

            return new VmAccessibilityRegisterAddress
            {
                StreetAddress = streetAddress,
                PostOffice = postOffice,
                Northing = northing,
                Easting = easting,
                AddressLanguage = addressLanguage
            };
        }

        private string GetJsonData(Uri uri)
        {
            return HttpClientWithProxy.Use(proxySettings, httpClient =>
            {
                using (var response = httpClient.GetAsync(uri).Result)
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        return response.Content?.ReadAsStringAsync().Result;
                    }
                    catch (Exception e)
                    {
                        if (!(e is HttpRequestException)) throw;
                        
                        // handling of deleted AR
                        if (response.StatusCode == HttpStatusCode.NotFound) return null;
                        throw new PtvAccessibilityRegisterException(e.Message, e);
                    }
                }
            });
        }

        private void DeleteAccessibilityRegister(string url)
        {
            if (url.IsNullOrEmpty()) return;
            HttpClientWithProxy.Use(proxySettings, httpClient =>
            {
                try
                {
                    var result = httpClient.DeleteAsync(url).Result;
                    if (result.StatusCode == HttpStatusCode.NotFound)
                    {
                        return;
                    }
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new PtvAccessibilityRegisterException($"{result.ReasonPhrase}{Environment.NewLine}{result.RequestMessage}");
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
                DeleteAccessibilityRegister(unitOfWork, vm.AccessibilityMeta.Id.Value);
                return;
            }
            
            // load AR for the location channel
            var arEntity = GetEntity<AccessibilityRegister>(vm.AccessibilityMeta.Id, unitOfWork, q => q.Include(i => i.Address)); 
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
            
            // handle AR coordinate for service channel address
            // (AR coordinate must be removed from service channel address, it stays for AR address only)
            mainEntranceAddress.Coordinates?.RemoveWhere(c => c.CoordinateState == CoordinateStates.EnteredByAR.ToString());
            
            var addressesToHandle = new List<VmAddressSimple>();
            foreach (var visitingAddress in vm.VisitingAddresses)
            {
                if (visitingAddress.AccessibilityRegister == null) continue;
                if (visitingAddress.AccessibilityRegister.IsMainEntrance) continue;
                addressesToHandle.Add(visitingAddress);
            }

            // handle additonal addreses / other addresses
            var coordinateTypeAccessibilityRegisterId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.AccessibilityRegister.ToString());
            foreach (var address in addressesToHandle)
            {
                address.Coordinates
                    .Where(c => c.CoordinateState == CoordinateStates.EnteredByAR.ToString() && !c.TypeId.IsAssigned())
                    .ForEach(c =>c.TypeId = coordinateTypeAccessibilityRegisterId);
                
                TranslationManagerToEntity.Translate<VmAddressSimple, Address>(address, unitOfWork);
                vm.VisitingAddresses.Remove(address);
            }
            
            // main entrance address has been changed, but it is still the same building => re-create AR url
            if (vm.AccessibilityMeta.IsChanged && !vm.AccessibilityMeta.IsDeleted)
            {
                var parsedAddress = ParseAddress(mainEntranceAddress);
                if (parsedAddress == null)
                {
                    DeleteAccessibilityRegister(unitOfWork, arEntity.Id);
                }

                var arLanguage = languageCache.GetByValue(arEntity.AddressLanguageId);
                var serviceName = vm.Name.ContainsKey(arLanguage) ? vm.Name[arLanguage] : null;
                if (serviceName == null) return;
                
                arEntity.Url = GenerateAccessibilityRegisterUrl(arEntity.ServiceChannelId, serviceName, parsedAddress);
            }
            
            // main entrance address has been changed, the new address is different building => remove AR 
            if (vm.AccessibilityMeta.IsChanged && vm.AccessibilityMeta.IsDeleted)
            {
                DeleteAccessibilityRegister(unitOfWork, arEntity.Id);
            }
        }
    }
}
