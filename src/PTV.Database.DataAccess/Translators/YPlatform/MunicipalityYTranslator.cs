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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Models.YPlatform;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.YPlatform
{
    [RegisterService(typeof(ITranslator<Municipality, VmYCodedArea>), RegisterType.Transient)]
    internal class MunicipalityYTranslator : Translator<Municipality, VmYCodedArea>
    {
        public MunicipalityYTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmYCodedArea TranslateEntityToVm(Municipality entity)
        {
            throw new NotImplementedException();
        }

        public override Municipality TranslateVmToEntity(VmYCodedArea vModel)
        {
            var names = new List<VmJsonName>();
            
            var definition = CreateViewModelEntityDefinition<Municipality>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.CodeValue == o.Code, def => def.UseDataContextCreate(i => true))
                .Propagation((i, o) =>
                {
                    names = i.PrefLabel.Select(x => new VmJsonName
                    {
                        Language = x.Key,
                        Name = x.Value,
                        OwnerReferenceId = o.Id
                    }).ToList();
                })
                .AddCollection(i => names, o => o.MunicipalityNames)
                .AddNavigation(i => i.CodeValue, o => o.Code)
                .AddSimple(i => i.IsValid, o => o.IsValid);
                
            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<MunicipalityName, VmJsonName>), RegisterType.Transient)]
    internal class MunicipalityNameJsonTranslator : Translator<MunicipalityName, VmJsonName>
    {
        public MunicipalityNameJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonName TranslateEntityToVm(MunicipalityName entity)
        {
            throw new NotImplementedException();
        }

        public override MunicipalityName TranslateVmToEntity(VmJsonName vModel)
        {
            var languageId = languageCache.Get(vModel?.Language);
            
            return CreateViewModelEntityDefinition<MunicipalityName>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), 
                    i => o => i.OwnerReferenceId == o.MunicipalityId && o.LocalizationId == languageId, 
                    def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => i.Name, o => o.Name)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .GetFinal();
        }
    }
}
