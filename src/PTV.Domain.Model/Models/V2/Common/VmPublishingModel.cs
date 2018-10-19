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