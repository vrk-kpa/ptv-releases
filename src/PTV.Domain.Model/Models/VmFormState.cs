using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// VM for formSate
    /// </summary>
    public class VmFormState
    {
        /// <summary>
        /// Id of saved form state
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name of saved form
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// EntityType of saved form state
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// EntityId of saved form state
        /// </summary>
        public string EntityId { get; set; }
        /// <summary>
        /// LanguageId of saved form state
        /// </summary>
        public Guid LanguageId { get; set; }
        /// <summary>
        /// UserId of save form state
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Form state
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Flag indication that form exists in db
        /// </summary>
        public bool Exists { get; set; } = true;
    }
}
