using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums.Security;
using System;

namespace PTV.Next.Model
{
    public class OrganizationRoleModel
    {
        public bool IsMain { get; set; }
        public Guid OrganizationId { get; set;}

        [JsonConverter(typeof(StringEnumConverter))]
        public UserRoleEnum Role { get; set;}
    }
}
