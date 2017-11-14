namespace PTV.DataImport.WinExcelToJson.Tool
{
    /// <summary>
    /// Contains ExcelReader settings.
    /// </summary>
    public class ExcelReaderSettings
    {
        /// <summary>
        /// Connection provider (application assumes OLE DB but this is here just to be able to easily try some other version).
        /// </summary>
        public string Provider { get; set; } = "Microsoft.ACE.OLEDB.12.0";

        /// <summary>
        /// Excel file (path).
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Does the first column contain column names (default: true).
        /// </summary>
        public bool FirstRowContainsColumnNames { get; set; } = true;

        /// <summary>
        /// Should the values from columns be read intermixed (default: true).
        /// </summary>
        public bool ReadIntermixed { get; set; } = true;

        /// <summary>
        /// Excel version.
        /// </summary>
        public string ExcelVersion { get; set; } = "Excel 12.0";

        /// <summary>
        /// How many rows should be skipped from the start.
        /// </summary>
        public int SkipRows { get; set; } = 0;

        /// <summary>
        /// Creates the connection string to Excel file.
        /// </summary>
        /// <returns>connection string</returns>
        public string CreateConnectionString()
        {
            string hdr = FirstRowContainsColumnNames ? "YES" : "NO";
            string imex = ReadIntermixed ? "1" : "0";

            return $"Provider={Provider};Data Source={File};Extended Properties=\"{ExcelVersion};HDR={hdr};IMEX={imex}\"";
        }
    }
}
