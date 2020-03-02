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
        private IOldAddressToClsStructConverter addressToClsStructConverter;

        public StreetDataService(IContextManager contextManager, ILanguageCache languageCache, ILogger<StreetDataService> logger, IOldAddressToClsStructConverter addressToClsStructConverter)
        {
            this.contextManager = contextManager;
            this.languageCache = languageCache;
            this.addressToClsStructConverter = addressToClsStructConverter;
            this.logger = logger;
        }

//        private Guid newBad = Guid.Parse("87984139-b5ad-424c-ad11-aad109473af8");
//        private Guid oldValid = Guid.Parse("9a67048e-58ed-4619-bd4e-d91952d089e0");


//        private class MapClsAddrs
//        {
//            public Guid PointId { get; set; }
//            public Guid StreetId { get; set; }
//            public Guid? StreetNumberId { get; set; }
//        }

        public void FixWrongImportCls()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var streetNumRep = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
                streetNumRep.AllPure().Where(i => i.EndNumber < i.StartNumber).ToList().ForEach(n => n.EndNumber = n.StartNumber);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var pointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                var toFix = pointRep.AllPure().Where(i => i.AddressStreetNumberId == null && !i.IsValid && i.StreetNumber != null).Include(i => i.AddressStreet)
                    .ThenInclude(i => i.StreetNumbers).ToList();
                var assigned = 0;
                toFix.ForEach(p =>
                {
                    var streetNumFormat = p.StreetNumber.Split('-');
                    var minNumStr = streetNumFormat.First().Trim();
                    var maxNumStr = streetNumFormat.Skip(1).FirstOrDefault()?.Trim()?.Split(' ')?.FirstOrDefault() ?? string.Empty;

                    if (!int.TryParse(minNumStr, out var strNumIntStart)) return;
                    int.TryParse(maxNumStr, out var strNumIntEnd);
                    if (strNumIntEnd < strNumIntStart)
                    {
                        strNumIntEnd = strNumIntStart;
                    }

                    var isEven = strNumIntStart.IsEven();
                    var availNumbers = p.AddressStreet.StreetNumbers.Where(i => i.IsEven == isEven &&
                        i.PostalCodeId == p.PostalCodeId && i.StartNumber <= strNumIntStart && ((Math.Max(Math.Max(i.EndNumber, i.EndNumberEnd), i.StartNumber) >= strNumIntEnd)) || (i.EndNumber == 0)).ToList();
                    var oldRefId = p.AddressStreetNumberId;
                    p.AddressStreetNumberId = availNumbers.FirstOrDefault(i => i.IsValid)?.Id ?? availNumbers.FirstOrDefault()?.Id;
                    if (oldRefId != p.AddressStreetNumberId)
                    {
                        assigned++;
                    }
                });
                unitOfWork.Save(SaveMode.AllowAnonymous);
                Console.WriteLine($"Numbers assigned {assigned}");
            });
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var pointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                pointRep.AllPure().Where(i => !i.IsValid && i.AddressStreet.IsValid && i.AddressStreetNumber.IsValid).ToList().ForEach(p => p.IsValid = true);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });

//            contextManager.ExecuteReader(unitOfWork =>
//            {
//                var pointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
//                var oldStreetUsages = pointRep.All().Where(i => i.AddressStreet.UpdateIdentifier == oldValid && !i.AddressStreet.IsValid).Count();
//                var newStreetUsages = pointRep.All().Where(i => i.AddressStreet.UpdateIdentifier == newBad && i.AddressStreet.IsValid).Count();
//                Console.WriteLine($"Old street usages {oldStreetUsages}");
//                Console.WriteLine($"New street usages {newStreetUsages}");
//            });
//            Console.WriteLine($"Old correct addresses switching to valid and new bad to invalid...");
//            contextManager.ExecuteWriter(unitOfWork =>
//            {
//                var streetRep = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
//                var streetNumRep = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
//                streetRep.All().Where(i => i.UpdateIdentifier == oldValid && !i.IsValid).ToList().ForEach(i => i.IsValid = true);
//                streetRep.All().Where(i => i.UpdateIdentifier == newBad && i.IsValid).ToList().ForEach(i => i.IsValid = false);
//                streetNumRep.All().Where(i => i.UpdateIdentifier == oldValid && !i.IsValid).ToList().ForEach(i => i.IsValid = true);
//                streetNumRep.All().Where(i => i.UpdateIdentifier == newBad && i.IsValid).ToList().ForEach(i => i.IsValid = false);
//                unitOfWork.Save(SaveMode.AllowAnonymous);
//            });
//            Console.WriteLine($"Old correct addresses switched back to valid.");
//            var mapOfSwitches = new List<MapClsAddrs>();
//            Console.WriteLine($"Creating map of changes...");
//            contextManager.ExecuteIsolatedReader(unitOfWork =>
//            {
//                var streetRep = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
//                var pointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
//                var toChangePoints = pointRep.All().Where(i => i.AddressStreet.UpdateIdentifier == newBad).OrderBy(i => i.Id).Include(i => i.AddressStreet)
//                    .ThenInclude(i => i.StreetNames).Include(i => i.AddressStreetNumber).ToList();
//                var groups = toChangePoints.GroupBy(i => new {i.AddressStreetId, i.AddressStreetNumberId});
//                Console.WriteLine($"Total groups {groups.Count()}");
//
//                groups.ForEach(strGroup =>
//                {
//                    var strPoint = strGroup.First();
//                    var strNumb = strPoint.AddressStreetNumber;
//                    var names = strPoint.AddressStreet.Names.Select(i => i.Name).ToList();
//                    var municipalityId = strPoint.AddressStreet.MunicipalityId;
//
//                    var isEven = strNumb?.IsEven;
//                    var startNumber = strNumb?.StartNumber;
//                    var startNumberEnd = strNumb?.StartNumberEnd;
//                    var endNumberEnd = strNumb?.EndNumberEnd;
//                    var endNumber = strNumb?.EndNumber;
//                    var postalCodeId = strPoint.PostalCodeId; // strNumb?.PostalCodeId;
//
//                    var semiQuery = streetRep.All().Where(i =>
//                        i.UpdateIdentifier == oldValid && i.MunicipalityId == municipalityId && i.StreetNames.Select(j => j.Name).All(m => names.Contains(m)));
//                    if (strNumb != null)
//                    {
//                        semiQuery = semiQuery.Where(i => i.StreetNumbers.Any(n => n.UpdateIdentifier == oldValid && n.IsEven == isEven
//                                                                                                                 && n.StartNumber == startNumber
//                                                                                                                 && n.StartNumberEnd == startNumberEnd
//                                                                                                                 && n.EndNumberEnd == endNumberEnd
//                                                                                                                 && n.EndNumber == endNumber
//                                                                                                                 && n.PostalCodeId == postalCodeId));
//
//                    }
//                    else
//                    {
//                        semiQuery = semiQuery.Where(i => i.StreetNumbers.Any(n => n.UpdateIdentifier == oldValid && n.PostalCodeId == postalCodeId));
//                    }
//
//                    var possibleStreets = semiQuery.Include(s => s.StreetNumbers).ToList();
//
//                    var toSwitchStreet = possibleStreets.FirstOrDefault();
//                    if (toSwitchStreet != null)
//                    {
//                        mapOfSwitches.AddRange(strGroup.Select(p => new MapClsAddrs()
//                        {
//                            PointId = p.Id,
//                            StreetId = toSwitchStreet.Id,
//                            StreetNumberId = strNumb == null
//                                ? (Guid?) null
//                                : toSwitchStreet.StreetNumbers.FirstOrDefault(n => n.IsEven == isEven
//                                                                                   && n.StartNumber == startNumber
//                                                                                   && n.StartNumberEnd == startNumberEnd
//                                                                                   && n.EndNumberEnd == endNumberEnd
//                                                                                   && n.EndNumber == endNumber
//                                                                                   && n.PostalCodeId == postalCodeId)?.Id
//
//                        }));
//                    }
//
//                });
//            });
//            Console.WriteLine($"Map created ({mapOfSwitches.Count}), writing changes...");
//            mapOfSwitches.Batch(100).ForEach(batch =>
//            {
//                contextManager.ExecuteWriter(unitOfWork =>{
//                    var pointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
//                    var ids = batch.Select(i => i.PointId).ToList();
//                    pointRep.All().Where(i => ids.Contains(i.Id)).ToList().ForEach(p =>
//                    {
//                        var newMap = batch.First(j => j.PointId == p.Id);
//                        p.AddressStreetId = newMap.StreetId;
//                        p.AddressStreetNumberId = newMap.StreetNumberId;
//                    });
//                    unitOfWork.Save(SaveMode.AllowAnonymous);
//                });
//                Console.WriteLine("Batch of 100 done.");
//            });
//            contextManager.ExecuteReader(unitOfWork =>
//            {
//                var pointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
//                var oldStreetUsages = pointRep.All().Where(i => i.AddressStreet.UpdateIdentifier == oldValid).Count();
//                var newStreetUsages = pointRep.All().Where(i => i.AddressStreet.UpdateIdentifier == newBad).Count();
//                Console.WriteLine($"Old street now valid usages {oldStreetUsages}");
//                Console.WriteLine($"New street now invalid usages {newStreetUsages}");
//            });
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

            foreach (var batch in updateBatches)
            {
                var data = batch.ToDictionary(i => i.Id);
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var existingOnes = UpdateStreetAddresses(unitOfWork, data);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreet>(EntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetName>(EntityMarker);
                    unitOfWork.AddPreSaveEntityProcessing<ClsAddressStreetNumber>(EntityMarker);
                    try
                    {
                        unitOfWork.Save(SaveMode.AllowAnonymous);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    newToAdd.AddRange(batch.Where(j => !existingOnes.Contains(j.Id)));
                    updatedStreets.AddRange(existingOnes);
                });
            }

            logger.LogInformation($"Updated streets: {updatedStreets.Count}");
            var createBatches = newToAdd.Batch(1000);
            foreach (var batch in createBatches)
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
            }

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

            var created = new List<Guid>();
            newToAdd.ForEach(toAdd =>
            {
                var municipality = toAdd?.Municipality?.Code != null ? municipalityCodeDictionary.TryGetOrDefault(toAdd.Municipality.Code) : null;
                if (municipality == null)
                {
                    logger.LogError($"Municipality with code {toAdd?.Municipality?.Code ?? "NONE"} not found for street address {toAdd?.Id}");
                    return;
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
            });
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

        public VmStreetAddressCollection LoadAll(string folder, DateTime created)
        {
            if (!Directory.Exists(folder))
            {
                throw new PtvAppException("Import directory doesn't exists");
            }

            var files = new DirectoryInfo(folder).GetFiles().Where(file=>file.LastWriteTime >= created).ToList();

            if (!files.Any())
            {
                throw new PtvAppException("Import files don't exist");
            }

            var result = new VmStreetAddressCollection {Results = new List<VmStreetAddress>(), Meta = new VmMeta()};
            int? totalResults = null;

            foreach (var file in files)
            {
                var batch = LoadBatch(file);
                if (batch == null) throw new PtvAppException("Cls addresses loading failed");  //continue;
                if (!totalResults.HasValue)
                {
                    totalResults = batch.Meta.TotalResults;
                }

                result.Results.AddRange(batch.Results);
            }

            return result;
        }

        private VmStreetAddressCollection LoadBatch(FileInfo fileInfo)
        {
            try
            {
                var response = File.ReadAllText(fileInfo.FullName);
                return JsonConvert.DeserializeObject<VmStreetAddressCollection>(response);
            }
            catch {}
            return null;
        }
    }
}
