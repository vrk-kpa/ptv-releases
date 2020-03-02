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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.Mass
{

    /// <summary>
    /// Generic view model for requesting publishing, coping, archiving
    /// </summary>
    public class VmMassDataModel<T> : IVmMassDataModel<T> where T : IVmLocalizedEntityModel
    {
        /// <summary>
        /// Identifier of process
        /// </summary>
        [JsonIgnore]
        public Guid Id { get; set; }
        /// <summary>
        /// Constructor of publishing
        /// </summary>
        public VmMassDataModel()
        {
            Services = new List<T>();
            Channels = new List<T>();
            GeneralDescriptions = new List<T>();
            Organizations = new List<T>();
            ServiceCollections = new List<T>();
        }

        /// <summary>
        /// List of service publishing models
        /// </summary>
        public IReadOnlyList<T> Services { get; set; }

        /// <summary>
        /// List of channels publishing models
        /// </summary>
        public IReadOnlyList<T> Channels { get; set; }

        /// <summary>
        /// List of organizations publishing models
        /// </summary>
        public IReadOnlyList<T> Organizations { get; set; }

        /// <summary>
        /// List of general descriptions publishing models
        /// </summary>
        public IReadOnlyList<T> GeneralDescriptions { get; set; }

        /// <summary>
        /// List of service collecions publishing models
        /// </summary>
        public IReadOnlyList<T> ServiceCollections { get; set; }

        /// <summary>
        /// Valid from for scheduling
        /// </summary>
        [JsonProperty("ValidFrom")]
        public long? PublishAt { get; set; }

        /// <summary>
        /// Valid to for scheduling
        /// </summary>
        [JsonProperty("ValidTo")]
        public long? ArchiveAt { get; set; }

        /// <summary>
        /// OrganizationId
        /// </summary>
        [JsonProperty("Organization")]
        public Guid OrganizationId { get; set; }
    }
}
