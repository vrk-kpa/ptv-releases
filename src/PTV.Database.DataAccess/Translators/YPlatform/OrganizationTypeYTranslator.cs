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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Models.YPlatform;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.YPlatform
{
    [RegisterService(typeof(ITranslator<OrganizationType, VmYCodeData>), RegisterType.Transient)]
    internal class OrganizationTypeYTranslator : Translator<OrganizationType, VmYCodeData>
    {
        private const string OntologyType = "ptvjultuottaja";
        
        public OrganizationTypeYTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmYCodeData TranslateEntityToVm(OrganizationType entity)
        {
            throw new System.NotImplementedException();
        }

        public override OrganizationType TranslateVmToEntity(VmYCodeData vModel)
        {
            var names = new List<VmJsonName>();
            var englishPrefix = new String(vModel.PrefLabel["en"].TakeWhile(char.IsLetter).ToArray());
            var code = "";
            
            var definition = CreateViewModelEntityDefinition<OrganizationType>(vModel)
                .UseDataContextUpdate(
                    i => true, 
                    i => o => i.IsValid == o.IsValid && i.CodeValue == o.YCode, 
                    def => def.UseDataContextUpdate(
                        i => true,
                        i => o => i.IsValid == o.IsValid && o.Names.Any(n => n.Name.StartsWith(englishPrefix)),
                        def2 => def2.UseDataContextCreate(i => true)))
                .Propagation((i, o) =>
                {
                    names = i.PrefLabel.Select(x => new VmJsonName
                    {
                        Language = x.Key,
                        Name = x.Value,
                        OwnerReferenceId = o.Id
                    }).ToList();
                    code = o.Code ?? i.CodeValue;
                })
                .AddCollection(i => names, o => o.Names)
                // Organization type codes are used across Open API and cannot be changed
                // We have to fill a new YPlatform Code field
                // If a new entity is being added, we set both YCode and Code to the same value
                .AddNavigation(i => i.CodeValue, o => o.YCode)
                .AddNavigation(i => code, o => o.Code)
                .AddNavigation(i => i.Uri, o => o.YUri)
                .AddNavigation(i => i.BroaderCode?.Uri, o => o.ParentYUri)
                .AddSimple(i => i.IsValid, o => o.IsValid)
                .AddNavigation(i => i.ShortName, o => o.Label)
                .AddNavigation(i => OntologyType, o => o.OntologyType);

            return definition.GetFinal();
        }
    }
}
