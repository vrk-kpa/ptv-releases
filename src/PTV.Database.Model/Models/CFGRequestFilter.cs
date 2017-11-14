using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.Model.Models
{
    public class CFGRequestFilter
    {
        public Guid Id { get; set; }
        public string Interface { get; set; }
        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public int ConcurrentRequests { get; set; }

    }
}
