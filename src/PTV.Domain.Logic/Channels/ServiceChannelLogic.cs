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

using System.Collections.Generic;
using PTV.Domain.Model.Models;
using System.Linq;
using PTV.Domain.Logic.Address;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Domain.Logic.Channels
{
    [RegisterService(typeof(ServiceChannelLogic), RegisterType.Singleton)]
    public class ServiceChannelLogic
    {
        private readonly ChannelAttachmentLogic attachmentLogic;
        private readonly WebPageLogic webPageLogic;
        private readonly OpeningHoursLogic openingHoursLogic;
        private AddressLogic addressLogic;

        public ServiceChannelLogic(ChannelAttachmentLogic attachmentLogic, WebPageLogic webPageLogic, OpeningHoursLogic openingHoursLogic, AddressLogic addressLogic)
        {
            this.attachmentLogic = attachmentLogic;
            this.webPageLogic = webPageLogic;
            this.openingHoursLogic = openingHoursLogic;
            this.addressLogic = addressLogic;
        }

        public void PrefilterViewModel(VmElectronicChannel vm)
        {
            PrefilterViewModel(vm.Step1Form);
            PrefilterViewModel(vm.Step2Form);
        }

        public void PrefilterViewModel(VmElectronicChannelStep1 vm)
        {
            if (vm.UrlAttachments != null)
            {
                vm.UrlAttachments = attachmentLogic.PrefilterModel(vm.UrlAttachments).ToList();
            }
        }

        public void PrefilterViewModel(VmPrintableFormChannel vm)
        {
            PrefilterViewModel(vm.Step1Form);
        }

        public void PrefilterViewModel(VmPrintableFormChannelStep1 vm)
        {
            if (vm.UrlAttachments != null)
            {
                vm.UrlAttachments = attachmentLogic.PrefilterModel(vm.UrlAttachments).ToList();
            }
        }

        public void PrefilterViewModel(VmPhoneChannel vm)
        {
            PrefilterViewModel(vm.Step2Form);
        }

        public void PrefilterViewModel(VmLocationChannel vm)
        {
            PrefilterViewModel(vm.Step2Form);
            PrefilterViewModel(vm.Step3Form);
            PrefilterViewModel(vm.Step4Form);
        }

        public void PrefilterViewModel(VmLocationChannelStep2 vm)
        {
            webPageLogic.PrefilterModel(vm);
        }

        public void PrefilterViewModel(VmLocationChannelStep3 vm)
        {
            addressLogic.PrefilterModel(vm);
        }

        public void PrefilterViewModel(VmOpeningHoursStep vm)
        {
            if (vm == null) return;
            vm.ExceptionHours = openingHoursLogic.SetMissingHours(vm.ExceptionHours).ToList();
            vm.ExceptionHours = openingHoursLogic.PrefilterModel(vm.ExceptionHours).Cast<VmExceptionalHours>().ToList();
            vm.SpecialHours = openingHoursLogic.PrefilterModel(vm.SpecialHours).Cast<VmSpecialHours>().ToList();
        }
    }
}
