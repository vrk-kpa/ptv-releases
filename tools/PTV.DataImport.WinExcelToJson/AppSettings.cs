using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.DataImport.WinExcelToJson
{
    internal static class AppSettings
    {
        // just assume we always run the app inside visualstudio and generate to the PTV.DataImport.ConsoleApp Generated folder.
        private const string DefaultRelativePath = "..\\..\\..\\PTV.DataImport.Console\\Generated";

        internal static string OutputDir
        {
            get
            {
                string od = ConfigurationManager.AppSettings["outputDir"];

                return string.IsNullOrWhiteSpace(od) ? AppSettings.DefaultRelativePath : od;
            }
        }
    }
}
