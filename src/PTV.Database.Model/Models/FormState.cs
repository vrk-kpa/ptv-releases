using PTV.Database.Model.Models.Base;
using System;

namespace PTV.Database.Model.Models
{
    internal class FormState : EntityIdentifierBase
    {
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public Guid LanguageId { get; set; }
        public string UserName { get; set; }
        public string State { get; set; }
        public string FormName { get; set; }
    }
}
