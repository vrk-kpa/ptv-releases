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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Enums;
using System.Linq;
using System.Collections.Generic;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.YPlatform;

namespace PTV.Database.DataAccess.Translators.Areas
{

    [RegisterService(typeof(ITranslator<Area, VmYCodedArea>), RegisterType.Transient)]
    internal class JsonAreaTranslator : Translator<Area, VmYCodedArea>
    {
        private readonly ITypesCache typesCache;

        public JsonAreaTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) 
            : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmYCodedArea TranslateEntityToVm(Area entity)
        {
            throw new NotSupportedException();
        }

        public override Area TranslateVmToEntity(VmYCodedArea vModel)
        {
            var names = new List<VmJsonName>();
            var areaTypeId = typesCache.Get<AreaType>(vModel.AreaType.ToString());
            
            var definition = CreateViewModelEntityDefinition<Area>(vModel)
                .UseDataContextUpdate(i => true, 
                    i => o => i.CodeValue == o.Code && areaTypeId == o.AreaTypeId, 
                    def => def.UseDataContextCreate(i => true))
                .Propagation((i, o) =>
                {
                    names = i.PrefLabel.Select(x => new VmJsonName
                    {
                        Language = x.Key,
                        Name = x.Value,
                        OwnerReferenceId = o.Id
                    }).ToList();
                })
                .AddSimple(i => areaTypeId, o => o.AreaTypeId)
                .AddCollection(i => names, o => o.AreaNames)
                .AddNavigation(i => i.CodeValue, o => o.Code)
                .AddSimple(i => i.IsValid, o => o.IsValid);
                
            return definition.GetFinal();
        }
    }
}
