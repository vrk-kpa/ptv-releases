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

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - interface for item with timed publishing properties
    /// </summary>
    public interface IOpenApiTimedPublishing : IOpenApiEntity
    {
        /// <summary>
        /// Date when item should be published.
        /// </summary>
        DateTime? ValidFrom { get; set; }
        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        DateTime? ValidTo { get; set; }
    }

    /// <summary>
    /// OPEN API - interface for item with required properties available languages
    /// </summary>
    public interface IOpenApiEntity: IOpenApiEntityVersion
    {
        /// <summary>
        /// Internal property to check the languages within required lists.
        /// </summary>
        IList<string> RequiredPropertiesAvailableLanguages { get; set; }
    }

    /// <summary>
    /// OPEN API - interface for item with available languages properties
    /// </summary>
    public interface IOpenApiEntityVersion : IOpenApiPublishing
    {
        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        IList<string> AvailableLanguages { get; set; }
    }

    /// <summary>
    /// OPEN API - interface for item with publishing property
    /// </summary>
    public interface IOpenApiPublishing
    {
        /// <summary>
        /// Publishing status.
        /// </summary>
        string PublishingStatus { get; set; }
    }
}
