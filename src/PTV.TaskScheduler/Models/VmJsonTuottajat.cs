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
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model;

namespace PTV.TaskScheduler.Models
{
    /// <summary>
    /// Model for PalvelunTuottajat json
    /// </summary>
    public class VmJsonTuottajat : VmPalvelunTuottajatBase
    {
        [RegularExpression(DomainConstants.OidParser)]
        public string palveluntuottajaOid { get; set; }

        public string alkamispaiva { get; set; }
        public string paattymispaiva { get; set; }
        public string ytunnus { get; set; }
        public List<VmJsonPalveluyksikot> palveluyksikot { get; set; }
    }

    /// <summary>
    /// Model for Palveluyksikot json
    /// </summary>
    public class VmJsonPalveluyksikot : VmPalvelunTuottajatBaseWithLocation
    {
        [RegularExpression(DomainConstants.OidParser)]
        public string palveluyksikkoOid { get; set; }

        public string alkamispaiva { get; set; }
        public string paattymispaiva { get; set; }
        public List<VmJsonToimipisteet> toimipisteet { get; set; }
    }

    /// <summary>
    /// Model for Toimipisteet json
    /// </summary>
    public class VmJsonToimipisteet : VmPalvelunTuottajatBaseWithLocation
    {
        [RegularExpression(DomainConstants.OidParser)]
        public string toimipisteOid { get; set; }
    }

    /// <summary>
    /// Model for PalvelunTuottajatBase json
    /// </summary>
    public class VmPalvelunTuottajatBase
    {
        public string nimi { get; set; }
        public string puhelin { get; set; }
        public string julkinen { get; set; }
        public string yksityinen { get; set; }
        public string katuosoite { get; set; }
        public string postinumero { get; set; }
        public string postitoimipaikka { get; set; }
    }

    /// <summary>
    /// Model for PalvelunTuottajatBase json
    /// </summary>
    public class VmPalvelunTuottajatBaseWithLocation : VmPalvelunTuottajatBase
    {
       public string sijaintikunta { get; set; }
    }
}
