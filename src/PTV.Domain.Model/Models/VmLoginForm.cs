using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model for login form
    /// </summary>
    public class VmLoginForm
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// UserAccessRightsGroup
        /// </summary>
        public Guid UserAccessRightsGroup { get; set; }

        /// <summary>
        /// Organization
        /// </summary>
        public Guid Organization { get; set; }

        /// <summary>
        /// ActiveOrganizationId
        /// </summary>
        public Guid ActiveOrganizationId { get; set; }

        /// <summary>
        /// PahaId
        /// </summary>
        public Guid PahaId { get; set; }

        /// <summary>
        /// Roles from paha
        /// </summary>
        public Dictionary<string, IList<string>> Roles { get; set; }
    }
}
