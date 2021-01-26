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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Translators.OpenApi.Common;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<OrganizationEInvoicingAdditionalInformation, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiOrganizationEInvoicingAdditionalInformationTranslator : OpenApiTextBaseTranslator<OrganizationEInvoicingAdditionalInformation>
    {
        public OpenApiOrganizationEInvoicingAdditionalInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(OrganizationEInvoicingAdditionalInformation entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override OrganizationEInvoicingAdditionalInformation TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            var languageId = languageCache.Get(vModel.Language);

            var definition = CreateViewModelEntityDefinition<OrganizationEInvoicingAdditionalInformation>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => true)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .AddNavigation(i => i.Value, o => o.Text);

            return definition.GetFinal();
        }
    }
}
