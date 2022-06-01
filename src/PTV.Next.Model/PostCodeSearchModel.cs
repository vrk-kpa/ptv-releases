using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class PostCodeSearchModel
    {
        public string SearchText { get; set; }
        public LanguageEnum Language { get; set; }
    }

    public class PostCodeResult
    {
        public List<PostalCodeModel> items { get; set; } = new List<PostalCodeModel>();
    }
}