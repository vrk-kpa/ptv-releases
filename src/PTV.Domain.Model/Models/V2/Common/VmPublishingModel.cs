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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    ///  View model for requesting publishing of entity
    /// </summary>
    public class VmPublishingModel : IVmLocalizedEntityModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmArchivingModel"/> class.
        /// </summary>
        public VmPublishingModel()
        {
            LanguagesAvailabilities =  new List<VmLanguageAvailabilityInfo>();
        }
        
        /// <summary>
        /// List of languages and their statuses of published entity
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Identifier of entity that should be published
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PublishActionTypeEnum? PublishAction { get; set; }
    }
    
    /// <summary>
    ///  View model for requesting archiving of entity
    /// </summary>
    public class VmArchivingModel : IVmLocalizedEntityModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmArchivingModel"/> class.
        /// </summary>
        public VmArchivingModel()
        {
            LanguagesAvailabilities =  new List<VmLanguageAvailabilityInfo>();
        }
        
        /// <summary>
        /// List of languages and their statuses of published entity
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Identifier of entity that should be archived
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PublishActionTypeEnum? PublishAction { get; set; }
    }
    
    /// <summary>
    ///  View model for requesting copying of entity
    /// </summary>
    public class VmCopyingModel : IVmLocalizedEntityModel, IVmRootBasedEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmCopyingModel"/> class.
        /// </summary>
        public VmCopyingModel()
        {
            LanguagesAvailabilities =  new List<VmLanguageAvailabilityInfo>();
        }
        
        /// <summary>
        /// List of languages and their statuses of published entity
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Identifier of entity that should be copied
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Publish action
        /// </summary>
        public PublishActionTypeEnum? PublishAction { get; set; }

        /// <summary>
        /// Identifier of root node.
        /// </summary>
        public Guid UnificRootId { get; set; }
    }
}
