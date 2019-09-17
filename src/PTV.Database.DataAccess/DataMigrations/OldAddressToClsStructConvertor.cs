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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.StreetData.Responses;
using PTV.Framework;

namespace PTV.Database.DataAccess.DataMigrations
{
    [RegisterService(typeof(IOldAddressToClsStructConverter), RegisterType.Transient)]
    public class OldAddressToClsStructConverter : IOldAddressToClsStructConverter
    {
        private readonly IContextManager contextManager;


        private string Flat(ClsAddressStreetNumber strNum)
        {
            return "'even"+strNum.IsEven+"."+strNum.StartNumber + "-" + strNum.EndNumber + "(" + strNum.StartNumberEnd+"-" + strNum.EndNumberEnd+ "), postal-"+strNum.PostalCodeId+" '";
        }
        
        private void FixWrongAssignmentsClsAddresses()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var repPoint = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                var wrongAssign = repPoint.AllPure().Where(i => i.StreetNumber != null && i.AddressStreetNumber != null && i.AddressStreet.StreetNumbers.Any() &&  i.AddressStreetNumber.ClsAddressStreetId != i.AddressStreetId).Include(i=> i.AddressStreetNumber).Include(i => i.AddressStreet).ThenInclude(i => i.StreetNumbers).ToList();
                Console.WriteLine($"Wrongly assigned pair street and number = {wrongAssign.Count}");
                var replaced = 0;
                var notFixed = 0;
                wrongAssign.ForEach(p =>
                {
                    var even = p.AddressStreetNumber.IsEven;
                    var startNumber = p.AddressStreetNumber.StartNumber;
                    var startNumberEnd = p.AddressStreetNumber.StartNumberEnd;
                    var endNumber = p.AddressStreetNumber.EndNumber;
                    var endNumberEnd = p.AddressStreetNumber.EndNumberEnd;

                    var replace = p.AddressStreet.StreetNumbers.FirstOrDefault(i => i.PostalCodeId == p.PostalCodeId &&
                        i.IsEven == even && i.StartNumber == startNumber && i.EndNumber == endNumber && i.StartNumberEnd == startNumberEnd && i.EndNumberEnd == endNumberEnd);
                    if (replace == null)
                    {
                        var numericPart = ParseDigitsPart(p.StreetNumber);
                        even = numericPart.IsEven();
                        startNumber = numericPart;
                        endNumber = numericPart;
                        replace = p.AddressStreet.StreetNumbers.FirstOrDefault(i => i.PostalCodeId == p.PostalCodeId &&
                                                                                    i.IsEven == even && (i.StartNumber <= startNumber && i.EndNumber >= endNumber) ||
                                                                                    (i.StartNumber == startNumber && i.EndNumber == 0));
                    }
                    if (replace != null)
                    {
                        p.AddressStreetNumberId = replace.Id;
                        replaced++;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(p.StreetNumber))
                        {
                            p.AddressStreetNumberId = null;
                        }
                        notFixed++;
                        //Console.WriteLine($"{p.AddressStreetId} Bad: '{p.StreetNumber}'// {Flat(p.AddressStreetNumber)} // {string.Join(':',p.AddressStreet.StreetNumbers.Select(Flat))}");
                    }
                });
                Console.WriteLine($"Fixed = {replaced}, not fixed = {notFixed}");
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }
        
        

        private void RemoveUnusedAndInvalidClsAddresses()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var repNum = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
                var repStreet = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
                var stopWatch = new Stopwatch();
                stopWatch.Restart();
                var invalidNums = repNum.All().Where(i => !i.IsValid && !i.AddressPoints.Any()).ToList();
                Console.WriteLine($"invalid street numbers = {invalidNums.Count}");
                invalidNums.ForEach(e => repNum.Remove(e));
                
                var invalidStreets = repStreet.All().Where(i => !i.IsValid && !i.AddressPoints.Any() && !i.StreetNumbers.Any(n => n.AddressPoints.Any()) ).ToList();
                Console.WriteLine($"invalid streets  = {invalidStreets.Count}");
                invalidStreets.ForEach(e =>
                {
                    e.StreetNumbers.ForEach(n => repNum.Remove(n));
                    repStreet.Remove(e);
                });
                unitOfWork.Save(SaveMode.AllowAnonymous);
                stopWatch.Stop();
                Console.WriteLine($"CleanDone in {stopWatch.ElapsedMilliseconds}");
            });
        }
        
        
        public void RemoveInvalidDuplicates()
        {
            FixWrongAssignmentsClsAddresses();
            RemoveUnusedAndInvalidClsAddresses();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var repNum = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
                var stopWatch = new Stopwatch();
                stopWatch.Restart();
                int removed = 0;
                int groups = 0;
                var dups = repNum.All().Include(i => i.AddressPoints).GroupBy(i => new {i.ClsAddressStreetId, i.StartNumber, i.EndNumber, i.StartNumberEnd, i.EndNumberEnd}).ToList();
                dups.Where(i => i.Count() > 1).ForEach(g =>
                {
                    groups++;
                    var validNumItem = g.FirstOrDefault(j => j.IsValid) ?? g.First();
                    g.Where(i => !i.IsValid).ForEach(ch =>
                    {
                        ch.AddressPoints.ForEach(p => p.AddressStreetNumberId = validNumItem.Id);
                    });
                    g.Where(r => r != validNumItem).ForEach(rm =>
                    {
                        removed++;
                        repNum.Remove(rm);
                    });
                });

                unitOfWork.Save(SaveMode.AllowAnonymous);
                stopWatch.Stop();
                Console.WriteLine($"Street number duplicates {removed} removed (of {groups} groups) in {stopWatch.ElapsedMilliseconds}");
            });
            RemoveUnusedAndInvalidClsAddresses();
        }
        
        
        
        
        
        public void ClearAllClsAddresses()
        {
            contextManager.ExecuteWriter(unitOfWork =>
                {
                    unitOfWork.CreateRepository<IClsAddressPointRepository>().DeleteAll();
                    unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>().DeleteAll();
                    unitOfWork.CreateRepository<IClsAddressStreetNameRepository>().DeleteAll();
                    unitOfWork.CreateRepository<IClsAddressStreetRepository>().DeleteAll();
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                }
            );
        }
        
        public void CreateInsertScriptForBrokenStreetAddresses()
        {
            var streetAddresses = File.ReadAllText("brokenstreets.dat").Split('\n').Where(i => !string.IsNullOrEmpty(i)).Select(i =>
            {
                try
                {
                    return Guid.Parse(i);
                }
                catch
                {
                    Console.WriteLine($"'{i}'");
                }

                return Guid.Empty;
            }).Where(i => i.IsAssigned()).ToList();
            
            var toFixData = contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                return rep.All().Where(i => streetAddresses.Contains(i.AddressId)).ToList();
            });

            var lastOperationIdentifier = Guid.NewGuid();
            var lastOperationTimeStamp = DateTime.UtcNow;
            var insertScript = toFixData.Select(i => $"INSERT INTO public.\"ClsAddressPoint\" VALUES ('{i.Created}', '{i.CreatedBy}', '{i.Modified}', '{i.ModifiedBy}', '{Guid.NewGuid()}', '{i.PostalCodeId}', '{i.MunicipalityId}', '{i.AddressStreetId}', '{i.AddressStreetNumberId}', '{i.AddressId}', '{i.StreetNumber}', {i.IsValid.ToString().ToLower()}, '{lastOperationIdentifier}', '{lastOperationTimeStamp}', {EntityState.Added.ToString()})").ToList(); 
            File.WriteAllText("importbrokenstreets.sql", string.Join(";\r\n", insertScript));
        }
        
        
        public OldAddressToClsStructConverter(IServiceProvider serviceProvider)
        {
            this.contextManager = serviceProvider.GetService<IContextManager>();
        }

        private bool HasEqualMunicipality(Dictionary<Guid, Guid> data, Guid postalCode1, Guid postalCode2)
        {
            Guid m1 = data.TryGetOrDefault(postalCode1);
            Guid m2 = data.TryGetOrDefault(postalCode2);
            return m1.IsAssigned() && m1 == m2;
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

//        private List<Tuple<int,Guid>> ComposeStreetNumbers(IEnumerable<ClsAddressStreetNumber> streetNumbers, bool validOnly = true)
//        {
//            var result = new List<Tuple<int,Guid>>();
//            streetNumbers.Where(i => !validOnly || (validOnly && i.IsValid)).ForEach(n =>
//            {
//                for (int i = n.StartNumber; i <= Math.Max(n.EndNumber.ValueOrDefaultIfZero(n.StartNumber), n.EndNumberEnd); i+=2)
//                {
//                    result.Add(new Tuple<int, Guid>(i, n.Id));
//                }
//            });
//            return result;
//        }
//
//        private Guid StreetNumIntervalsOverlapping(List<Tuple<int,Guid>> streetNums, int numLow, int numHigh)
//        {
//            var start = streetNums.FirstOrDefault(i => i.Item1 == numLow);
//            //var end = streetNums.FirstOrDefault(i => i.Item1 == numHigh);
//            return (start != null /*&& end != null*/) ? start.Item2 : Guid.Empty;

//            if (!streetNums.Any(i => i.Item1 == numLow) || !streetNums.Any(i => i.Item1 == numHigh))
//            {
//                return Guid.Empty;
//            }
//
//            bool found = true;
//            for (int i = numLow; i <= numHigh; i++)
//            {
//                if (!streetNums.Any(j => j.Item1 == i))
//                {
//                    found = false;
//                    break;
//                }
//            }
//
//            if (found)
//            {
//                return streetNums.First(i => i.Item1 == numLow).Item2;
//            }
//            for (int i = numLow; i <= numHigh; i+=2)
//            {
//                if (!streetNums.Any(j => j.Item1 == i))
//                {
//                    return Guid.Empty;
//                }
//            }
//            return streetNums.First(i => i.Item1 == numLow).Item2;
//        }

        private ClsAddressStreetNumber GetStreetNumberInterval(ICollection<ClsAddressStreetNumber> streetNumbers, int streetNumberIntLow, int streetNumberIntHigh, char streetNumberChar)
        {
            if (streetNumberIntHigh < streetNumberIntLow)
            {
                streetNumberIntHigh = streetNumberIntLow;
            }

            bool isEven = streetNumberIntLow.IsEven();
            return streetNumbers.FirstOrDefault(i => i.IsEven == isEven && i.StartNumber <= streetNumberIntLow && Math.Max(i.EndNumber.ValueOrDefaultIfZero(i.StartNumber), i.EndNumberEnd) >= streetNumberIntLow);



//            var availableNumbers = ComposeStreetNumbers(streetNumbers);
//            var assignedStreetNumber = StreetNumIntervalsOverlapping(availableNumbers, streetNumberIntLow, streetNumberIntHigh);
//            if (!assignedStreetNumber.IsAssigned())
//            {
//                availableNumbers = ComposeStreetNumbers(streetNumbers, false);
//                assignedStreetNumber = StreetNumIntervalsOverlapping(availableNumbers, streetNumberIntLow, streetNumberIntHigh);
//            }
//            return assignedStreetNumber.IsAssigned() ? streetNumbers.First(i => i.Id == assignedStreetNumber) : null;
        }

        public void ConvertAddresses()
        {
            var noMunicipalityAddresses = new List<Guid>();
            var noPostalCodeAddresses = new List<Guid>();
            var noStreetNameAddresses = new List<Guid>();
            var noStreetNumberAddresses = new List<Guid>();
            var emptyStreetNumberAddresses = new List<Guid>();
            var wrongStreetNumberAddresses = new List<Guid>();
            var nonUniqueAddresses = new List<Guid>();
            var convertedAddresses = new List<Guid>();
            int totalAddrs = 0;
            contextManager.ExecuteWriter(unitOfWork =>
            {
//                Console.WriteLine("Loading data...");
//                var streetDataRaw = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>().All().Where(i => i.ClsAddressStreet.IsValid && i.Name != null && i.Name != "").Include(i => i.ClsAddressStreet).ThenInclude(i => i.StreetNumbers).ToList();
//                var streetData = streetDataRaw.GroupBy(i => i.Name.ToLower().Trim()).ToDictionary(i => i.Key, i => i.Select(j => j).Where(m => m.ClsAddressStreet.IsValid).ToList());
//                var postalCodes = unitOfWork.CreateRepository<IPostalCodeRepository>().All().Where(i => i.MunicipalityId != null).ToList().ToDictionary(i => i.Id, i => i.MunicipalityId.Value);
//   
//                Console.WriteLine("Data loaded, starting conversion...");
//                var addressStreetRep = unitOfWork.CreateRepository<IAddressStreetRepository>();
//                var clsAddressPointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
//                var mainConvertQuery = addressStreetRep.All().Where(i => i.Address.Type.Code == AddressTypeEnum.Street.ToString());
//                noPostalCodeAddresses = mainConvertQuery.Where(i => i.PostalCodeId == null && i.Address.Type.Code == AddressTypeEnum.Street.ToString()).Select(i => i.AddressId).ToList();
//                
//                var allOldAddrs = mainConvertQuery.Where(i => i.PostalCodeId != null).Include(i => i.StreetNames);
//                int counter = 0;
//                
//                allOldAddrs.Batch(1000).ForEach(data =>
//                    {
//                        var newAddressPoints = new List<ClsAddressPoint>();
//                        Console.WriteLine($"Converting batch {++counter}...");
//                        data.ForEach(addr =>
//                        {
//                            totalAddrs++;
//                            clsAddressPointRep.BatchDelete(i => i.AddressId, addr.AddressId);
//                            Guid muniId = addr.MunicipalityId.IsAssigned() ? addr.MunicipalityId.Value : postalCodes.TryGetOrDefault(addr.PostalCodeId.Value, Guid.Empty);
//                            if (string.IsNullOrEmpty(addr.StreetNumber))
//                            {
//                                emptyStreetNumberAddresses.Add(addr.AddressId);
//                                CreateNonClsAddress(clsAddressPointRep, addr, muniId, 0, 0);
//                                return;
//                            }
//                            (var streetNumberIntLower, var streetNumberIntHigher, var streetNumberChar) = ExtractStreetNumber(addr.StreetNumber);
//                            
//                            if (streetNumberIntLower == 0)
//                            {
//                                wrongStreetNumberAddresses.Add(addr.AddressId);
//                                CreateNonClsAddress(clsAddressPointRep, addr, muniId, streetNumberIntLower, streetNumberIntHigher);
//                                return;
//                            }
//                            
//                            var availableStreets = new HashSet<ClsAddressStreet>();
//                            addr.StreetNames.Where(j => !string.IsNullOrEmpty(j.Name?.Trim())).ForEach(sName =>
//                            {
//                                var streetName = streetData.TryGetOrDefault(sName.Name.ToLower());
//                                if (streetName == null) return;
//                                streetName.ForEach(j => availableStreets.Add(j.ClsAddressStreet));
//                            });
//                            var matchingStreets = availableStreets.Where(i => i.StreetNumbers.Select(j => j.PostalCodeId).Contains(addr.PostalCodeId.Value)).ToList();
//                            if (matchingStreets.Count > 1)
//                            {
//                                matchingStreets = matchingStreets.Where(i => i.MunicipalityId == muniId).ToList();
//                            }
//
//                            if (matchingStreets.IsNullOrEmpty())
//                            {
//                                matchingStreets = availableStreets.Where(i => i.StreetNumbers.Any(j => HasEqualMunicipality(postalCodes, j.PostalCodeId, addr.PostalCodeId.Value))).ToList();
//                            }
//                            if (matchingStreets.IsNullOrEmpty())
//                            {
//                                matchingStreets = availableStreets.Where(i => i.MunicipalityId == muniId).ToList();
//                            }
//
//                            var assignStreets = matchingStreets.Select(i => new {Street = i, NumberInterval = GetStreetNumberInterval(i.StreetNumbers, streetNumberIntLower, streetNumberIntHigher, streetNumberChar)}).Where(i => i.NumberInterval != null).ToList();
//                            
//                            if (assignStreets.IsNullOrEmpty())
//                            {
//                                if (matchingStreets.Any())
//                                {
//                                    noStreetNumberAddresses.Add(addr.AddressId);
//                                    CreateNonClsAddress(clsAddressPointRep, addr, muniId, streetNumberIntLower, streetNumberIntHigher);
//                                    return;
//                                }
//
//                                noStreetNameAddresses.Add(addr.AddressId);
//                                CreateNonClsAddress(clsAddressPointRep, addr, muniId, streetNumberIntLower, streetNumberIntHigher);
//                                return;
//                            }
//                            if (assignStreets.Count > 1)
//                            {
//                                nonUniqueAddresses.Add(addr.AddressId);
//                                CreateNonClsAddress(clsAddressPointRep, addr, muniId, streetNumberIntLower, streetNumberIntHigher);
//                                return;
//                            }
//                            var selectedStreet = assignStreets.First();
//                            newAddressPoints.Add(new ClsAddressPoint()
//                            {
//                                Id = addr.AddressId,
//                                AddressId = addr.AddressId,
//                                MunicipalityId = muniId,
//                                PostalCodeId = addr.PostalCodeId.Value,
//                                StreetNumber = addr.StreetNumber,
//                                AddressStreet = selectedStreet.Street,
//                                AddressStreetId = selectedStreet.Street.Id,
//                                AddressStreetNumber = selectedStreet.NumberInterval,
//                                AddressStreetNumberId = selectedStreet.NumberInterval.Id,
//                                IsValid = selectedStreet.NumberInterval.IsValid && selectedStreet.Street.IsValid
//                            });
//                            convertedAddresses.Add(addr.AddressId);
//                        });
//                        clsAddressPointRep.BatchInsert(newAddressPoints, "PTVapp");
//                    });
//                Console.WriteLine("Saving new invalid addresses...");
//                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
            Console.WriteLine($"done.");
            Console.WriteLine($"Total count of addresses {totalAddrs}");
            Console.WriteLine($"Converted addresses {convertedAddresses.Count}");
            Console.WriteLine($"No postal code addresses {noPostalCodeAddresses.Count}");
            Console.WriteLine($"No municipality addresses {noMunicipalityAddresses.Count}");
            Console.WriteLine($"No street name addresses {noStreetNameAddresses.Count}");
            Console.WriteLine($"No street number addresses {noStreetNumberAddresses.Count}");
            Console.WriteLine($"Empty street number addresses {emptyStreetNumberAddresses.Count}");
            Console.WriteLine($"NonUnique addresses {nonUniqueAddresses.Count}");
            Console.WriteLine($"Invalid addresses created {noStreetNameAddresses.Count+noStreetNumberAddresses.Count+emptyStreetNumberAddresses.Count+nonUniqueAddresses.Count}");
        }

//        private void CreateNonClsAddress(IClsAddressPointRepository clsAddressPointRep, AddressStreet addr, Guid muniId, int streetNumberIntLower, int streetNumberIntHigher)
//        {
//            Guid streetId = Guid.NewGuid();
//            clsAddressPointRep.Add(new ClsAddressPoint()
//            {
//                Id = addr.AddressId,
//                AddressId = addr.AddressId,
//                MunicipalityId = muniId,
//                PostalCodeId = addr.PostalCodeId.Value,
//                StreetNumber = addr.StreetNumber,
//                AddressStreet = new ClsAddressStreet()
//                {
//                    Id = streetId,
//                    NonCls = true,
//                    IsValid = false,
//                    MunicipalityId = muniId,
//                    UpdateIdentifier = Guid.Empty,
//                    StreetNames = addr.StreetNames.Select(i => new ClsAddressStreetName()
//                    {
//                        NonCls = true,
//                        Name = i.Name,
//                        Name3 = i.Name.SafeSubstring(0, 3)?.ToLower(),
//                        LocalizationId = i.LocalizationId
//                    }).ToList()
//                },
//                AddressStreetNumber = new ClsAddressStreetNumber()
//                {
//                    Id = Guid.NewGuid(), NonCls = true, IsValid = false,
//                    IsEven = streetNumberIntLower.IsEven(), StartNumber = streetNumberIntLower, EndNumber = streetNumberIntHigher, ClsAddressStreetId = streetId, PostalCodeId = addr.PostalCodeId.Value, 
//                },
//                IsValid = false
//            });
//        }
//        
        public int ReValidateAssignedStreetNumbers()
        {
            return contextManager.ExecuteWriter(unitOfWork =>
                {
                    var addrPointRep = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                    addrPointRep.All().Where(i => i.IsValid && !i.AddressStreet.IsValid).Batch(1000).ForEach(toCheckList =>
                    {
                        toCheckList.ForEach(addr => addr.IsValid = false);
                    });
                    addrPointRep.All().Where(i => (i.IsValid && !i.AddressStreetNumber.IsValid) || (!i.IsValid && i.AddressStreetNumber.IsValid && i.AddressStreet.IsValid)).Include(i => i.AddressStreet).ThenInclude(i => i.StreetNumbers).Include(i => i.AddressStreetNumber).Batch(1000).ForEach(toCheckList =>
                    {
                        toCheckList.ForEach(addr =>
                        {
                            (var streetNumberIntLower, var streetNumberIntHigher, var streetNumberChar) = ExtractStreetNumber(addr.StreetNumber);
                            var rightStrNum = GetStreetNumberInterval(addr.AddressStreet.StreetNumbers, streetNumberIntLower, streetNumberIntHigher, streetNumberChar);
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
                    return unitOfWork.Save(SaveMode.AllowAnonymous);
                }
            );
        }
    }
    
    
    
    public interface IOldAddressToClsStructConverter {
        void ClearAllClsAddresses();
        int ReValidateAssignedStreetNumbers();
        void RemoveInvalidDuplicates();
    }
}
