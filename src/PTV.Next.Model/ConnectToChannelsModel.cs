using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class ConnectToChannelsModel
    {
        public Guid ServiceId { get; set; }
        public List<Guid> ServiceChannelUnificRootIds { get; set; } = new List<Guid>();
    }
}
