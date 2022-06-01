using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IServiceChannelConnectionCommands
    {
        void SaveConnection(ConnectionModel model);
    }
}
