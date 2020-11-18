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

using PTV.TaskScheduler.Interfaces;

namespace PTV.TaskScheduler.Configuration
{
    // public class DatabaseBackupConfiguration : IJobDataConfiguration<DatabaseBackupConfiguration>
    public class DatabaseBackupConfiguration : IJobDataConfiguration<DatabaseBackupConfiguration>, IApplicationJobConfiguration
    {
        public string ConfigurationName => nameof(DatabaseBackupConfiguration);
        
        public string Folder { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
        
        public string DatabaseConnection { get; set; }
        
        public bool Equals(DatabaseBackupConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return string.Equals(Folder, other.Folder)
                && string.Equals(Command, other.Command)
                && string.Equals(Arguments, other.Arguments)
                && string.Equals(DatabaseConnection, other.DatabaseConnection);
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(Folder)
                   && !string.IsNullOrEmpty(Command)
                   && !string.IsNullOrEmpty(Arguments)
                   && !string.IsNullOrEmpty(DatabaseConnection);
        }
    }
}
