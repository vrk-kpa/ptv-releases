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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace PTV.TaskScheduler.Models
{
    /// <summary>
    /// Model for PalvelunTuottajat json 
    /// </summary>
    public class VmJsonJarjestajat
    {
        public string id { get; set; }
        public string oid { get; set; }
        public string voimassaoloAlkuPvm { get; set; }
        public string voimassaoloLoppuPvm { get; set; }
        public string soteOid { get; set; }
        public string wwwOsoite { get; set; }
        public VmJsonYhteysHenkilo yhteysHenkilo { get; set; }
        
        [JsonIgnore]
        public string laskutustiedot { get; set; }
        
        public List<VmJsonKunnat> kunnat  { get; set; }
        public VmJsonOrganisaatio organisaatio  { get; set; }
    }

    /// <summary>
    /// Model for Organisaatio json 
    /// </summary>
    public class VmJsonOrganisaatio
    {
        public string oid { get; set; }
        public string nimi { get; set; }
        public string pitkaNimi { get; set; }
        public string lyhenne { get; set; }
        public string katuosoite { get; set; }
        public string postiosoite { get; set; }
        public string postinumero { get; set; }
        public string postitoimipaikka { get; set; }
        public string puhelin { get; set; }
        public string alkamispaiva { get; set; }
        public string paattymispaiva { get; set; }
        public string ytunnus { get; set; }
    }

    /// <summary>
    /// Model for Kunnat json 
    /// </summary>
    public class VmJsonKunnat
    {
        public string kuntaNumero { get; set; }
        public string alkuPvm { get; set; }
        public string loppuPvm { get; set; }  
    }

    /// <summary>
    /// Model for YhteysHenkilo json
    /// </summary>
    public class VmJsonYhteysHenkilo
    {
        public string nimi { get; set; }
        public string email { get; set; }
        public string puhelinNumero { get; set; }
    }
}
