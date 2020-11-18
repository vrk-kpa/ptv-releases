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

using System.Collections.Generic;
using System.Linq;
using PTV.TaskScheduler.Interfaces;
using PTV.Framework;

namespace PTV.TaskScheduler.Configuration
{
    public class GeoServerJobConfiguration : IJobDataConfiguration<GeoServerJobConfiguration>
    {

        /// <summary>
        /// List of materialized views for refresh
        /// </summary>
        public List<string> MaterializedViewNames { get; set; }

        /// <summary>
        /// SQL command for refreshing of materialized views
        /// </summary>
        public string MaterializedViewRefreshCommand { get; set; }

        /// <summary>
        /// List of SQL procedures for refresh
        /// </summary>
        public List<string> UpdateScripts { get; set; }

        public bool Equals(GeoServerJobConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (MaterializedViewNames == null && other.MaterializedViewNames != null) return false;
            if (UpdateScripts == null && other.UpdateScripts != null) return false;

            return string.Equals(MaterializedViewRefreshCommand, other.MaterializedViewRefreshCommand)
                && MaterializedViewNames.SequenceEqual(other.MaterializedViewNames)
                && UpdateScripts.SequenceEqual(other.UpdateScripts);
        }

        public bool Validate()
        {
            return !MaterializedViewRefreshCommand.IsNullOrEmpty() && MaterializedViewNames != null;
        }
    }
}
