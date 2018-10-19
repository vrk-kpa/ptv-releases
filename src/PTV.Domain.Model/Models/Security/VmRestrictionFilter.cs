using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class VmRestrictedType
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<VmRestrictionFilter> Filters { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class VmRestrictionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string FilterName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public VmRestrictedType RestrictedType { get; set; }
        
        /// <summary>
        /// List of attached organizations
        /// </summary>
        public List<VmOrganizationFilter> OrganizationFilters { get; set; }
        
        /// <summary>
        /// Type of filter, is it allowed for or blocked for
        /// </summary>
        public EVmRestrictionFilterType FilterType { get; set; }
        
        /// <summary>
        /// Filter is related to specific type only and other values of this type should be blocked
        /// </summary>
        public bool BlockOtherTypes { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VmOrganizationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OrganizationId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public VmRestrictionFilter Filter { get; set; }
    }
}