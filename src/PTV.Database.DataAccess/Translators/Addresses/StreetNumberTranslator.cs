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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<ClsAddressStreetNumber, VmStreetNumber>), RegisterType.Transient)]
    internal class StreetNumberTranslator : Translator<ClsAddressStreetNumber, VmStreetNumber>
    {
        public StreetNumberTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmStreetNumber TranslateEntityToVm(ClsAddressStreetNumber entity)
        {
            var translation = CreateEntityViewModelDefinition<VmStreetNumber>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.StartNumber, o => o.StartNumber)
                .AddSimple(i => Math.Max(i.EndNumber.ValueOrDefaultIfZero(i.StartNumber), i.EndNumberEnd), o => o.EndNumber)
                .AddSimple(i => i.IsEven, o => o.IsEven)
                .AddSimple(i => i.IsValid, o => o.IsValid)
                .AddSimple(i => i.ClsAddressStreetId, o => o.StreetId)
                .AddNavigation(i => i.PostalCode, o => o.PostalCode)
                .AddNavigation(i => i.Coordinates.FirstOrDefault(), o => o.Coordinate);

            return translation.GetFinal();
        }

        public override ClsAddressStreetNumber TranslateVmToEntity(VmStreetNumber vModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
