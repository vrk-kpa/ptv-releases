using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public abstract class CodeBaseModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Dictionary<LanguageEnum, string> Names { get; set; } = new Dictionary<LanguageEnum, string>();
        public Dictionary<LanguageEnum, string> Descriptions { get; set; } = new Dictionary<LanguageEnum, string>();
    }
}