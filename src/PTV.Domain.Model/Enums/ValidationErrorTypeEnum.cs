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

using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Enums
{
    /// <summary>
    /// Validation error types.
    /// </summary>
    public enum ValidationErrorTypeEnum
    {
        /// <summary>
        /// field is mandatory
        /// </summary>
        MandatoryField,
        /// <summary>
        /// field has to contained published entity
        /// </summary>
        PublishedMandatoryField,
        /// <summary>
        /// field value is same as other one
        /// </summary>
        ValueIsSame,
        /// <summary>
        /// field limit is reached
        /// </summary>
        MaxLimit,
        /// <summary>
        /// field has to contained published organization in content language
        /// </summary>
        PublishedOrganizationLanguageMandatoryField,
        /// <summary>
        /// Field is duplicate with others
        /// </summary>
        IsDuplicate,
        /// <summary>
        /// field value length is less than 5 chars
        /// </summary>
        MinLengthLimit,
        /// <summary>
        /// Connection type is not common and ASTI connection from other organ. exists
        /// </summary>
        PublishedNotCommonAstiConnections,
        /// <summary>
        /// User tried to enter a not-permitted value.
        /// </summary>
        InvalidValue,
        /// <summary>
        /// User tried to specify the same responsible organization twice.
        /// </summary>
        DuplicateOrganization
    }
}
