using System;
using System.Collections.Generic;

namespace PTV.Domain.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class StsJsonUser
    {
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid Organization { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Role { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<string> Rights { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserOrgRoleMappingData
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid OrganizationId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid RoleId { get; set; }
    }
}