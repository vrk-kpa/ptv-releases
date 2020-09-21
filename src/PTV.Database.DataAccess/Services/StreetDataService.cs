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
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.DataMigrations;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Cloud;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
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
        private readonly IContextManager contextManager;
        private readonly ILanguageCache languageCache;
        private readonly ILogger<StreetDataService> logger;
        private readonly Guid updateIdentifier = Guid.NewGuid();
        private readonly IStorageService storage;

        public StreetDataService(
            IContextManager contextManager, 
            ILanguageCache languageCache, 
            ILogger<StreetDataService> logger,
            IStorageService storage)
        {
            this.contextManager = contextManager;
            this.languageCache = languageCache;
            this.logger = logger;
            this.storage = storage;
        }
        
        public StreetDataImportResult ImportAndUpdateAddresses(VmStreetAddressCollection streetAddressCollection)
        {
            logger.LogInformation($"Importing and updating of addresses started. ({DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss.fff", CultureInfo.InvariantCulture)})");
            PolyfillIsEven(streetAddressCollection);
            
            var newToAdd = new List<VmStreetAddress>();
            var updatedStreets = new List<Guid>();
            var createdStreets = new List<Guid>();
            
            UpdateExistingStreets(streetAddressCollection, newToAdd, updatedStreets);
            logger.LogInformation($"Updated streets: {updatedStreets.Count}");
            
            CreateNewStreets(newToAdd, createdStreets);
            logger.LogInformation($"Created streets: {createdStreets.Count}");
            
            var invalidatedResults = InvalidateNonAffectedStreetAddresses();
            logger.LogInformation($"Invalidated streets: {invalidatedResults.Count}");
            
            var affectedAddresses = ReValidateAssignedStreetNumbers();
            logger.LogInformation($"Revalidated addresses: {affectedAddresses}");

            var deletedAddresses = DeleteUnusedAddresses();
            logger.LogInformation($"Deleted unused addresses: {deletedAddresses}");
            
            var deletedClsData = DeleteUnusedClsData();
            logger.LogInformation($"Deleted CLS data: {deletedClsData}");
            
            logger.LogInformation($"Importing and updating of addresses done. ({DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss.fff", CultureInfo.InvariantCulture)})");
            return new StreetDataImportResult
            {
                ImportedStreets = updatedStreets.Union(createdStreets).Count(),
                InvalidatedStreets = invalidatedResults.Count,
                ReValidatedData = affectedAddresses,
                DeletedAddresses = deletedAddresses,
                DeletedClsData = deletedClsData
            };
        }

        private int DeleteUnusedClsData()
        {
            var result = 0;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var streetRepo = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
                var streetNumberRepo = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();

                var unusedStreetNumbers = streetNumberRepo.All()
                    .Where(x => !x.IsValid && !x.NonCls && !x.AddressPoints.Any());
                foreach (var batch in unusedStreetNumbers.Batch(1000))
                {
                    result += batch.Count;
                    logger.LogInformation($"Deleted CLS data {result}");
                    streetNumberRepo.Remove(batch);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }

                var unusedStreets = streetRepo.All()
                    .Where(x => !x.IsValid && !x.NonCls && !x.AddressPoints.Any() && !x.StreetNumbers.Any());
                foreach (var batch in unusedStreets.Batch(1000))
                {
                    result += batch.Count;
                    logger.LogInformation($"Deleted CLS data {result}");
                    streetRepo.Remove(batch);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
            });
            return result;
        }

        private int DeleteUnusedAddresses()
        {
            var counter = 0;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var addressRepo = unitOfWork.CreateRepository<IAddressRepository>();
                var unusedAddresses = addressRepo.All()
                    .Where(x => !x.AccessibilityRegisters.Any()
                                && !x.AccessibilityRegisterEntrances.Any()
                                && !x.OrganizationAddresses.Any()
                                && !x.ServiceChannelAddresses.Any()
                                && !x.ServiceServiceChannelAddresses.Any());
                
                foreach (var batch in unusedAddresses.Batch(1000))
                {
                    logger.LogInformation($"Deleting addresses {counter += batch.Count}");
                    addressRepo.Remove(batch);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
            });

            return counter;
        }

        private void CreateNewStreets(List<VmStreetAddress> newToAdd, List<Guid> createdStreets)
        {
            var counter = 0;
            var createBatches = newToAdd.Batch(1000);
            foreach (var batch in createBatches)
            {
                var list = batch.ToList();
                logger.LogInformation($"Creating batch {counter += list.Count}/{newToAdd.Count}");
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var addedOnes = CreateStreetAddresses(unitOfWork, list);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreet>(AddEntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetName>(AddEntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetNumber>(AddEntityMarker);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    createdStreets.AddRange(addedOnes);
                });
            }
        }

        private void UpdateExistingStreets(VmStreetAddressCollection streetAddressCollection, List<VmStreetAddress> newToAdd,
            List<Guid> updatedStreets)
        {
            var counter = 0;
            var updateBatches = streetAddressCollection.Results.Batch(1000);

            foreach (var batch in updateBatches)
            {
                var data = batch.ToDictionary(i => i.Id);
                logger.LogInformation($"Updating batch {counter += data.Count}/{streetAddressCollection.Results.Count}");
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var existingOnes = UpdateStreetAddresses(unitOfWork, data);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreet>(AddEntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetName>(AddEntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetNumber>(AddEntityMarker);
                    try
                    {
                        unitOfWork.Save(SaveMode.AllowAnonymous);
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning($"Save exception {e.FlattenWithInnerExceptions()}");
                        throw;
                    }

                    newToAdd.AddRange(data.Values.Where(j => !existingOnes.Contains(j.Id)));
                    updatedStreets.AddRange(existingOnes);
                });
            }
        }

        private void PolyfillIsEven(VmStreetAddressCollection streetAddressCollection)
        {
            logger.LogInformation("Polyfilling even number identifier");
            streetAddressCollection.Results.ForEach(street =>
            {
                if (street.StreetNumbers == null) street.StreetNumbers = new List<VmStreetNumber>();
                street.StreetNumbers.ForEach(i => i.IsEven = i.StartNumber.IsEven());
            });
        }

        private void AddEntityMarker(IClsAddressEntity entity, EntityState state)
        {
            if ((state == EntityState.Added) || (state == EntityState.Modified))
            {
                entity.NonCls = false;
                entity.UpdateIdentifier = updateIdentifier;
            }
        }

        private int ReValidateAssignedStreetNumbers()
        {
            return contextManager.ExecuteWriter(unitOfWork =>
                {
                    logger.LogInformation("Revalidating addresses, part 1");
                    var addrPointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                    addrPointRep.All()
                        .Where(i => i.IsValid && !i.AddressStreet.IsValid)
                        .Batch(1000)
                        .ForEach(toCheckList =>
                        {
                            toCheckList.ForEach(addr => addr.IsValid = false);
                        });
                    
                    logger.LogInformation("Revalidating addresses, part 2");
                    addrPointRep.All()
                        .Where(i => i.IsValid && !i.AddressStreetNumber.IsValid 
                            || !i.IsValid && i.AddressStreetNumber.IsValid && i.AddressStreet.IsValid)
                        .Include(i => i.AddressStreet)
                        .ThenInclude(i => i.StreetNumbers)
                        .Include(i => i.AddressStreetNumber)
                        .Batch(1000)
                        .ForEach(toCheckList =>
                        {
                            toCheckList.ForEach(addr =>
                            {
                                (var streetNumberIntLower, var streetNumberIntHigher, var streetNumberChar) = ExtractStreetNumber(addr.StreetNumber);
                                var rightStrNum = GetStreetNumberInterval(addr.AddressStreet.StreetNumbers, streetNumberIntLower, streetNumberIntHigher);
                                if (rightStrNum == null || (rightStrNum.PostalCodeId != addr.AddressStreetNumber.PostalCodeId))
                                {
                                    addr.IsValid = false;
                                }
                                else if (rightStrNum.Id.IsAssigned() && (addr.AddressStreetNumberId != rightStrNum.Id))
                                {
                                    addr.AddressStreetNumberId = rightStrNum.Id;
                                    addr.IsValid = rightStrNum.IsValid && addr.AddressStreet.IsValid;
                                }
                            });
                        });
                    
                    logger.LogInformation("Revalidating done.");
                    return unitOfWork.Save(SaveMode.AllowAnonymous);
                }
            );
        }
        
        private ClsAddressStreetNumber GetStreetNumberInterval(ICollection<ClsAddressStreetNumber> streetNumbers, int streetNumberIntLow, int streetNumberIntHigh)
        {
            bool isEven = streetNumberIntLow.IsEven();
            return streetNumbers.FirstOrDefault(i =>
                i.IsEven == isEven && i.StartNumber <= streetNumberIntLow &&
                Math.Max(i.EndNumber.ValueOrDefaultIfZero(i.StartNumber), i.EndNumberEnd) >= streetNumberIntLow);
        }

        private (int, int, char) ExtractStreetNumber(string rawStreetNumber)
        {
            rawStreetNumber = rawStreetNumber?.Trim();
            if (string.IsNullOrEmpty(rawStreetNumber)) return (0, 0, default(char));

            var streetNumParts = rawStreetNumber.Split('-').Select(i => i.Trim()).ToList();
            if (streetNumParts.Count == 2)
            {
                int intPart1 = ParseDigitsPart(streetNumParts[0]);
                int intPart2 = ParseDigitsPart(streetNumParts[1]);
                if (intPart1 > 0 && intPart2 > 0)
                {
                    return (intPart1, intPart2, default(char));
                }
            }

            int result = ParseDigitsPart(rawStreetNumber);
            if (result > 0)
            {
                string intString = result.ToString();
                int firstPos = rawStreetNumber.IndexOf(intString, StringComparison.Ordinal);
                if (firstPos == 0)
                {
                    char charPart = rawStreetNumber.Substring(intString.Length).Trim().GetFirstChar();
                    return (result, result, charPart);
                }
            }

            //Console.WriteLine($"NON CONVERTABLE STREETNUM! '{rawStreetNumber}' intpart: '{result}'");

            return (0, 0, default(char));
        }
        
        private int ParseDigitsPart(string str)
        {
            string streetNumberInString = new string(str.Trim().TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(streetNumberInString, out int result))
            {
                return result;
            }
            streetNumberInString = new string(str.Trim().Replace(" ", string.Empty).TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(streetNumberInString, out int result2) ? result2 : 0;
        }

        private List<Guid> CreateStreetAddresses(IUnitOfWorkWritable unitOfWork, List<VmStreetAddress> newToAdd)
        {
            var counter = 0;
            var municipalityCodeDictionary = GetMunicipalityCodeDictionary(unitOfWork);
            var postalCodesDictionary = GetPostalCodesDictionary(unitOfWork);

            var streetRep = unitOfWork.CreateRepository<IClsAddressStreetRepository>();

            var created = new List<Guid>();
            foreach (var toAdd in newToAdd)
            {
                if (counter % 100 == 0)
                {
                    logger.LogInformation($"Creating CLS streets {counter}/{newToAdd.Count}");
                }
                var municipality = toAdd?.Municipality?.Code != null ? municipalityCodeDictionary.TryGetOrDefault(toAdd.Municipality.Code) : null;
                if (municipality == null)
                {
                    logger.LogError($"Municipality with code {toAdd?.Municipality?.Code ?? "NONE"} not found for street address {toAdd?.Id}");
                    continue;
                }

                var newStreet = new ClsAddressStreet
                {
                    Id = toAdd.Id,
                    IsValid = true,
                    Municipality = municipality,
                    UpdateIdentifier = updateIdentifier,
                    NonCls = false
                };
                UpdateStreetNames(newStreet, toAdd);
                UpdateStreetNumbers(postalCodesDictionary, newStreet, toAdd);
                streetRep.Add(newStreet);
                created.Add(newStreet.Id);
                counter++;
            }

            return created;
        }

        private List<Guid> UpdateStreetAddresses(IUnitOfWorkWritable unitOfWork, Dictionary<Guid, VmStreetAddress> data)
        {
            var municipalityCodeDictionary = GetMunicipalityCodeDictionary(unitOfWork);
            var postalCodesDictionary = GetPostalCodesDictionary(unitOfWork);
            var streetIds = data.Values.Select(i => i.Id).ToList();

            var streetRep = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
            var existingOnes = streetRep.All()
                .Where(i => streetIds.Contains(i.Id))
                .Include(i => i.StreetNames)
                .Include(s => s.StreetNumbers)
                .ToList();
            foreach (var i in existingOnes)
            {
                var municipality = i?.Municipality?.Code != null ? municipalityCodeDictionary.TryGetOrDefault(i.Municipality.Code, null) : null;
                var downloaded = data[i.Id];
                i.IsValid = municipality != null;
                i.Municipality = municipality ?? i.Municipality;
                i.UpdateIdentifier = updateIdentifier;
                UpdateStreetNames(i, downloaded);
                UpdateStreetNumbers(postalCodesDictionary, i, downloaded);
            }

            var updatedStreetsById = existingOnes.Select(i => i.Id).ToList();

            return updatedStreetsById;
        }

        private bool UpdateStreetNames(ClsAddressStreet entity, VmStreetAddress model)
        {
            if (model == null) return false;
            Guid langFiId = languageCache.Get("fi");
            Guid langSvId = languageCache.Get("sv");
            Guid langEnId = languageCache.Get("en");

            UpdateStreetNameEntity(entity, langFiId, model?.Names?.Fi);
            UpdateStreetNameEntity(entity, langSvId, model?.Names?.Sv);
            UpdateStreetNameEntity(entity, langEnId, model?.Names?.En);
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
                .Select(g => g.OrderByDescending(sn => sn.Created).FirstOrDefault())
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
            }

            if (!postalCodes.TryGetValue(model.PostalCode.Code, out var postalCode))
            {
                logger.LogError($"Postal code {model.PostalCode.Code} not found for street address {street.Id}");
            }

            streetNumber.PostalCode = postalCode ?? streetNumber.PostalCode;
            streetNumber.EndCharacter = model.EndCharacter;
            streetNumber.EndCharacterEnd = model.EndCharacterEnd;
            streetNumber.EndNumber = model.EndNumber;
            streetNumber.EndNumberEnd = model.EndNumberEnd;
            streetNumber.IsEven = model.IsEven;
            streetNumber.IsValid = true;
            streetNumber.StartCharacter = model.StartCharacter;
            streetNumber.StartCharacterEnd = model.StartCharacterEnd;
            streetNumber.StartNumber = model.StartNumber;
            streetNumber.StartNumberEnd = model.StartNumberEnd;
            streetNumber.UpdateIdentifier = updateIdentifier;
            return streetNumber.Id;
        }

        private Dictionary<string, Municipality> GetMunicipalityCodeDictionary(IUnitOfWork unitOfWork)
        {
            var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();
            return municipalityRepo.All().ToList().ToDictionary(m => m.Code);
        }

        private Dictionary<string, PostalCode> GetPostalCodesDictionary(IUnitOfWork unitOfWork)
        {
            var postalCodesRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
            return postalCodesRepo.All().ToList().ToDictionary(pc => pc.Code);
        }

        private void UpdateStreetNameEntity(ClsAddressStreet street, Guid langId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var streetName = street.StreetNames.FirstOrDefault(j => j.LocalizationId == langId) ??
                                 new ClsAddressStreetName
                                 {
                                     LocalizationId = langId,
                                     ClsAddressStreetId = street.Id
                                 };

            streetName.Name = name;
            streetName.Name3 = name.SafeSubstring(0, 3)?.ToLower();
            streetName.UpdateIdentifier = updateIdentifier;
        }

        /// <inheritdoc cref="Delete"/>
        public List<Guid> InvalidateNonAffectedStreetAddresses(Guid? updateId = null)
        {
            logger.LogInformation("Invalidating addresses...");
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var repositoryStreet = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
                var toInvalidateStreets = repositoryStreet.All()
                    .Where(e => e.UpdateIdentifier != (updateId ?? updateIdentifier)
                        && e.IsValid
                        && !e.NonCls)
                    .ToList();
                toInvalidateStreets.ForEach(e => e.IsValid = false);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                logger.LogInformation($"Invalidated streets: {toInvalidateStreets.Count}");
                
                var repositoryNumbers = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
                var toInvalidateNumbs = repositoryNumbers.All()
                    .Where(e => e.UpdateIdentifier != (updateId ?? updateIdentifier)
                        && e.IsValid)
                    .ToList();
                toInvalidateNumbs.ForEach(e => e.IsValid = false);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                logger.LogInformation($"Invalidated street numbers: {toInvalidateNumbs.Count}");
                
                logger.LogInformation("Invalidating addresses done.");
                return toInvalidateStreets.Select(i => i.Id).ToList();
            });
        }

        public VmStreetAddressCollection LoadAll(string folder, DateTime created)
        {
            using (var client = storage.GetClient())
            {
                var files = client.ListFiles(folder).Where(file => file.LastModified >= created).ToList();

                if (!files.Any())
                {
                    throw new PtvAppException("Import files don't exist");
                }

                var result = new VmStreetAddressCollection {Results = new List<VmStreetAddress>(), Meta = new VmMeta()};
                int? totalResults = null;

                foreach (var file in files)
                {
                    // Workaround, because the whole filepath is already known.
                    var batch = LoadBatch(null, file.Name, client);
                    if (batch == null) throw new PtvAppException("Cls addresses loading failed"); //continue;
                    if (!totalResults.HasValue)
                    {
                        totalResults = batch.Meta.TotalResults;
                    }

                    result.Results.AddRange(batch.Results);
                }

                return result;
            }
        }

        private VmStreetAddressCollection LoadBatch(string path, string fileName, IStorageClient client)
        {
            var response = client.ReadFile(path, fileName);
            return JsonConvert.DeserializeObject<VmStreetAddressCollection>(response);
        }
    }
}
