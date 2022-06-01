using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class StreetSearchModel
    {
        public string SearchText { get; set; }
        public string PostalCode { get; set; }
        public LanguageEnum Language {get; set; }
        public int Offset { get; set; }
        public bool OnlyValid { get; set; } = true;
    }

    public class StreetSearchResult
    {
        public int MaxPageCount { get; set; }
        public int PageNumber { get; set; }
        public bool MoreAvailable { get; set; }
        public int Skip { get; set; }
        public List<StreetModel> Items { get; set; } = new List<StreetModel>();
    }

    public class StreetModel
    {
        public Guid? Id { get; set; }
        public Dictionary<LanguageEnum, string> Names { get; set; } = new Dictionary<LanguageEnum, string>();
        public string MunicipalityCode { get; set; }
        public bool IsValid { get; set; }
        public List<StreetNumberModel> StreetNumbers { get; set; } = new List<StreetNumberModel>();
    }
    
    public class StreetNumberModel
    {
        public Guid Id { get; set; }
        public Guid StreetId { get; set; }
        public int StartNumber { get; set; }
        public int EndNumber { get; set; }
        public bool IsEven { get; set; }
        public string PostalCode { get; set; }
        public bool IsValid { get; set; }        
    }

    public class StreetNameSearchModel
    {
        public string SearchText { get; set; }
        public bool OnlyValid { get; set; } = true;
    }
    
    public class StreetNameResult
    {
        public List<Dictionary<LanguageEnum, string>> Items { get; set; } = new List<Dictionary<LanguageEnum, string>>();
    }

    public class StreetNameModel
    {
        public Dictionary<LanguageEnum, string> Names { get; set; } = new Dictionary<LanguageEnum, string>();
    }

    public class PostalCodeResult
    {
        public List<PostalCodeModel> Items { get; set; }
    }
}
