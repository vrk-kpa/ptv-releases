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

/*
1. Record ID 1 x (5)
2. Driving Date 6 x (8) vvvvpppp
3. Postcode 14 x (5)
4. Postcode name in Finnish 19 x (30)
5. Postal code name in Swedish 49 x (30)
6. Abbreviation of the postal code in Finnish 79 x (12)
7. Abbreviation of the postal code name in Swedish 91 x (12)
8. Street (place) name in Finnish 103 x (30)
9. Street (place) name in Swedish 133 x (30)
10. Empty 163 x (12)
11. Empty 175 x (12)
12. Property type 187 x (1) 1 = odd
2 = even
13. Minimum Property Number (Details of an Even / Odd Property)
14. Property number 1 188 x (5)
15. Property Letter 1 193 x (1)
16. Punctuation 194 x (1)
17. Property number 2 195 x (5)
18. Property Letter 2 200 x (1)
19. Largest Property Number (Dual / Odd Property Details)
20. Property number 1 201 x (5)
21. Property Letter 1 206 x (1)
22. Punctuation 207 x (1)
23. Property Number 2 208 x (5)
24. Property Letter 2 213 x (1)
25. Municipal Code 214 x (3)
26. Name of the municipality in Finnish 217 x (20)
27. Name of the municipality in Swedish 237 x (20)
 */

using System;

namespace PTV.Domain.Model.Models.PostiStreets
{
    /// <summary>
    ///
    /// </summary>
    public class PostiStreetLine : IStreetImportItem
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="line"></param>
        public PostiStreetLine(string line)
        {
            PostalCode = line.Substring(13, 5).Trim();
            StreetNameFi = line.Substring(102, 30).Trim();
            StreetNameSv = line.Substring(132, 30).Trim();
            IsEven = ConvertEven(line.Substring(186, 1).Trim());
            if (int.TryParse(line.Substring(187, 5).Trim(), out var startNumber))
            {
                StartNumberRaw = startNumber;
            }

            StartCharacter = line.Substring(192, 1).Trim();
            StartNumberPunctuation = line.Substring(193, 1).Trim();

            if (int.TryParse(line.Substring(194, 5).Trim(), out var startNumberDual))
            {
                StartNumberDual = startNumberDual;
            }

            StartCharacterDual = line.Substring(199, 1).Trim();
            //

            if (int.TryParse(line.Substring(200, 5).Trim(), out var endNumber))
            {
                EndNumberRaw = endNumber;
            }

            EndCharacter = line.Substring(205, 1).Trim();
            EndNumberPunctuation = line.Substring(206, 1).Trim();

            if (int.TryParse(line.Substring(207, 5).Trim(), out var endNumberDual))
            {
                EndNumberDual = endNumberDual;
            }

            EndCharacterDual = line.Substring(212, 1).Trim();
            MunicipalityCode = line.Substring(213, 3).Trim();
            MunicipalityFi = line.Substring(216, 20).Trim();
            MunicipalitySv = line.Substring(236, 20).Trim();
        }

        /// <summary>
        ///
        /// </summary>
        public string MunicipalitySv { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string MunicipalityFi { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string MunicipalityCode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string EndCharacterDual { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? EndNumberDual { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string EndCharacter { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string EndNumberPunctuation { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? EndNumberRaw { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string StartCharacterDual { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? StartNumberDual { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string StartNumberPunctuation { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string StartCharacter { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? StartNumberRaw { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool? IsEven { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string StreetNameSv { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string StreetNameFi { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int StartNumber => Math.Min(StartNumberRaw ?? int.MaxValue, StartNumberDual ?? int.MaxValue);

        /// <summary>
        ///
        /// </summary>
        public int EndNumber => Math.Max(EndNumberRaw ?? 0, EndNumberDual ?? 0);

        /// <summary>
        ///
        /// </summary>
        public Guid StreetId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Guid StreetNumberId { get; set; }

        private bool? ConvertEven(string substring)
        {
            switch (substring)
            {
                case "2":
                    return true;
                case "1":
                    return false;
                default:
                    return null;
            }
        }

    }
}
