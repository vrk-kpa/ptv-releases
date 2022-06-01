using System;

namespace PTV.Next.Model
{
    public class AdditionalInfoLinkModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string AdditionalInformation { get; set; }
        public int? OrderNumber { get; set; }
    }
}