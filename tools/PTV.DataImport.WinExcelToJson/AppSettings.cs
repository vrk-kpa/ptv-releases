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

        internal static string OutputResourcesDir
        {
            get
            {
                string od = ConfigurationManager.AppSettings["outputResourcesDir"];

                return string.IsNullOrWhiteSpace(od) ? AppSettings.DefaultRelativePath : od;
            }
        }
    }
}
