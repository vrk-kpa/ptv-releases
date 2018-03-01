using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.V2.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class VmConnectionsServiceSearch : IVmMultiLocalized, IVmSearchParamsBase
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Fulltext { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<LanguageCode> Languages { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ServiceTypeId { get; set; }
        /// <summary>
        /// List if sorting params.
        /// </summary>
        public List<VmSortParam> SortData { get; set; }
          /// <summary>
        /// Id of entity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxPageCount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        public List<Guid> SelectedPublishingStatuses { get; set; }
        /// <summary>
        /// Gets or sets the selected service classes.
        /// </summary>
        public List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the selected ontology terms.
        /// </summary>
        public List<Guid> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the selected area information types.
        /// </summary>
        public List<Guid> AreaInformationTypes { get; set; }
        /// <summary>
        /// Gets or sets the selected target groups.
        /// </summary>
        public List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the selected life events.
        /// </summary>
        public List<Guid> LifeEvents { get; set; }
        /// <summary>
        /// Gets or sets the selected industrial classes.
        /// </summary>
        public List<Guid> IndustrialClasses { get; set; }        
    }
}
