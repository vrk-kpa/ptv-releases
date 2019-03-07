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
using System.Text;

namespace PTV.Domain.Model.Models.Configuration
{
    /// <summary>
    /// Database configuration for maintenance restore, dump, etc.
    /// </summary>
    public class MaintenanceDatabaseConfiguration
    {
        /// <summary>
        /// CommandsPath
        /// </summary>
        public string CommandsPath { get; set; }
        /// <summary>
        /// DumpName
        /// </summary>
        public string DumpName { get; set; }
        /// <summary>
        /// DumpSchema
        /// </summary>
        public string DumpSchema { get; set; }
        /// <summary>
        /// Database
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// Host
        /// </summary> 
        public string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Source folder Name
        /// </summary>
        public string RestoreDumpFolderName { get; set; }
        /// <summary>
        /// Target folder Name
        /// </summary>
        public string BackupDumpFolderName { get; set; }

        /// <summary>
        /// ConfigurationName
        /// </summary>
        public virtual string ConfigurationName => this.GetType().Name;
    }
}
