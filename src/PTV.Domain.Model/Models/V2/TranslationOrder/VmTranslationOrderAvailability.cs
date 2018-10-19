namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    /// Model for setting data of translation orders
    /// </summary>
    public class VmTranslationOrderAvailability
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// IsTranslationDelivered.
        /// </value>
        public bool IsTranslationDelivered { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// IsInTranslation.
        /// </value>
        public bool IsInTranslation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// CanBeTranslated.
        /// </value>
        public bool CanBeTranslated { get; set; }
    }
}
