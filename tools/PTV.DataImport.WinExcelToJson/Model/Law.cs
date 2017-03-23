using System.Collections.Generic;

namespace PTV.DataImport.WinExcelToJson.Model
{
    internal class Law
    {
        public Law()
        {
            Links = new List<LanguageText>();
            Names = new List<LanguageText>();
        }

        public string LawReference { get; set; }
//        public List<LawDetail> Details { get; set; }
        public List<LanguageText> Links { get; set; }
        public List<LanguageText> Names { get; set; }
//        public LawDetail LawSv { get; set; }
    }

    internal class GeneralDescriptionJson
    {
        public GeneralDescriptionJson()
        {
            Name = new List<LanguageText>();
            ShortDescription = new List<LanguageText>();
            Description = new List<LanguageText>();
            Laws = new List<Law>();
            ServiceRestrictions = new List<LanguageText>();
            UserInstructions = new List<LanguageText>();
            ChargeTypeAdditionalInfo = new List<LanguageText>();
            DeadLineAdditionalInfo = new List<LanguageText>();
            ProcessingTimeAdditionalInfo = new List<LanguageText>();
            ValidityTimeAdditionalInfo = new List<LanguageText>();
            TargetGroup = new List<KeyValuePair<string, string>>();
            ServiceClass = new List<KeyValuePair<string, string>>();
            OntologyTerm = new List<KeyValuePair<string, string>>();
            LifeEvent = new List<KeyValuePair<string, string>>();
            IndustrialClass = new List<KeyValuePair<string, string>>();
        }

        public string ReferenceCode { get; set; }
        public string ServiceType { get; set; }
        public List<LanguageText> Name { get; set; }
        public List<LanguageText> ShortDescription { get; set; }
        public List<LanguageText> Description { get; set; }
        public List<Law> Laws { get; set; }
        public List<LanguageText> ServiceRestrictions { get; set; }
        public List<LanguageText> UserInstructions { get; set; }
        public string ChargeType { get; set; }
        public List<LanguageText> ChargeTypeAdditionalInfo { get; set; }
        public List<LanguageText> DeadLineAdditionalInfo { get; set; }
        public List<LanguageText> ProcessingTimeAdditionalInfo { get; set; }
        public List<LanguageText> ValidityTimeAdditionalInfo { get; set; }
        public List<KeyValuePair<string, string>> TargetGroup { get; set; }
        public List<KeyValuePair<string, string>> ServiceClass { get; set; }
        public List<KeyValuePair<string, string>> OntologyTerm { get; set; }
        public List<KeyValuePair<string, string>> LifeEvent { get; set; }
        public List<KeyValuePair<string, string>> IndustrialClass { get; set; }
    }
}