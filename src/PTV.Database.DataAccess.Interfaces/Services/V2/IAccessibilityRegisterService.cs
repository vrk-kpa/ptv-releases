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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IAccessibilityRegisterService
    {
        /// <summary>
        /// Set AccessibilityRegister for ServiceChannel
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="model"></param>
        VmAccessibilityRegisterSetOut SetAccessibilityRegister(IUnitOfWorkWritable unitOfWork, VmAccessibilityRegisterSetIn model);

        /// <summary>
        /// Load AccessibilityRegister for ServiceChannel
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="accessibilityRegisterId"></param>
        void LoadAccessibilityRegister(IUnitOfWorkWritable unitOfWork, Guid accessibilityRegisterId);

        /// <summary>
        /// Delete AccessibilityRegister for ServiceChannel
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="accessibilityRegisterId"></param>
        VmServiceLocationChannel DeleteAccessibilityRegister(IUnitOfWorkWritable unitOfWork, Guid accessibilityRegisterId);

        /// <summary>
        /// Handle Accessibility Register when chanel is saving
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="vm"></param>
        void HandleAccessibilityRegisterSave(IUnitOfWorkWritable unitOfWork, VmServiceLocationChannel vm);
    }
}