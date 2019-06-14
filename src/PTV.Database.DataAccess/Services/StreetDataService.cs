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
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.DataMigrations;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using PTV.Domain.Model.Models.StreetData;
using PTV.Domain.Model.Models.StreetData.Responses;
using PTV.Framework;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services
{
    /// <inheritdoc cref="IStreetDataService"/>
    [RegisterService(typeof(IStreetDataService), RegisterType.Transient)]
    public class StreetDataService : IStreetDataService
    {
        const int downloadRetries = 3;
        
        private readonly IContextManager contextManager;
        private readonly ILanguageCache languageCache;
        private readonly ILogger<StreetDataService> logger;
        private readonly Guid updateIdentifier = Guid.NewGuid();
        private IOldAddressToClsStructConverter addressToClsStructConverter;

        public StreetDataService(IContextManager contextManager, ILanguageCache languageCache, ILogger<StreetDataService> logger, IOldAddressToClsStructConverter addressToClsStructConverter)
        {
            this.contextManager = contextManager;
            this.languageCache = languageCache;
            this.addressToClsStructConverter = addressToClsStructConverter;
            this.logger = logger;
        }


//        private bool CharIntervalEquals(string startCharA, string endCharA, string startCharB, string endCharB)
//        {
//            return (startCharA.GetFirstChar() == startCharB.GetFirstChar()) && (endCharA.GetFirstChar() == endCharB.GetFirstChar());
//        }
        
        public StreetDataImportResult ImportAndUpdateAddresses(VmStreetAddressCollection streetAddressCollection)
        {
            //int wrongCounter = 0;
            logger.LogInformation($"Importing and updating of addresses started. ({DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss.fff", CultureInfo.InvariantCulture)})");
            streetAddressCollection.Results.ForEach(street =>
            {
                if (street.StreetNumbers == null) street.StreetNumbers = new List<VmStreetNumber>();
                street.StreetNumbers.ForEach(i =>
                {
                    i.PostalCode.Code = ParsePostalCode(i.PostalCode.Url);
//                    i.EndNumber = Math.Max(i.EndNumber.ValueOrDefaultIfZero(i.StartNumberEnd.ValueOrDefaultIfZero(i.StartNumber)), i.EndNumberEnd);
//                    if (i.EndCharacter.GetFirstChar() < i.EndCharacterEnd.GetFirstChar())
//                    {
//                        i.EndCharacter = i.EndCharacterEnd;
//                    }
//                    if ((i.StartNumber > i.StartNumberEnd) && i.StartNumberEnd > 0)
//                    {
//                        Console.WriteLine($"even:{i.IsEven} >  {i.StartNumber}/{i.StartNumberEnd} - {i.EndNumber}/{i.EndNumberEnd}");
//                        i.StartNumber = i.StartNumberEnd;
//                    }
//                    if (i.EndNumber.IsEven() != i.StartNumber.IsEven())
//                    {
//                        i.EndNumber--;
//                    }
//                    if (i.EndNumber < i.StartNumber)
//                    {
//                        i.EndNumber = i.StartNumber;
//                    }
//                    if (i.EndNumberEnd < i.StartNumberEnd)
//                    {
//                        i.EndNumberEnd = i.StartNumberEnd;
//                    }
//                    
//                    
//                    if (i.IsEven != i.StartNumber.IsEven() || (i.EndNumber > 0 && i.IsEven != i.EndNumber.IsEven()) ||  (i.StartNumber > i.EndNumber) || 
//                        (i.EndNumberEnd > 0 && i.IsEven != i.EndNumberEnd.IsEven()) ||
//                        (i.StartNumberEnd > 0 && i.IsEven != i.StartNumberEnd.IsEven()))
//                    {
//                        Console.WriteLine($"{++wrongCounter} WRONG STREET NUMBER IN CLS : street name: {street.Names?.Fi ?? string.Empty}/{street.Names?.Sv ?? string.Empty} valid:{i.Status == VmStatusType.Valid}, even:{i.IsEven}, {i.StartNumber}/{i.StartNumberEnd} - {i.EndNumber}/{i.EndNumberEnd}");
//                    }
                });
//                for (int i = 0; i < street.StreetNumbers.Count-1; i++)
//                {
//                    var sn = street.StreetNumbers[i];
//                    if (sn.Status != VmStatusType.Valid) continue;
//                    foreach (var n in street.StreetNumbers.Where(j => (sn != j) && (sn.Id != j.Id) && (sn.PostalCode?.Code == j.PostalCode?.Code) &&
//                                                                      (j.IsEven == sn.IsEven && j.Status == VmStatusType.Valid) &&
//                                                                      (sn.StartNumber == j.StartNumber) && (sn.EndNumber == j.EndNumber) &&
//                                                                      CharIntervalEquals(sn.StartCharacter, sn.EndCharacter, j.StartCharacter,j.EndCharacter)
//                                                                      ))
//                    {
//                        n.Status = VmStatusType.Invalid;
//                    }
//                    
//                    foreach (var j in street.StreetNumbers.Where(j => (sn != j) && (sn.Id != j.Id) && (sn.PostalCode?.Code == j.PostalCode?.Code) && (j.IsEven == sn.IsEven) && (j.Status == VmStatusType.Valid) && CharIntervalEquals(sn.StartCharacter, sn.EndCharacter, j.StartCharacter,j.EndCharacter)))
//                    {
//                        if ((sn.EndNumber == j.StartNumber || sn.EndNumber+2 == j.StartNumber) && sn.EndNumber > 0)
//                        {
//                            sn.EndNumber = j.EndNumber;
//                            j.Status = VmStatusType.Invalid;
//                            i = -1;
//                            break;
//                        }
//                        if ((sn.StartNumber == j.EndNumber || sn.StartNumber-2 == j.EndNumber) && j.EndNumber > 0)
//                        {
//                            sn.StartNumber = j.StartNumber;
//                            j.Status = VmStatusType.Invalid;
//                            i = -1;
//                            break;
//                        }
//                    };
//                }
                street.StreetNumbers.ForEach(i => i.IsEven = i.StartNumber.IsEven());
            });
            var updateBatches = streetAddressCollection.Results.Batch(1000);
            List<VmStreetAddress> newToAdd = new List<VmStreetAddress>();
            List<Guid> updatedStreets = new List<Guid>();
            List<Guid> createdStreets = new List<Guid>();

            void EntityMarker(IClsAddressEntity entity, EntityState state)
            {
                if ((state == EntityState.Added) || (state == EntityState.Modified))
                {
                    entity.NonCls = false;
                    entity.UpdateIdentifier = updateIdentifier;
                }
            }

            updateBatches.ForEach(batch =>
            {
                var data = batch.ToDictionary(i => i.Id);
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var existingOnes = UpdateStreetAddresses(unitOfWork, data);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreet>(EntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetName>(EntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetNumber>(EntityMarker);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    newToAdd.AddRange(batch.Where(j => !existingOnes.Contains(j.Id)));
                    updatedStreets.AddRange(existingOnes);
                });
            });
            logger.LogInformation($"Updated streets: {updatedStreets.Count}");
            var createBatches = newToAdd.Batch(1000);
            createBatches.ForEach(batch =>
            {
                var list = batch.ToList();
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var addedOnes = CreateStreetAddresses(unitOfWork, list);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreet>(EntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetName>(EntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetNumber>(EntityMarker);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    createdStreets.AddRange(addedOnes);
                });
            });
            logger.LogInformation($"Created streets: {createdStreets.Count}");
            var invalidatedResults = InvalidateNonAffectedStreetAddresses();
            logger.LogInformation($"Invalidated results: {invalidatedResults.Count}");
            var affectedAddresses = addressToClsStructConverter.ReValidateAssignedStreetNumbers();
            logger.LogInformation($"Affected addresses to invalid state: {affectedAddresses}");
            logger.LogInformation($"Importing and updating of addresses done. ({DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss.fff", CultureInfo.InvariantCulture)})");
            return new StreetDataImportResult(updatedStreets.Union(createdStreets).Count(), invalidatedResults.Count, affectedAddresses);
        }
      

        private List<Guid> CreateStreetAddresses(IUnitOfWorkWritable unitOfWork, List<VmStreetAddress> newToAdd)
        {
            var municipalityCodeDictionary = GetMunicipalityCodeDictionary(unitOfWork);
            var postalCodesDictionary = GetPostalCodesDictionary(unitOfWork);
                    
            var streetRep = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
            var streetNamesRep = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
            
            var created = new List<Guid>();
            newToAdd.ForEach(toAdd =>
            {
                var municipality = toAdd?.Municipality?.Code != null ? municipalityCodeDictionary.TryGetOrDefault(toAdd.Municipality.Code) : null;
                if (municipality == null)
                {
                    logger.LogError($"Municipality with code {toAdd?.Municipality?.Code ?? "NONE"} not found for street address {toAdd?.Url}");
                    return;
                }
                
                var newStreet = new ClsAddressStreet()
                {
                    Id = toAdd.Id,
                    IsValid = toAdd.Status == VmStatusType.Valid,
                    Municipality = municipality,
                    UpdateIdentifier = updateIdentifier,
                    NonCls = false
                };
                UpdateStreetNames(streetNamesRep, newStreet, toAdd);
                UpdateStreetNumbers(postalCodesDictionary, newStreet, toAdd);
                streetRep.Add(newStreet);
                created.Add(newStreet.Id);
            });
            return created;
        }

        private List<Guid> UpdateStreetAddresses(IUnitOfWorkWritable unitOfWork, Dictionary<Guid, VmStreetAddress> data)
        {
            var municipalityCodeDictionary = GetMunicipalityCodeDictionary(unitOfWork);
            var postalCodesDictionary = GetPostalCodesDictionary(unitOfWork);
            var streetIds = data.Values.Select(i => i.Id).ToList();
                    
            var streetRep = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
            var streetNamesRep = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
            var affectedStreetNumbers = new List<Guid>();
            //var updatedStreetsByData = new List<Guid>();
            var existingOnes = streetRep.All()
                .Where(i => streetIds.Contains(i.Id))
                .Include(i => i.StreetNames)
                .Include(s => s.StreetNumbers)
                .ToList();
            existingOnes.ForEach(i =>
            {
                var municipality = i?.Municipality?.Code != null ? municipalityCodeDictionary.TryGetOrDefault(i.Municipality.Code, null) : null;
                var downloaded = data[i.Id];
                i.IsValid = downloaded.Status == VmStatusType.Valid && (municipality != null);
                i.Municipality = municipality ?? i.Municipality;
                i.UpdateIdentifier = updateIdentifier;
                UpdateStreetNames(streetNamesRep, i, downloaded);
                affectedStreetNumbers.AddRange(UpdateStreetNumbers(postalCodesDictionary, i, downloaded));
            });
            var updatedStreetsById = existingOnes.Select(i => i.Id).ToList();
//            var searchable = data.Values.GroupBy(i => i.Municipality.Code);
//            existingOnes = streetRep.All()
//                .Where(i => !updatedStreetsById.Contains(i.Id))
//                .Include(i => i.Municipality)
//                .Include(i => i.StreetNames)
//                .Include(s => s.StreetNumbers)
//                .ToList();
//            existingOnes.GroupBy(i => i.Municipality.Code).ForEach(bulk =>
//            {
//                var updatable = searchable.FirstOrDefault(i => i.Key == bulk.Key);
//                if (updatable == null) return;
//                bulk.ForEach(s =>
//                {
//                    var streetNew = updatable.FirstOrDefault(i =>
//                        s.Names.Select(j => j.Name).Contains(i.Names?.Fi) || s.Names.Select(j => j.Name).Contains(i.Names?.Sv) || s.Names.Select(j => j.Name).Contains(i.Names?.En));
//                    if (UpdateStreetNames(streetNamesRep, s, streetNew))
//                    {
//                        updatedStreetsByData.Add(s.Id);
//                        updatedStreetsByData.Add(streetNew.Id);
//                    };
//                });
//            });
//            Console.WriteLine($"Updated streets by ID: {updatedStreetsById.Count}");
//            Console.WriteLine($"Updated streets by direct data: {updatedStreetsByData.Count/2}");
            return updatedStreetsById;
        }

        private bool UpdateStreetNames(IClsAddressStreetNameRepository streetNamesRep, ClsAddressStreet entity, VmStreetAddress model)
        {
            if (model == null) return false;
            Guid langFiId = languageCache.Get("fi");
            Guid langSvId = languageCache.Get("sv");
            Guid langEnId = languageCache.Get("en");
            
            UpdateStreetNameEntity(streetNamesRep, entity, langFiId, model?.Names?.Fi);
            UpdateStreetNameEntity(streetNamesRep, entity, langSvId, model?.Names?.Sv);
            UpdateStreetNameEntity(streetNamesRep, entity, langEnId, model?.Names?.En);
            return true;
        }

        private List<Guid> UpdateStreetNumbers(
            Dictionary<string, PostalCode> postalCodes, 
            ClsAddressStreet street, 
            VmStreetAddress model)
        {
            if (model.StreetNumbers.IsNullOrEmpty())
                return new List<Guid>();
            
            var uniqueStreetNumbers = model.StreetNumbers
                .GroupBy(sn => new { 
                    sn.IsEven, 
                    sn.StartNumber, 
                    sn.EndNumber, 
                    sn.StartNumberEnd, 
                    sn.EndNumberEnd, 
                    PostalCode = sn.PostalCode?.Code ?? string.Empty, 
                    StartCharacter = sn.StartCharacter ?? string.Empty, 
                    EndCharacter = sn.EndCharacter ?? string.Empty, 
                    StartCharacterEnd = sn.StartCharacterEnd ?? string.Empty, 
                    EndCharacterEnd = sn.EndCharacterEnd ?? string.Empty})
                .Select(g => g.FirstOrDefault(m => m.Status == VmStatusType.Valid) ?? g.OrderByDescending(sn => sn.Created).FirstOrDefault())
                .ToList();
            return uniqueStreetNumbers.Select(sn => UpdateStreetNumberEntity(postalCodes, street, sn)).ToList();
        }

        private Guid UpdateStreetNumberEntity(
            Dictionary<string, PostalCode> postalCodes, 
            ClsAddressStreet street,
            VmStreetNumber model)
        {
            var streetNumber = street.StreetNumbers.FirstOrDefault(i => i.Id == model.Id) 
                               ?? street.StreetNumbers.FirstOrDefault(sn => sn.IsEven == model.IsEven
                                                                         && sn.StartNumber == model.StartNumber
                                                                         && sn.EndNumber == model.EndNumber);
            
            if (streetNumber == null)
            {
                streetNumber = new ClsAddressStreetNumber { ClsAddressStreetId = street.Id, Id = model.Id.IsAssigned() ? model.Id : Guid.NewGuid() };
                street.StreetNumbers.Add(streetNumber);
            }
            
            var parsedPostalCode = ParsePostalCode(model.PostalCode.Url);
            if (!postalCodes.TryGetValue(parsedPostalCode, out var postalCode))
            {
                logger.LogError($"Postal code {parsedPostalCode} not found for street address {street.Id}");
            }
            
            streetNumber.PostalCode = postalCode ?? streetNumber.PostalCode;
            streetNumber.EndCharacter = model.EndCharacter;
            streetNumber.EndCharacterEnd = model.EndCharacterEnd;
            streetNumber.EndNumber = model.EndNumber;
            streetNumber.EndNumberEnd = model.EndNumberEnd;
            streetNumber.IsEven = model.IsEven;
            streetNumber.IsValid = model.Status == VmStatusType.Valid;
            streetNumber.StartCharacter = model.StartCharacter;
            streetNumber.StartCharacterEnd = model.StartCharacterEnd;
            streetNumber.StartNumber = model.StartNumber;
            streetNumber.StartNumberEnd = model.StartNumberEnd;
            streetNumber.UpdateIdentifier = updateIdentifier;
            return streetNumber.Id;
        }

        private string ParsePostalCode(string postalCodeUrl)
        {
            var parts = postalCodeUrl.Split("/", StringSplitOptions.RemoveEmptyEntries);
            return parts.Last();
        }

        private Dictionary<string, Municipality> GetMunicipalityCodeDictionary(IUnitOfWork unitOfWork)
        {
            var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();
            return municipalityRepo.All().ToDictionary(m => m.Code);
        }

        private Dictionary<string, PostalCode> GetPostalCodesDictionary(IUnitOfWork unitOfWork)
        {
            var postalCodesRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
            return postalCodesRepo.All().ToDictionary(pc => pc.Code);
        }

        private void UpdateStreetNameEntity(IClsAddressStreetNameRepository rep, ClsAddressStreet street, Guid langId,
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            
            var streetName = street.StreetNames.FirstOrDefault(j => j.LocalizationId == langId) ?? rep.Add(
                                 new ClsAddressStreetName()
                                 {
                                     LocalizationId = langId,
                                     ClsAddressStreetId = street.Id
                                 });
            
            streetName.Name = name;
            streetName.Name3 = name.SafeSubstring(0, 3)?.ToLower();
            streetName.UpdateIdentifier = updateIdentifier;
        }

        /// <inheritdoc cref="Delete"/>
        public List<Guid> InvalidateNonAffectedStreetAddresses(Guid? updateId = null)
        {
            Console.WriteLine("Invalidating addresses...");
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var repositoryStreet = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
                var toInvalidateStreets = repositoryStreet.All().Where(e => e.UpdateIdentifier != (updateId ?? updateIdentifier)).ToList();
                toInvalidateStreets.ForEach(e => e.IsValid = false);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                logger.LogInformation($"Invalidated streets: {toInvalidateStreets.Count}");
                var repositoryNumbers = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
                var toInvalidateNumbs = repositoryNumbers.All().Where(e => e.UpdateIdentifier != (updateId ?? updateIdentifier)).ToList();
                toInvalidateNumbs.ForEach(e => e.IsValid = false);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                logger.LogInformation($"Invalidated street numbers: {toInvalidateNumbs.Count}");
                Console.WriteLine("Invalidating addresses done.");
                return toInvalidateStreets.Select(i => i.Id).ToList();
            });
        }

        public async Task<VmStreetAddressCollection> DownloadAll(HttpClient client, int pageSize, Func<int, string> getBatchUrl)
        {
            var result = new VmStreetAddressCollection {Results = new List<VmStreetAddress>(), Meta = new VmMeta()};
            int? totalResults = null;

            for (var from = 0; !totalResults.HasValue || from < totalResults; from += pageSize)
            {
                var batchUrl = getBatchUrl(from);
                var batch = await DownloadBatch(client, batchUrl);
                if (batch == null) throw new PtvAppException("Cls addresses download failed");  //continue;
                if (!totalResults.HasValue)
                {
                    totalResults = batch.Meta.TotalResults;
                }
                
                result.Results.AddRange(batch.Results);
            }

            return result;
        }

        private async Task<VmStreetAddressCollection> DownloadBatch(HttpClient client, string url)
        {
            int tries = 0;
            while (tries++ < downloadRetries)
            {
                try
                {
                    var response = await client.GetStringAsync(url);
                    return JsonConvert.DeserializeObject<VmStreetAddressCollection>(response);
                }
                catch {}
            }
            logger.LogError($"Downloading of addresses FAILED! URL: {url}");
            return null;
        }
    }
}