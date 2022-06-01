using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class OntologyTermModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Dictionary<LanguageEnum, string> Names { get; set; } = new Dictionary<LanguageEnum, string>();
    }
}