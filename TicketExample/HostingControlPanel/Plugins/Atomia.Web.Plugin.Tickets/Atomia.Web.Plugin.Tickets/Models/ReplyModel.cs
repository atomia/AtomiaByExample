using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomia.Web.Plugin.Example.Models
{
    public class ReplyModel
    {
        public int HiddenId { get; set; }

        public string Message { get; set; }

        public string From { get; set; }

        public string Attachement { get; set; }

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
