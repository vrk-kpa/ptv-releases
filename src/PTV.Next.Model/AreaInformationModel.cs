using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class AreaInformationModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AreaInformationTypeEnum AreaInformationType { get; set; }
        [JsonProperty("areaTypes", ItemConverterType=typeof(StringEnumConverter))]
        public List<AreaTypeEnum> AreaTypes { get; set; }
        public List<Guid> BusinessRegions { get; set; }
        public List<Guid> HospitalRegions { get; set; }
        public List<Guid> Municipalities { get; set; }
        public List<Guid> Provinces { get; set; }
    }
}