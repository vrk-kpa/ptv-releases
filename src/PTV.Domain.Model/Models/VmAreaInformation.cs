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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of AreaInformation
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmAreaInformation" />
    public class VmAreaInformation : IVmAreaInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmAreaInformation"/> class.
        /// </summary>
        public VmAreaInformation()
        {           
            AreaBusinessRegions = new List<Guid>();
            AreaHospitalRegions = new List<Guid>();
            AreaProvince = new List<Guid>();
            AreaMunicipality = new List<Guid>();
        }
        /// <summary>
        /// Gets or sets the service id(short id - only for new service)
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the are infomration type identifier.
        /// </summary>
        /// <value>
        /// The area information type identifier.
        /// </value>
        public Guid AreaInformationTypeId { get; set; }
        /// <summary>
        /// Gets or sets the list of HospitalRegions.
        /// </summary>
        /// <value>
        /// The list of HospitalRegions.
        /// </value>
        public List<Guid> AreaHospitalRegions { get; set; }
        /// <summary>
        /// Gets or sets the list of Municipality.
        /// </summary>
        /// <value>
        /// The list of Municipality.
        /// </value>
        public List<Guid> AreaMunicipality { get; set; }
        /// <summary>
        /// Gets or sets the list of Province.
        /// </summary>
        /// <value>
        /// The list of Province.
        /// </value>
        public List<Guid> AreaProvince { get; set; }
        /// <summary>
        /// Gets or sets the list of BusinessRegions.
        /// </summary>
        /// <value>
        /// The list of BusinessRegions.
        /// </value>
        public List<Guid> AreaBusinessRegions { get; set; }
    }
}
