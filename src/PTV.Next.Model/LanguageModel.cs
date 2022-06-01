namespace PTV.Next.Model
{
    public class LanguageModel : CodeBaseModel
    {
        public bool IsForData { get; set; }
        public bool IsForTranslation { get; set; }
        public int? Order { get; set; }
    }
}