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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class VmOrganizationBrokenLink : IVmBase, IVmSearchBase
    {
        /// <summary>
        /// 
        /// </summary>
        public VmOrganizationBrokenLink()
        {
            Content = new List<VmOrganizationBrokenLinkDetail>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ValidationDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBroken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsException { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ExceptionComment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<VmOrganizationBrokenLinkDetail> Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxPageCount { get; set; } = 10;
        /// <summary>
        /// 
        /// </summary>
        public int Skip { get; set; }
        /// <summary>                
        ///                          
        /// </summary>               
        public int Count { get; set; }
        /// <summary>                   
        ///                             
        /// </summary>                  
        public bool MoreAvailable { get; set; }  
    }
}
