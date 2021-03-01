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

using System;

namespace PTV.Domain.Model.Models.OpenApi.Extensions
{
    /// <summary>
    /// Constants for Open api validation
    /// </summary>
    public class ValidationConsts
    {
        /// <summary>
        /// Default regex for checking of business code.
        /// </summary>
        public const string BusinessCode = @"^[0-9]{7}-[0-9]{1}$";

        /// <summary>
        /// Default regex for checking of oid.
        /// </summary>
        public const string Oid = @"^[A-Za-z0-9.-]*$";

        /// <summary>
        /// Default regex for checking of area code.
        /// </summary>
        public const string AreaCode = @"^[0-9]{1,6}$";

        /// <summary>
        /// Default regex for checking of external source id.
        /// </summary>
        public const string ExternalSource = @"^[A-Za-z0-9-.]*$";

        /// <summary>
        /// Default regex for checking of ontology term.
        /// </summary>
        public const string OntologyTerm = @"^http://www.yso.fi/onto/[a-z]*/p[0-9]{1,5}$";

        /// <summary>
        /// Default regex for checking of target group.
        /// </summary>
        public const string TargetGroup = @"^http://urn.fi/URN:NBN:fi:au:ptvl:v[0-9]+$";

        /// <summary>
        /// Default regex for checking of service class.
        /// </summary>
        public const string ServiceClass = @"^http://urn.fi/URN:NBN:fi:au:ptvl:v[0-9]+$";

        /// <summary>
        /// Default regex for checking of life event.
        /// </summary>
        public const string LifeEvent = @"^http://urn.fi/URN:NBN:fi:au:ptvl:v[0-9]+$";

        /// <summary>
        /// Default regex for checking of industrial class.
        /// </summary>
        public const string IndustrialClass = @"^http://www.stat.fi/meta/luokitukset/toimiala/001-2008/[0-9]{5}$";

        /// <summary>
        /// Default regex for checking of language.
        /// </summary>
        public const string Language = @"^[a-z-]*$";

        /// <summary>
        /// Default regex for checking of municipality code.
        /// </summary>
        public const string Municipality = @"^[0-9]{1,3}$";

        /// <summary>
        /// Default regex for checking of postal code.
        /// </summary>
        public const string PostalCode = @"\d{5}?";

        /// <summary>
        /// Default regex for checking of country.
        /// </summary>
        public const string Country = @"^[A-Z]{2}$";

        /// <summary>
        /// Default regex for checking of coordinate.
        /// </summary>
        public const string Coordinate = @"^\d+\.?\d*$";

        /// <summary>
        /// Default regex for checking of time.
        /// Only quaters of an hour are allowed, e.g. 14:55 is not valid but 14:45 is.
        /// </summary>
        public const string Time = @"^([0-1][0-9]|[2][0-3]):(00|15|30|45)(:00)?$";
    }
}
