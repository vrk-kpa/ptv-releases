using System;
using System.Collections.Generic;
using System.Text;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Enums;

namespace PTV.Domain.Model.Models.Security
{
    /// <summary>
    /// Model for holding simple info about user access group
    /// </summary>
    public class VmUserAccessRightsGroupSimple
    {
        /// <summary>
        /// Unique identifier of user access group
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Localized name of user access group
        /// </summary>
        public List<VmSimpleName> Name { get; set; }
    }

    /// <summary>
    /// Model for holding simple info about user access group
    /// </summary>
    public class VmUserAccessRightsGroup
    {
        /// <summary>
        /// Unique identifier of user access group
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public UserRoleEnum UserRole { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AccessRightEnum AccessRightFlag { get; set; }
    }


    /// <summary>
    /// Model for holding simple info about name, ie. value and its localization
    /// </summary>
    public class VmSimpleName
    {
        /// <summary>
        /// Text value of name
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Language identifier of localization
        /// </summary>
        public string Localization { get; set; }
    }
}
