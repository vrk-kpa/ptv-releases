using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class ServiceVoucherModel
    {
        public string Info { get; set; }
        public List<AdditionalInfoLinkModel> Links { get; set; }
    }
}