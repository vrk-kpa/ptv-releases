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
using System.Threading.Tasks;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Attributes;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal partial class Versioning : EntityIdentifierBase
    {
        public Versioning()
        {
            ChildrenVersions = new HashSet<Versioning>();
            Services = new HashSet<ServiceVersioned>();
            Channels = new HashSet<ServiceChannelVersioned>();
            Organizations = new HashSet<OrganizationVersioned>();
            ServiceCollections = new HashSet<ServiceCollectionVersioned>();
            GeneralDescriptions = new HashSet<StatutoryServiceGeneralDescriptionVersioned>();
        }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }

        public Guid? PreviousVersionId { get; set; }
        public Versioning PreviousVersion { get; set; }
        public ICollection<Versioning> ChildrenVersions { get; set; }

        public ICollection<ServiceVersioned> Services { get; set; }
        public ICollection<ServiceChannelVersioned> Channels { get; set; }
        public ICollection<OrganizationVersioned> Organizations { get; set; }
        public ICollection<ServiceCollectionVersioned> ServiceCollections { get; set; }
        public ICollection<StatutoryServiceGeneralDescriptionVersioned> GeneralDescriptions { get; set; }
        [NotForeignKey]
        public Guid? UnificRootId { get; set; }
        public string Meta { get; set; }

        public bool Ignored { get; set; }
    }
}
