using System.Collections.Generic;

namespace PTV.DataImport.WinExcelToJson.Model
{
    internal class Law
    {
        public Law()
        {
            Links = new List<LanguageLabel>();
            Names = new List<LanguageLabel>();
        }

        public string LawReference { get; set; }
//        public List<LawDetail> Details { get; set; }
        public List<LanguageLabel> Links { get; set; }
        public List<LanguageLabel> Names { get; set; }
//        public LawDetail LawSv { get; set; }
    }

    internal class GeneralDescriptionJson
    {
        public GeneralDescriptionJson()
        {
            Name = new List<LanguageLabel>();
            ShortDescription = new List<LanguageLabel>();
            Description = new List<LanguageLabel>();
            Laws = new List<Law>();
            ServiceRestrictions = new List<LanguageLabel>();
            UserInstructions = new List<LanguageLabel>();
            ChargeTypeAdditionalInfo = new List<LanguageLabel>();
            DeadLineAdditionalInfo = new List<LanguageLabel>();
            ProcessingTimeAdditionalInfo = new List<LanguageLabel>();
            ValidityTimeAdditionalInfo = new List<LanguageLabel>();
            TargetGroup = new List<KeyValuePair<string, string>>();
            ServiceClass = new List<KeyValuePair<string, string>>();
            OntologyTerm = new List<KeyValuePair<string, string>>();
            LifeEvent = new List<KeyValuePair<string, string>>();
            IndustrialClass = new List<KeyValuePair<string, string>>();
        }

        public string ReferenceCode { get; set; }
        public string ServiceType { get; set; }
        public List<LanguageLabel> Name { get; set; }
        public List<LanguageLabel> ShortDescription { get; set; }
        public List<LanguageLabel> Description { get; set; }
        public List<Law> Laws { get; set; }
        public List<LanguageLabel> ServiceRestrictions { get; set; }
        public List<LanguageLabel> UserInstructions { get; set; }
        public string ChargeType { get; set; }
        public List<LanguageLabel> ChargeTypeAdditionalInfo { get; set; }
        public List<LanguageLabel> DeadLineAdditionalInfo { get; set; }
        public List<LanguageLabel> ProcessingTimeAdditionalInfo { get; set; }
        public List<LanguageLabel> ValidityTimeAdditionalInfo { get; set; }
        public List<KeyValuePair<string, string>> TargetGroup { get; set; }
        public List<KeyValuePair<string, string>> ServiceClass { get; set; }
        public List<KeyValuePair<string, string>> OntologyTerm { get; set; }
        public List<KeyValuePair<string, string>> LifeEvent { get; set; }
        public List<KeyValuePair<string, string>> IndustrialClass { get; set; }
    }
}