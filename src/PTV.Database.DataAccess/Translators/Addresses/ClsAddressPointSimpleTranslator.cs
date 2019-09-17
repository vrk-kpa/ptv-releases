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
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<ClsAddressPoint, VmAddressSimple>), RegisterType.Transient)]
    internal class ClsAddressPointSimpleTranslator : Translator<ClsAddressPoint, VmAddressSimple>
    {
        public ClsAddressPointSimpleTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAddressSimple TranslateEntityToVm(ClsAddressPoint entity)
        {
            var validAddress = entity != null && 
                (entity.AddressStreetNumber?.IsValid == true ||
                 entity.AddressStreetNumber != null && entity.AddressStreetNumber.Coordinates.Any(x => x.CoordinateState == CoordinateStates.Ok.ToString()) ||
                 entity.Address.Coordinates.Any(x => x.CoordinateState == CoordinateStates.Ok.ToString()));
            var translation = CreateEntityViewModelDefinition<VmAddressSimple>(entity)
                .AddSimple(i => !validAddress , o => o.InvalidAddress)
                .AddSimple(i => i.Id, o => o.ClsPointId)
                .AddSimple(i => i.MunicipalityId, o => o.Municipality)
                .AddNavigation(i => i.AddressStreet, o => o.Street)
                .Propagation((i, o) =>
                {
                    var texts = o.Street?.Translation?.Texts;
                    o.StreetName = texts == null 
                        ? new Dictionary<string, string>()
                        : new Dictionary<string, string>(texts);
                })
                .AddNavigation(i => i.AddressStreetNumber, o => o.StreetNumberRange)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.PostalCode, o => o.PostalCode);

            return translation.GetFinal();
        }

        public override ClsAddressPoint TranslateVmToEntity(VmAddressSimple vModel)
        {
            var translation = CreateViewModelEntityDefinition<ClsAddressPoint>(vModel)
                .UseDataContextCreate(i => !i.ClsPointId.IsAssigned())
                .UseDataContextUpdate(i => i.ClsPointId.IsAssigned(), i => o => i.ClsPointId == o.Id)
                .AddSimple(i => true, o => o.IsValid)
                .AddSimple(i => i.Municipality ?? Guid.Empty, o => o.MunicipalityId)
                .AddSimple(i => i.StreetNumberRange?.Id, o => o.AddressStreetNumberId)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => new VmStreet() {IsValid  =  vModel.Coordinates.Any(c=>c.CoordinateState==CoordinateStates.Ok.ToString()) ,StreetNumbers = new List<VmStreetNumber>() { new VmStreetNumber() { PostalCode = i.PostalCode } }, Id = i.Street?.Id ?? Guid.Empty, MunicipalityId = i.Municipality ?? Guid.Empty, Translation = new VmTranslationItem() { Texts = i.StreetName, DefaultText = i.StreetName.FirstOrDefault().Value}}, o => o.AddressStreet)
                .Propagation((i,o) =>
                {
                    i.Street = new VmStreet() {Id = o.AddressStreet?.Id ?? Guid.Empty};
                })
                .AddNavigation(i => i, o => o.AddressStreetNumber)
                .Propagation((i,o) =>
                {
                    if (o.AddressStreetNumber != null && o.AddressStreet != null && o.AddressStreetNumber.ClsAddressStreet == null && !o.AddressStreetNumber.ClsAddressStreetId.IsAssigned())
                    {
                        o.AddressStreetNumber.ClsAddressStreet = o.AddressStreet;
                    }
                })
                .AddSimple(i => i.PostalCode?.Id ?? Guid.Empty, o => o.PostalCodeId)
                .AddNavigation(i => i.PostalCode?.Code, o => o.PostalCode);

            return translation.GetFinal();
        }       
    }
}