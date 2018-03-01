using System;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{   
    /// <summary>
    /// Translation order data result
    /// </summary>
    public class VmTranslationOrderDataResult
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? TranslationOrderId { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Id 
        /// </summary>
        public Guid? OrderStatusId { get; set; }
    }
}