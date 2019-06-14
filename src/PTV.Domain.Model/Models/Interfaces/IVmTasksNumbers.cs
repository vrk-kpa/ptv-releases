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

using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model for tasks number
    /// </summary>
    public interface IVmTasksNumbers : IVmBase
    {
        /// <summary>
        /// Count of outdated draft services
        /// </summary>
        int OutdatedDraftServicesCount { get; set; }
        /// <summary>
        /// Visibility of postpone button for draft services
        /// </summary>
        bool IsPostponeButtonDSVisible { get; set; }
        /// <summary>
        /// Count of outdated published services
        /// </summary>
        int OutdatedPublishedServicesCount { get; set; }

        /// <summary>
        /// Count of outdated draft channels
        /// </summary>
        int OutdatedDraftChannelsCount { get; set; }

        /// <summary>
        /// Count of outdated published channels
        /// </summary>
        int OutdatedPublishedChannelsCount { get; set; }

        /// <summary>
        /// Count of services without connected channels
        /// </summary>
        int ServicesWithoutChannelsCount { get; set; }

        /// <summary>
        /// Count of channels without connected services
        /// </summary>
        int ChannelsWithoutServicesCount { get; set; }
    }
}