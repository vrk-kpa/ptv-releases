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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V10
{
    /// <summary>
    /// OPEN API V10 - View Model of electronic channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiElectronicChannelVersionBase" />
    public class V10VmOpenApiElectronicChannel : VmOpenApiElectronicChannelVersionBase
    {
        #region methods

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 10;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V9VmOpenApiElectronicChannel>();
            // SFIPTV-850
            if (AccessibilityClassification?.Count > 0)
            {
                var accessibility = this.AccessibilityClassification.GetSingle();
                if (accessibility != null)
                {
                    vm.AccessibilityClassificationLevel = accessibility.AccessibilityClassificationLevel;
                    vm.WCAGLevel = accessibility.WcagLevel;
                }
                vm.AccessibilityStatementWebPage = new List<V9VmOpenApiWebPage>();
                this.AccessibilityClassification?.ForEach(a =>
                {
                    if (!a.AccessibilityStatementWebPageName.IsNullOrEmpty() || !a.AccessibilityStatementWebPage.IsNullOrEmpty())
                    {
                        vm.AccessibilityStatementWebPage.Add(new V9VmOpenApiWebPage
                        {
                            Value = a.AccessibilityStatementWebPageName,
                            Url = a.AccessibilityStatementWebPage,
                            Language = a.Language
                        });
                    }
                });
            }
            // SFIPTV-362
            this.Services.ForEach(s => vm.Services.Add(s.ConvertToPreviousVersion()));
            return vm;
        }
        #endregion
    }
}
