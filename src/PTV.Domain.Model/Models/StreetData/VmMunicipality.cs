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
using PTV.Domain.Model.Models.Interfaces.StreetData;
using PTV.Domain.Model.Models.StreetData.Responses;

namespace PTV.Domain.Model.Models.StreetData
{
    /// <summary>
    /// A single municipality object.
    /// </summary>
    public class VmMunicipality : IScalarModel
    {
        /// <summary>
        /// Municipality code.
        /// </summary>
        public string Code { get; set; }

        /// <inheritdoc />
        public string Url { get; set; }

        /// <inheritdoc />
        public Guid Id { get; set; }

        /// <inheritdoc />
        public string Source { get; set; }

        /// <inheritdoc />
        public DateTime Created { get; set; }

        /// <inheritdoc />
        public DateTime? Modified { get; set; }
        
        /// <summary>
        /// Available name translations.
        /// </summary>
        public VmStreetNamesJson Names { get; set; }
        
        /// <summary>
        /// Type of the municipality.
        /// </summary>
        public VmMunicipalityType Type { get; set; }
        
        /// <summary>
        /// Language of the municipality.
        /// </summary>
        public List<string> Languages { get; set; }
        
        /// <summary>
        /// The purpose of this field is unknown to me as of now.
        /// </summary>
        public string LanguageRelation { get; set; }
        
        /// <summary>
        /// Link to the related magistrate.
        /// </summary>
        public VmClsNavigationEdge Magistrate { get; set; }
        
        /// <summary>
        /// Link to the region to which the municipality belongs.
        /// </summary>
        public VmClsNavigationEdge Region { get; set; }
        
        /// <summary>
        /// Link to the health care district to which the municipality belongs.
        /// </summary>
        public VmClsNavigationEdge HealthCareDistrict { get; set; }
        
        /// <summary>
        /// Link to the electoral district to which the municipality belongs.
        /// </summary>
        public VmClsNavigationEdge ElectoralDistrict { get; set; }
        
        /// <summary>
        /// Link to the related magistrate service unit.
        /// </summary>
        public VmClsNavigationEdge MagistrateServiceUnit { get; set; }
        
        /// <summary>
        /// Link to the related business service subregion.
        /// </summary>
        public VmClsNavigationEdge BusinessServiceSubRegion { get; set; }

        /// <inheritdoc />
        public VmStatusType Status { get; set; }
    }
}