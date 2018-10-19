using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PTV.Application.Web.Controllers
{
    public class SignalRTasks : Hub
    {
        public Task UpdateTasksCount()
        {
            return null;
//            List<String> ConnectionIDToIgnore = new List<String>();
//            ConnectionIDToIgnore.Add(Context.ConnectionId);
//            return Clients.AllExcept(ConnectionIDToIgnore).InvokeAsync("UpdateTasksCount");
        }
    }
}