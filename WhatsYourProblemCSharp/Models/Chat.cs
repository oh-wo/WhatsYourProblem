using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;


namespace WhatsYourProblemCSharp
{
    [HubName("chatHub")]
    public class LetsChat : Hub
    {
        public void send(string name, string message,string groupName)
        {
            Clients.Group(groupName).addMessage(name, message);
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }


    }
}