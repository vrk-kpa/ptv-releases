using System.Collections.Generic;

namespace PTV.DataImport.WinExcelToJson.Model
{
    internal class Province
    {
        public Province()
        {
            Municipalities = new List<string>();
        }

        public string Code { get; set; }
        public List<LanguageName> Names { get; set; }
        public List<string> Municipalities { get; set; }
    }
}