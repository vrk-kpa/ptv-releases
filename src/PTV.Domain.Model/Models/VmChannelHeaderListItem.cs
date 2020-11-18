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

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// Model for item in header table for channel form
    /// </summary>
    public class VmChannelHeaderListItem
    {
        /// <summary>
        /// Modified by user name
        /// </summary>
        public string ModifiedByName { get; set; }
        /// <summary>
        /// Modified by user email
        /// </summary>
        public string ModifiedByEmail { get; set; }
        /// <summary>
        /// Modified at
        /// </summary>
        public DateTime ModifiedAt { get; set; }
        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Commented by
        /// </summary>
        public string CommentedByName { get; set; }
        /// <summary>
        /// Commented at
        /// </summary>
        public DateTime CommentedAt { get; set; }
    }
}
