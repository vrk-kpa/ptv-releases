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
using System.Text;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Enums;

namespace PTV.Domain.Model.Models.Security
{
    /// <summary>
    /// Model for holding simple info about user access group
    /// </summary>
    public class VmUserAccessRightsGroupSimple
    {
        /// <summary>
        /// Unique identifier of user access group
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Localized name of user access group
        /// </summary>
        public List<VmSimpleName> Name { get; set; }
    }

    /// <summary>
    /// Model for holding simple info about user access group
    /// </summary>
    public class VmUserAccessRightsGroup
    {
        /// <summary>
        /// Unique identifier of user access group
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        ///
        /// </summary>
        public UserRoleEnum UserRole { get; set; }

        /// <summary>
        ///
        /// </summary>
        public AccessRightEnum AccessRightFlag { get; set; }
    }


    /// <summary>
    /// Model for holding simple info about name, ie. value and its localization
    /// </summary>
    public class VmSimpleName
    {
        /// <summary>
        /// Text value of name
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Language identifier of localization
        /// </summary>
        public string Localization { get; set; }
    }
}
