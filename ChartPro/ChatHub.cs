using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRSimpleChat
{
    public class Chat : Hub
    {      
        public async Task Send( string message)
        {
            await Clients.All.SendAsync("Send",  message);
        }
    }
}
