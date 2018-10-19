namespace PTV.Domain.Model.Enums.Security
{
    /// <summary>
    /// Enum of errors set by authorization proces
    /// </summary>
    public enum TokenErrorEnum
    {
        /// <summary>
        /// Wrong credentials
        /// </summary>
        WrongCredentials,
        /// <summary>
        /// User has not ser organization
        /// </summary>
        MissingOrganization,
        /// <summary>
        /// User has not set any role group
        /// </summary>
        UserNotMapped,
        /// <summary>
        /// No organization set or role group
        /// </summary>
        NoOrgOrGroup
    }
}