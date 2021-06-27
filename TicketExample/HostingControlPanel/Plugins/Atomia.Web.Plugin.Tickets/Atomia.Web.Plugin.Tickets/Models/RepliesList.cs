using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomia.Web.Plugin.Example.Models
{
    class RepliesList
    {
        private RepliesList()
        {
        }
        public static bool sort { get; set; }
        private static List<ReplyModel> instance = null;
        public static List<ReplyModel> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new List<ReplyModel>();
                }
                return instance;
            }
        }
    }
}
