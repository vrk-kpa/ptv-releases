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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Finto
{
    [RegisterService(typeof(ITranslator<OrganizationType, VmListItem>), RegisterType.Transient)]
    internal class OrganizationTypeTranslator : Translator<OrganizationType, VmListItem>
    {
        public OrganizationTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        /// <summary>
        /// translate OrganizationType to VmListItem
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override VmListItem TranslateEntityToVm(OrganizationType entity)
        {
            return CreateEntityViewModelDefinition<VmListItem>(entity)
                .AddPartial(i => i as IFintoItemBase)
                .GetFinal();
        }

        /// <summary>
        /// translate VmListItem to OrganizationType
        /// </summary>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public override OrganizationType TranslateVmToEntity(VmListItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationType, string>), RegisterType.Transient)]
    internal class OrganizationTypeStringTranslator : Translator<OrganizationType, string>
    {
        public OrganizationTypeStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        /// <summary>
        /// translate OrganizationType to string
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string TranslateEntityToVm(OrganizationType entity)
        {
            return CreateEntityViewModelDefinition<string>(entity)
                .AddNavigation(i=>i.Code, o=>o)
                .GetFinal();
        }

        /// <summary>
        /// translate string to OrganizationType
        /// </summary>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public override OrganizationType TranslateVmToEntity(string vModel)
        {
            return CreateViewModelEntityDefinition<OrganizationType>(vModel)
                .UseDataContextUpdate(input => true, input => output => input == output.Code)
                .GetFinal();
        }
    }


}
