using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChartPro.Models;

namespace ChartPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatDbContext _context;
        public HomeController(ILogger<HomeController> logger,ChatDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
      
        public JsonResult DefaultMsg()

        {
            var data = "";
         List<DefaultMessage> messages = _context.DefaultMessages.ToList();
            if (messages.Count!=0)
            {
                List<DefaultMessage> allmessage = messages.Where(a => a.IsActive == false).ToList();
                if (allmessage.Count != 0)
                {
                  var firstData =  allmessage.First();
                    if (firstData != null)
                    {
                        firstData.IsActive = true;
                        data = firstData.Message;
                        _context.DefaultMessages.Update(firstData);
                        _context.SaveChanges();
                        var lastdata = allmessage[allmessage.Count - 1];
                        //if (lastdata == firstData)
                        //{
                        //    //foreach (var item in messages)
                        //    //{
                        //    //     item.IsActive = false;
                        //    //    _context.DefaultMessages.Update(item);
                        //    //    _context.SaveChanges();
                        //    //}
                        //    data = "Typing....";
                        //}
                    }
                }
                else
                {
                    //foreach (var item in messages)
                    //{
                    //     item.IsActive = false;
                    //    _context.DefaultMessages.Update(item);
                    //    _context.SaveChanges();
                    //}
                    data = "Typing....";
                }
            }
            return Json(data);
        }

        public JsonResult SetMsgStatus()
        {
            var data = "";
            List<DefaultMessage> messages = _context.DefaultMessages.ToList();
           
                    foreach (var item in messages)
                    {
                        item.IsActive = false;
                        _context.DefaultMessages.Update(item);
                        _context.SaveChanges();
                    }
                    data = "Done....";
              
            return Json(data);
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
