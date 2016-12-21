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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    public class VmLocationChannel : VmEntityStatusBase, IVmLocationChannel
    {
        public VmLocationChannelStep1 Step1Form { get; set; }
        public VmLocationChannelStep2 Step2Form { get; set; }
        public VmLocationChannelStep3 Step3Form { get; set; }
        public VmOpeningHoursStep Step4Form { get; set; }

        public VmLocationChannel()
        {
            Step1Form = new VmLocationChannelStep1();
            Step2Form = new VmLocationChannelStep2();
            Step3Form = new VmLocationChannelStep3();
            Step4Form = new VmOpeningHoursStep();
        }

        public LanguageCode Language { get; set; }
    }
}
