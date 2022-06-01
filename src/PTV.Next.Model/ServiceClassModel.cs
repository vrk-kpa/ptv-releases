using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class ServiceClassModel : CodeBaseModel
    {
        public List<Guid> ChildrenIds { get; set; }
        public Guid? ParentId { get; set; }
    }
}