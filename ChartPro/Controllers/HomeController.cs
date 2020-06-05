using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChartPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace ChartPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatDbContext _context;
        public HomeController(ILogger<HomeController> logger, ChatDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Get IP Address
            IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = heserver.AddressList.ToList().Where(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault().ToString();
            //Set Session in IP Address
            HttpContext.Session.SetString("UserName", ipAddress);

            // Truncate Table in Next Day
            TrancateHistoryActiveMessage();
            //Add All Messages In Active Chat
            List<Active_Messages> active_message = _context.Active_Messages.Where(a => a.UserName == ipAddress).ToList();
            if (active_message.Count == 0)
            {
                List<DefaultMessage> defaultMessages = _context.DefaultMessages.ToList();
                foreach (var item in defaultMessages)
                {
                    Active_Messages active_ = new Active_Messages();
                    active_.UserName = ipAddress;
                    active_.Messages = item.Message;
                    active_.IsActive = true;
                    active_.AddDate = DateTime.Now;
                    _context.Active_Messages.Add(active_);
                    _context.SaveChanges();
                }
            }
            return View();
        }
        public JsonResult GetHistoryMessages()
        {
            string username = HttpContext.Session.GetString("UserName");
            List<MessageHistory> messages = _context.MessageHistories.Where(a => a.UserId == username).ToList();
            return Json(messages);
        }
        public JsonResult DefaultMsg()
        {
            string username = HttpContext.Session.GetString("UserName");
            var data = "";
            List<Active_Messages> messages = _context.Active_Messages.ToList();
            if (messages.Count != 0)
            {
                List<Active_Messages> allmessage = messages.Where(a => a.UserName == username && a.IsActive == true).ToList();
                if (allmessage.Count != 0)
                {
                    var firstData = allmessage.First();
                    if (firstData != null)
                    {
                        firstData.IsActive = false;
                        data = firstData.Messages;
                        _context.Active_Messages.Update(firstData);
                        MessageHistory messageHistory = new MessageHistory
                        {
                            Message = data,
                            UserName = username,
                            MessageDate = DateTime.Now,
                            UserId = username
                        };

                        _context.MessageHistories.Add(messageHistory);
                        _context.SaveChanges();
                        var lastdata = allmessage[allmessage.Count - 1];
                    }
                }
                else
                {
                    data = "Typing....";
                }
            }
            return Json(data);
        }

        public void TrancateHistoryActiveMessage()
        {
            var history = _context.MessageHistories.ToList();
            var active = _context.Active_Messages.ToList();
            DateTime nowdate = DateTime.Now;
            if (history.Count != 0)
            {
                var messageHistory = history.First();
                if (messageHistory.MessageDate.Date != nowdate.Date)
                {
                    var itemsToDelete = _context.Set<MessageHistory>();
                    _context.MessageHistories.RemoveRange(itemsToDelete);
                    _context.SaveChanges();
                    //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE [MessageHistories]");
                    //_context.SaveChanges();
                }
            }
            if (active.Count != 0)
            {
                var active_Messages = active.First();
                if (active_Messages.AddDate.Date != nowdate.Date)
                {
                    var itemsToDelete = _context.Set<Active_Messages>();
                    _context.Active_Messages.RemoveRange(itemsToDelete);
                    _context.SaveChanges();
                    //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Active_Messages]");
                    //_context.SaveChanges();
                }
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
