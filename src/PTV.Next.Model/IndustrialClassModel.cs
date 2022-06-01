using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class IndustrialClassModel : CodeBaseModel
    {
        public Guid? ParentId { get; set; }
        public List<Guid> ChildrenIds { get; set; }
    }
}