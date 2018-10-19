using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for sending translation order
    /// </summary>
    public class VmTranslationOrderInput : VmTranslationOrderBase, IVmTranslationOrderInput
    {
        /// <summary>
        /// Constructor of transaltion order
        /// </summary>
        public VmTranslationOrderInput()
        {
            RequiredLanguages = new List<Guid>();
        }
        
        /// <summary>
        /// Identifier of translation company.
        /// </summary>
        public Guid TranslationCompanyId { get; set; }

        /// <summary>
        /// Required languages for translation
        /// </summary>
        public List<Guid> RequiredLanguages { get; set; }

        /// <summary>
        /// Source entity name
        /// </summary>
        public string SourceEntityName { get; set; }

        /// <summary>
        /// Source organization name
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Source organization business code
        /// </summary>
        public string OrganizationBusinessCode { get; set; }
        
        /// <summary>
        /// Source organization identifier
        /// </summary>
        public Guid? OrganizationIdentifier { get; set; }

        /// <summary>
        /// Source language char amount
        /// </summary>
        public long SourceLanguageCharAmount { get; set; }


    }
}