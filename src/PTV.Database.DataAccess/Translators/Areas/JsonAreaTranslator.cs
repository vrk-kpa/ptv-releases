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

namespace PTV.Database.DataAccess.Translators.Municipalities
{

    [RegisterService(typeof(ITranslator<Area, VmJsonArea>), RegisterType.Scope)]
    internal class JsonAreaTranslator : Translator<Area, VmJsonArea>
    {
        public JsonAreaTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonArea TranslateEntityToVm(Area entity)
        {
            throw new NotSupportedException();
        }

        public override Area TranslateVmToEntity(VmJsonArea vModel)
        {
            bool isnew = false;
            var definition = CreateViewModelEntityDefinition<Area>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Code == o.Code && i.AreaTypeId == o.AreaTypeId, def =>
                    def.UseDataContextCreate(x => true, o => o.Id, i =>
                    {
                        isnew = true;
                        return Guid.NewGuid();
                    }));
            var entity = definition.GetFinal();
            if (!isnew)
            {
                vModel.Names.ForEach(x => x.OwnerReferenceId = entity.Id);
            }

            //Add AreaId
            vModel.AreaMunicipalities.ForEach(x => x.AreaId = entity.Id);
            definition
                .AddSimple(input => input.AreaTypeId, output => output.AreaTypeId)
                .AddNavigation(input => input.Code, output => output.Code)
                .AddCollectionWithRemove(input => input.Names, output => output.AreaNames, x => true)
                .AddCollectionWithRemove(input => input.AreaMunicipalities, o => o.AreaMunicipalities, x => true)
                .AddSimple(input => input.IsValid, o => o.IsValid);
            return entity;
        }
    }
}
