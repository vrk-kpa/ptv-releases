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
using System.Collections.Generic;
using System.Linq;
using PTV.Framework;

namespace PTV.Domain.Model.Models.PostiStreets
{
    /// <summary>
    /// 
    /// </summary>
    public class AlandItem : IStreetImportItem
    {
        private const int StreetNameIndex = 0;
        private const int StreetNumberRangeIndex = 1;
        private const int MunicipalityNameIndex = 2;
        private const int PostalCodeIndex = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="csvLine"></param>
        /// <param name="municipalityNameCodes"></param>
        /// <returns></returns>
        public static AlandItem TryParse(string csvLine, Dictionary<string, string> municipalityNameCodes)
        {
            try
            {
                return new AlandItem(csvLine, municipalityNameCodes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="csvLine"></param>
        /// <param name="municipalityNameCodes"></param>
        private AlandItem(string csvLine, Dictionary<string, string> municipalityNameCodes)
        {
            var parts = csvLine.Split(";");
            MunicipalityCode = ParseMunicipalityCode(parts[MunicipalityNameIndex], municipalityNameCodes);
            PostalCode = parts[PostalCodeIndex];
            StreetNameFi = parts[StreetNameIndex];
            StreetNameSv = parts[StreetNameIndex];

            var streetNumberRange = parts[StreetNumberRangeIndex];
            StartNumber = streetNumberRange.TakeWhileWithRest(char.IsDigit, out var rest).ParseToInt() ?? 0;
            rest = new string(rest.SkipWhile(x => !char.IsDigit(x)).ToArray());
            EndNumber = rest.TakeWhileWithRest(char.IsDigit, out rest).ParseToInt() ?? 0;
        }

        private string ParseMunicipalityCode(string name, Dictionary<string, string> municipalityNameCodes)
        {
            return municipalityNameCodes.TryGetOrDefault(name);
        }

        /// <inheritdoc />
        public string MunicipalityCode { get; set; }
        /// <inheritdoc />
        public int StartNumber { get; set; }
        /// <inheritdoc />
        public string StartCharacter { get; set; }
        /// <inheritdoc />
        public int EndNumber { get; set; }
        /// <inheritdoc />
        public string EndCharacter { get; set; }
        /// <inheritdoc />
        public bool? IsEven => StartNumber == 0 ? null : StartNumber.IsEven() as bool?;
        /// <inheritdoc />
        public string StreetNameFi { get; set; }
        /// <inheritdoc />
        public string StreetNameSv { get; set; }
        /// <inheritdoc />
        public string PostalCode { get; set; }
        /// <inheritdoc />
        public Guid StreetId { get; set; }
        /// <inheritdoc />
        public Guid StreetNumberId { get; set; }
    }
}
