using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartPro.Models
{
    public class MessageHistory
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime MessageDate { get; set; }
        public string UserId { get; set; }
    }
}
