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

namespace PTV.Domain.Model.Enums
{
    /// <summary>
    /// Defines the most recent entity update.
    /// </summary>
    [Flags]
    public enum LastOperationType
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,
        /// <summary>
        /// Affected by Eeva role. Mutually exclusive with Pete and Shirley.
        /// </summary>
        Eeva = 1,
        /// <summary>
        /// Affected by Pete role. Mutually exclusive with Eeva and Shirley.
        /// </summary>
        Pete = 2,
        /// <summary>
        /// Affected by Shirley role. Mutually exclusive with Pete and Eeva.
        /// </summary>
        Shirley = 4,
        /// <summary>
        /// Entity was added. Mutually exclusive with Update and Remove.
        /// </summary>
        Create = 8,
        /// <summary>
        /// Entity was updated. Mutually exclusive with Create and Remove.
        /// </summary>
        Update = 16,
        /// <summary>
        /// Entity was removed. Mutually exclusive with Update and Create.
        /// </summary>
        Remove = 32,
        /// <summary>
        /// Entity has publishing status set to draft, restored, reverted or modified.
        /// </summary>
        Edited = 64,
        /// <summary>
        /// Entity has publishing status set to published.
        /// </summary>
        Published = 128,
        /// <summary>
        /// Entity has publishing status set to deleted or old published.
        /// </summary>
        Archived = 256,
        /// <summary>
        /// Entity has publishing status set to permanently removed.
        /// </summary>
        PermanentlyDeleted = 512,
        /// <summary>
        /// A composed state of Published | Eeva
        /// </summary>
        PublishedByEeva = Published | Eeva,
        
    }
}
