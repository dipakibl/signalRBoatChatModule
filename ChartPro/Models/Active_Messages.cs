using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartPro.Models
{
    public class Active_Messages
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Messages { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddDate { get; set; }

    }
}
