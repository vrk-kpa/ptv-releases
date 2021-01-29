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
// ReSharper disable InconsistentNaming

namespace PTV.Framework.Enums
{
    [Flags]
    public enum AccessRightEnum
    {
        OpenApiRead = 0, // default, if not set, read access to open api allowed only (GET methods on open api are "free")
        UiAppRead = 1,
        UiAppWrite = 2,
        OpenApiWrite = 4,
        ASTIWrite = 8
/* SOTE has been disabled (SFIPTV-1177)
        SOTEWrite = 16
*/
    }

    public enum UserAccessRightsGroupEnum
    {
        DENY,
        PTV_VIEWER,
        PTV_USER,
        PTV_MAIN_USER,
        PTV_API_MAIN_USER,
        API_USER,
        API_ASTI_USER,
        PTV_ADMINISTRATOR
/* SOTE has been disabled (SFIPTV-1177)
 API_SOTE_USER
*/
    }


    public class AccessRightWrapper
    {
        public AccessRightEnum AccessRight { get; set; }

        public AccessRightWrapper(AccessRightEnum accessRight)
        {
            this.AccessRight = accessRight;
        }
    }
}
