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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework.ServiceManager;


namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for list of 
    /// </summary>
    public class VmTranslationOrderStateSaveOutputs : VmBase
    {
        /// <summary>
        /// Constructor of transaltion order state save output
        /// </summary>
        public VmTranslationOrderStateSaveOutputs()
        {
            Translations = new List<VmTranslationOrderStateOutputs>();
            Services = new List<VmServiceOutput>();
            Channels = new List<IVmChannel>();
            GeneralDescriptions = new List<VmGeneralDescriptionOutput>();
        }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The charge type id.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        ///  Gets or sets the Translation order states
        /// </summary>
        /// <value>
        /// The list of translation order state.
        /// </value>
        public IReadOnlyList<VmTranslationOrderStateOutputs> Translations { get; set; }
        /// <summary>
        ///  Gets or sets the Translation availability of Services
        /// </summary>
        /// <value>
        /// The list of vailability of Services
        /// </value>
        public IReadOnlyList<VmServiceOutput> Services { get; set; }
        /// <summary>
        ///  Gets or sets the Translation vailability of Channels
        /// </summary>
        /// <value>
        /// The list of translation vailability of Channels.
        /// </value>
        public IReadOnlyList<IVmChannel> Channels { get; set; }
        /// <summary>
        ///  Gets or sets the Translation vailability of GeneralDescriptions.
        /// </summary>
        /// <value>
        /// The list of translation vailability of GeneralDescriptions.
        /// </value>
        public IReadOnlyList<VmGeneralDescriptionOutput> GeneralDescriptions { get; set; }
    }
}
