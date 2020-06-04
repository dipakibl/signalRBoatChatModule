using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartPro.Models
{
    public class ChatDbContext:DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options):base(options)
        {

        }
        public DbSet<DefaultMessage> DefaultMessages { get; set; }
        public DbSet<Active_Messages> Active_Messages { get; set; }
        public DbSet<MessageHistory> MessageHistories { get; set; }
    }
}
