using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TDDSandbox.SignalRServer
{
    //[HubName("chatHub")]
    public class ChatHub: Hub
    {
        public void NewChatMessage(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}