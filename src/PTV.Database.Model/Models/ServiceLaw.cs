using PTV.Database.Model.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Database.Model.Models
{
    internal partial class ServiceLaw : EntityBase
    {
        public Guid LawId { get; set; }
        public Guid ServiceVersionedId { get; set; }
        public virtual ServiceVersioned ServiceVersioned { get; set; }
        public virtual Law Law { get; set; }
    }
}
