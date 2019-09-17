﻿/**
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model interface of filtered tree for search
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmGetFilteredTree" />
    public class VmGetFilteredTree : IVmGetFilteredTree
    {
        /// <summary>
        /// Id of the Tree
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Value to be searched
        /// </summary>
        public string SearchValue { get; set; }
        /// <summary>
        /// Type of the Tree - ServiceClass, OntologyTerm, LifeEvent, Organization, IndustrialClass, DigitalAuthorization, AnnotationOntologyTerm
        /// </summary>
        [EnumDataType(typeof(TreeTypeEnum))]
        public string TreeType { get; set; }
        /// <summary>
        /// LanguageCode - fi, sv, en
        /// </summary>        
        public List<string> Languages { get; set; }
    }
}
