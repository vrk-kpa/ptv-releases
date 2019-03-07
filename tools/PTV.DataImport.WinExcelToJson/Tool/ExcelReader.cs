/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace PTV.DataImport.WinExcelToJson.Tool
{
    /// <summary>
    /// Reads data from Excel file.
    /// </summary>
    public class ExcelReader
    {
        private const string OleDbSchemaTableName = "TABLE_NAME";

        private ExcelReaderSettings _settings;

        /// <summary>
        /// Creates a new instance with supplied settings.
        /// </summary>
        /// <param name="settings"></param>
        public ExcelReader(ExcelReaderSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            // NOTE! the settings doesn't have read only state currently so it is possible to change the settings from outside
            _settings = settings;
        }

        /// <summary>
        /// Gets the Excel sheet names.
        /// </summary>
        /// <returns>Exel sheet names</returns>
        public List<string> GetSheetNames()
        {
            List<string> sheetNames = new List<string>(5);

            using (OleDbConnection conn = new OleDbConnection(_settings.CreateConnectionString()))
            {
                conn.Open();

                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                foreach (DataRow dr in schema.Rows)
                {
                    object tblName = dr[ExcelReader.OleDbSchemaTableName];

                    if (tblName != null)
                    {
                        sheetNames.Add(tblName.ToString());
                    }
                }
            }

            return sheetNames;
        }

        /// <summary>
        /// Reads the Excel sheet and calls the supplied row handler to process the row data.
        /// </summary>
        /// <param name="sheetName">Excel sheet name which is read</param>
        /// <param name="rowHandler">delegate to handle the reading of a row, return object from delegate that contains the desired row data. If the returned object is null it will not be added to the list.</param>
        /// <returns>List of objects</returns>
        public List<object> ReadSheet(string sheetName, Func<DbDataReader, object> rowHandler)
        {
            if (rowHandler == null)
            {
                throw new ArgumentNullException(nameof(rowHandler));
            }

            List<object> rowData = new List<object>(50);

            using (OleDbConnection excelConn = new OleDbConnection(_settings.CreateConnectionString()))
            {
                var cmd = excelConn.CreateCommand();
                cmd.CommandText = $"select * from [{sheetName}]";
                cmd.CommandType = CommandType.Text;

                excelConn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        var i = 0;

                        while (reader.Read())
                        {
                            if (i < _settings.SkipRows)
                            {
                                i += 1;
                                continue;
                            }

                            var o = rowHandler(reader);

                            if (o != null)
                            {
                                rowData.Add(o);
                            }
                        }
                    }
                }
            }

            return rowData;
        }
    }
}
