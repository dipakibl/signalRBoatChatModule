using System;
using System.Threading.Tasks;
using ChartPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace SignalRSimpleChat
{
    public class Chat : Hub
    {
        private readonly ChatDbContext _context;
        public Chat(ChatDbContext context)
        {
            _context = context;
        }
        public async Task Send( string message ,string UserName)
        {
            MessageHistory messageHistory = new MessageHistory
            {
                Message = message,
                UserName = "You",
                MessageDate = DateTime.Now,
                UserId = UserName
            };

            _context.MessageHistories.Add(messageHistory);
            _context.SaveChanges();
            await Clients.All.SendAsync("Send",  message);
           
        }
    }
}
