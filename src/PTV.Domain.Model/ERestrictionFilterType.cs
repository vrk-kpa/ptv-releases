namespace PTV.Domain.Model
{
    /// <summary>
    /// 
    /// </summary>
    public enum EVmRestrictionFilterType
    {
        /// <summary>
        /// Filter allows types for attached organization
        /// </summary>
        Allowed = 0,
        
        /// <summary>
        /// Filter denies types for attached organization
        /// </summary>
        Denied = 1,
        
        /// <summary>
        /// Type is allowed but only for reading data containig it, not for assigning it to new data
        /// </summary>
        ReadOnly = 2
    }
}