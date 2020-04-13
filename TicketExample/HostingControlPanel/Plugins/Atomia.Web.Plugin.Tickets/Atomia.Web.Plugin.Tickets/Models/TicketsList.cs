using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomia.Web.Plugin.Example.Models
{
   public class TicketsList
    {
        private TicketsList()
        {
        }
        public static bool sort { get; set; }
        private static List<TicketModel> instance = null;
        public static List<TicketModel> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new List<TicketModel>();
                }
                return instance;
            }
        }
    }
}
