using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class MunicipalityModel : CodeBaseModel
    {
        public List<PostalCodeModel> PostalCodes { get; set;} = new List<PostalCodeModel>();
    }
}