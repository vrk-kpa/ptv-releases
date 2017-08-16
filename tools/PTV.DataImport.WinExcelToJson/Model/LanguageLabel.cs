using System.CodeDom;
using System.Collections.Generic;

namespace PTV.DataImport.WinExcelToJson.Model
{
    internal class LanguageLabel
    {
        public LanguageLabel(string label, string lang)
        {
            Label = label;
            Lang = lang;
        }

        public LanguageLabel()
        {
        }

        public string Label { get; set; }
        public string Lang { get; set; }
    }

    internal class LanguageName
    {
        public LanguageName(string name, string language)
        {
            Name = name;
            Language = language;
        }

        public LanguageName()
        {
        }

        public string Name { get; set; }
        public string Language { get; set; }
    }

    internal static class LanguageTextExtensions
    {
        public static void AddText(this List<LanguageLabel> list, string text, string language)
        {
            if (!string.IsNullOrEmpty(text))
            {
                list.Add(new LanguageLabel { Label = text, Lang = language});
            }
        }
    }
}