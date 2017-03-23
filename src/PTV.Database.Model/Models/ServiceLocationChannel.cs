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
using PTV.Database.Model.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Database.Model.Models
{
    internal class ServiceLocationChannel : ChannelBase
    {
        public ServiceLocationChannel()
        {
            ServiceAreas = new HashSet<ServiceLocationChannelServiceArea>();
            Addresses = new HashSet<ServiceLocationChannelAddress>();
        }

        /// <summary>
        /// Is the service location restricted to a municipality (if yes the ServiceAreas property should contain list of municipalities).
        /// </summary>
        public virtual bool ServiceAreaRestricted { get; set; }

        /// <summary>
        /// Service location latitude.
        /// </summary>
        public virtual string Latitude { get; set; }
        /// <summary>
        /// Service location longitude.
        /// </summary>
        public virtual string Longitude { get; set; }
        /// <summary>
        /// Service location coordinate system used for latitude and longitude.
        /// </summary>
        public virtual string CoordinateSystem { get; set; }
        /// <summary>
        /// Are the coordinates set manually. true if the user has inputed the values otherwise false.
        /// </summary>
        public virtual bool CoordinatesSetManually { get; set; }

        /// <summary>
        /// Does it costs something to call to Phone number (is there some charge). Default is false.
        /// </summary>
        public virtual bool PhoneServiceCharge { get; set; }

        public virtual ICollection<ServiceLocationChannelServiceArea> ServiceAreas { get; set; }

        public virtual ICollection<ServiceLocationChannelAddress> Addresses { get; set; }
    }
}
