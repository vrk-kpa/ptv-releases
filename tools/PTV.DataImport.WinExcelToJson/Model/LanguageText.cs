using System.CodeDom;
using System.Collections.Generic;

namespace PTV.DataImport.WinExcelToJson.Model
{
    internal class LanguageText
    {
        public LanguageText(string label, string lang)
        {
            Label = label;
            Lang = lang;
        }

        public LanguageText()
        {
        }

        public string Label { get; set; }
        public string Lang { get; set; }
    }

    internal static class LanguageTextExtensions
    {
        public static void AddText(this List<LanguageText> list, string text, string language)
        {
            if (!string.IsNullOrEmpty(text))
            {
                list.Add(new LanguageText { Label = text, Lang = language});
            }
        }
    }
}