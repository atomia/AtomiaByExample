using Atomia.Web.Base.Validation.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomia.Web.Plugin.Example.Models
{
    public class TicketModel
    {
        public string Ticketid { get; set; }

        public int HiddenId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool Emergency { get; set; }

        public string Status { get; set; }

        private DateTime date;
        public DateTime Date
        {
            get
            {
                if (date != default(DateTime))
                {
                    return date;
                }

                date = DateTime.Now;
                return date;
            }
            set
            {
                date = value;
            }
        }

    }
}
