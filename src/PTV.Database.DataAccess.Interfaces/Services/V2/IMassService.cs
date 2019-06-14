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
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IMassService
    {
        /// <summary>
        /// Publish entities by language availabilities
        /// </summary>
        /// <param name="model"></param>
        /// <param name="result"></param>
        IMessage PublishEntities(VmMassDataModel<VmPublishingModel> model, IServiceResultWrap result);

        /// <summary>
        /// Archive entities by entity ids
        /// </summary>
        /// <param name="model"></param>
        IMessage ArchiveEntities(VmMassDataModel<VmArchivingModel> model);

        /// <summary>
        /// Copying selected entities by ids 
        /// </summary>
        /// <param name="model"></param>
        IMessage CopyEntities(VmMassDataModel<VmCopyingModel> model);


        /// <summary>
        /// Restoring selected entities by ids 
        /// </summary>
        /// <param name="model"></param>
        IMessage RestoreEntities(VmMassDataModel<VmRestoringModel> model);

        /// <summary>
        /// Publish scheduled language versions of entity
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="jobName"></param>
        void PublishScheduledLanguageVersions(DateTime dateTime);

        /// <summary>
        /// Archive scheduled language versions
        /// </summary>
        /// <param name="dateTime">Input for archiving language version.</param>
        /// <param name="jobName"></param>
        void ArchiveScheduledLanguageVersions(DateTime dateTime);
    }
}
