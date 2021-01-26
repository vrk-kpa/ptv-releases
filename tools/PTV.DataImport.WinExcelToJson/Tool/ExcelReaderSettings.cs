/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
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
