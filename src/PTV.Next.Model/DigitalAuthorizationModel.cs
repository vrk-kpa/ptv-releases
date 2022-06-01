using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class DigitalAuthorizationModel : CodeBaseModel
    {
        public List<DigitalAuthorizationModel> Children { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsValid { get; set; }
    }
}