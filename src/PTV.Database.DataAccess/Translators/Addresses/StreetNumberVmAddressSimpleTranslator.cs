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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<ClsAddressStreetNumber, VmAddressSimple>), RegisterType.Transient)]
    internal class StreetNumberVmAddressSimpleTranslator : Translator<ClsAddressStreetNumber, VmAddressSimple>
    {
        private readonly ILanguageOrderCache languageOrderCache;

        public StreetNumberVmAddressSimpleTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives)
        {
            this.languageOrderCache = cacheManager.LanguageOrderCache;
        }

        public override VmAddressSimple TranslateEntityToVm(ClsAddressStreetNumber entity)
        {
            throw new System.NotImplementedException();
        }

        public override ClsAddressStreetNumber TranslateVmToEntity(VmAddressSimple vModel)
        {
            var parsedStreetNumber = StreetService.ParseStreetNumber(vModel.StreetNumber);
            try
            {
                var definition = CreateViewModelEntityDefinition<ClsAddressStreetNumber>(vModel)
                    .UseDataContextUpdate(
                        i => true,
                        i => o => (i.Street.Id == o.ClsAddressStreetId) &&  (parsedStreetNumber >= o.StartNumber && (parsedStreetNumber <= o.EndNumber || parsedStreetNumber <= o.EndNumberEnd || o.EndNumber == 0))&& (o.PostalCode.Code == i.PostalCode.Code || i.PostalCode.Id == o.PostalCodeId),
                        def =>
                        {
                            var searchedStreetNames = vModel.StreetName
                                .OrderBy(s => languageOrderCache.Get(languageCache.Get(s.Key)))
                                .Select(s => new 
                                { 
                                    LanguageId = languageCache.Get(s.Key), 
                                    Name = s.Value?.Trim().ToLower(),
                                    Name3 = s.Value?.Trim()?.ToLower()?.SafeSubstring(0, 3)
                                }).ToList();
                            bool found = false;
                            foreach (var streetName in searchedStreetNames)
                            {
                                found = true;
                                def.UseDataContextUpdate(
                                    i => true,
                                    i => o => o.ClsAddressStreet.StreetNames.Any(j =>
                                                  j.LocalizationId == streetName.LanguageId && j.Name3 == streetName.Name3 &&
                                                  j.Name.ToLower().Contains(streetName.Name)) && (o.PostalCode.Code == i.PostalCode.Code || i.PostalCode.Id == o.PostalCodeId) &&
                                              (parsedStreetNumber >= o.StartNumber &&
                                               (parsedStreetNumber <= o.EndNumber || parsedStreetNumber <= o.EndNumberEnd || o.EndNumber == 0) ), d => found = false, i => o => o.IsValid, i => o => o.NonCls);
                                if (found) break;
                            }
                            if (!found)
                            {
                                CreateIfNotFound(vModel, def); 
                            }
                        },i => o => o.IsValid, i => o => o.NonCls);
                if (vModel.Coordinates != null &&
                    vModel.Coordinates.Any(c => c.CoordinateState.ToLower() == "ok"))
                {
                    var validCoordinate = vModel.Coordinates.First(c => c.CoordinateState.ToLower() == "ok");
                    definition.Propagation((i, o) =>
                        {
                            if (o.Id.IsAssigned())
                            {
                                i.Coordinates?.ForEach(c => c.OwnerReferenceId = o.Id);
                            }
                        })
                        .AddCollectionWithRemove(i => new List<VmCoordinate>{validCoordinate}, o => o.Coordinates,  x => true)
                        .AddSimple(i => true, o => o.IsValid);
                }

                return definition.GetFinal();
            }
            // Return null in case entity was not found, since street number range is optional
            catch (DbEntityNotFoundException)
            {
                return null;
            }
        }

        private void CreateIfNotFound(VmAddressSimple vModel, ITranslationDefinitionsForVersioning<VmAddressSimple, ClsAddressStreetNumber> def)
        {
            var parsedNumber = StreetService.ParseStreetNumber(vModel.StreetNumber) ?? -1;
            def.UseDataContextCreate(i => true);

            def.AddSimple(i => false, o => o.IsValid)
                .AddSimple(i => i.Street?.Id ?? Guid.Empty, o => o.ClsAddressStreetId)
                .AddSimple(i => parsedNumber % 2 == 0, o => o.IsEven)
                .AddSimple(i => true, o => o.NonCls)
                .AddSimple(i => parsedNumber, o => o.EndNumber)
                .AddSimple(i => parsedNumber, o => o.StartNumber)
                .AddSimple(i => i.PostalCode?.Id ?? Guid.Empty, o => o.PostalCodeId);
//            
//            if (vModel.Coordinates != null &&
//                vModel.Coordinates.Any(c => c.CoordinateState.ToLower() == "ok"))
//            {
//                def.AddCollection(i => i.Coordinates, o => o.Coordinates);
//            }
//            else
//            {
//                throw new DbEntityNotFoundException(
//                    $"{CoreMessages.EntityNotFoundToUpdate}. {nameof(VmAddressSimple)} - {nameof(ClsAddressStreetNumber)}");
//            }
        }

//        // TODO: this requires an optimization, since it has a serious impact on the DB and part of the code is
//        // duplicated from PTV.Database.DataAccess.Translators.Addresses.StreetTranslator:TranslateVmToEntity
//        private bool SelectRelatedEntity(VmAddressSimple input, ClsAddressStreetNumber output)
//        {
//            var searchedStreetName = input.StreetName
//                .OrderBy(s => languageOrderCache.Get(languageCache.Get(s.Key)))
//                .Select(s => new 
//                { 
//                    LanguageId = languageCache.Get(s.Key), 
//                    Name = s.Value?.Trim(),
//                    Name3 = s.Value?.Trim()?.ToLower()?.SafeSubstring(0, 3)
//                })
//                .FirstOrDefault();
//
//            var isAddressInRange = StreetService.IsAddressNumberInRange(
//                input.StreetNumber,
//                output.StartNumber,
//                Math.Max(output.EndNumber.ValueOrDefaultIfZero(output.StartNumber), output.EndNumberEnd),
//                output.IsEven);
//
//            var arePostalCodesEqual = ArePostalCodesEqual(input, output);
//            
//            return isAddressInRange
//                   && output.IsValid
//                   && arePostalCodesEqual
//                   && output.ClsAddressStreet.StreetNames.Any(osn =>
//                       osn.LocalizationId == searchedStreetName?.LanguageId
//                       && osn.Name3 == searchedStreetName.Name3
//                       && osn.Name == searchedStreetName.Name);
//        }

//        private bool ArePostalCodesEqual(VmAddressSimple input, ClsAddressStreetNumber output)
//        {
//            if (input.PostalCode == null || output.PostalCode == null)
//            {
//                return false;
//            }
//
//            return output.PostalCode.Code == input.PostalCode.Code || output.PostalCodeId == input.PostalCode.Id;
//        }
    }
}