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
namespace PTV.Domain.Model.Enums
{
    /// <summary>
    /// Publishing status of data.
    /// </summary>
    public enum PublishingStatus
    {
        /// <summary>
        /// Draft
        /// </summary>
        Draft=0,
        /// <summary>
        /// Published
        /// </summary>
        Published=1,
        /// <summary>
        /// Deleted
        /// </summary>
        Deleted=2,
        /// <summary>
        /// Modified
        /// </summary>
        Modified=3,
        /// <summary>
        /// Older published
        /// </summary>
        OldPublished=4,
        /// <summary>
        /// Content removed by request and not available anymore
        /// </summary>
        Removed=5
    }
}
