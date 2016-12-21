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

using System;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces
{
    public interface IVmServiceStep1 : IVmEntityBase, IEnumCollection, IVmLocalized
    {
        string ServiceName { get; set; }
        string AlternateServiceName { get; set; }
        string ShortDescriptions { get; set; }
        string Description { get; set; }
        string UserInstruction { get; set; }
        string ServiceUsage { get; set; }
        string AdditionalInformation { get; set; }
        string AdditionalInformationTasks { get; set; }
        string AdditionalInformationDeadLine { get; set; }
        string AdditionalInformationProcessingTime { get; set; }
        string AdditionalInformationValidityTime { get; set; }
        Guid ServiceCoverageTypeId { get; set; }
        List<Guid> Languages { get; set; }
        List<Guid> Municipalities { get; set; }
    }
}
