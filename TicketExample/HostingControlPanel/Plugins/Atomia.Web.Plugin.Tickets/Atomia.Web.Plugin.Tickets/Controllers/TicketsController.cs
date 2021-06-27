using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.HCP.Authorization;
using Atomia.Web.Plugin.HCP.Authorization.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.Controllers;
using System.Web.Mvc;
using Atomia.Web.Plugin.ServiceReferences;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Atomia.Web.Plugin.Example.Models;
using System.Collections.Generic;
using System;
using System.Web.Script.Serialization;
using Atomia.Web.Plugin.Example.Helpers;

namespace Atomia.Web.Plugin.Example.Controllers
{
    [HCPHandleError(Order = 1)]
    [AtomiaServiceChannel(Order = 2)]
    [HCPInitialization(Order = 3)]
    [Internationalization(Order = 4)]
    public class TicketsController : MainController
    {
        [AcceptVerbs(System.Net.WebRequestMethods.Http.Get, System.Net.WebRequestMethods.Http.Post)]
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(System.Net.WebRequestMethods.Http.Get, System.Net.WebRequestMethods.Http.Post)]
        public ActionResult ViewTicket(string ticketId)
        {
            ViewData["ticket"] = TicketHelper.getTicket(Int32.Parse(ticketId));
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult AddTicket(string subject, string message, bool emergency)
        {
            bool result = addTicket(subject,message,emergency);

            if (result)
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, Errors = "Empty inputs" });
            }
            
        }

        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult AddReply(string reply)
        {
            bool result = addReply(reply);

            if (result)
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, Errors = "Empty inputs" });
            }
        }

        public bool addTicket(string subject, string message, bool emergency)
        {

            if (!String.IsNullOrEmpty(subject) && !String.IsNullOrEmpty(message))
            {

                int id = TicketsList.Instance.Count + 1;

                TicketModel ticket = new TicketModel();
                ticket.Ticketid = "10001";
                ticket.HiddenId = id;
                ticket.Subject = subject;
                ticket.Message = message;
                ticket.Status = "Open";
                ticket.Emergency = emergency;

                TicketsList.Instance.Add(ticket);
                return true;            
            }
            else
            {
                return false;
            }
        }

        private bool addReply(string message)
        {

            if (!String.IsNullOrEmpty(message))
            {

                int id = RepliesList.Instance.Count + 1;

                ReplyModel reply = new ReplyModel();
                reply.From = "Andjela";
                reply.Message = message;
                RepliesList.Instance.Add(reply);
                return true;
            }
            else
            {
                return false;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Search(string sSearch, string iDisplayStart, string iDisplayLength, string sEcho, string iSortCol_0, string sSortDir_0, string iFilter)
        {

            var ticketData = new List<string[]>();

            List<TicketModel> lista = new List<TicketModel>();

            if (TicketsList.Instance.Count < 1)
            {
                TicketHelper.createTickets();
            }

           
            lista = TicketHelper.searchForResult(lista, sSearch);

            int br = 0;
            int i = Int32.Parse(iDisplayStart);
            while (i < lista.Count)
            {
                br++;
                var data = new string[6];
                data[0] = lista[i].Ticketid;
                data[1] = lista[i].Date.ToString();
                data[2] = lista[i].Subject;
                data[3] = lista[i].Status;
                data[4] = lista[i].Date.ToString();
                data[5] = this.Url.Action("ViewTicket", new { area = "Tickets", controller = "Tickets", ticketId = lista[i].HiddenId.ToString() });
                ticketData.Add(data);

                i++;

                if (br == Int32.Parse(iDisplayLength))
                {
                    break;
                }
            }      
    
            var dataToReturn = new
            {
                sEcho,
                iTotalRecords = lista.Count,
                iTotalDisplayRecords = lista.Count,
                aaData = ticketData.ToArray()
            };

            return Json(dataToReturn, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SearchReplies(string sSearch, string iDisplayStart, string iDisplayLength, string sEcho, string iSortCol_0, string sSortDir_0, string iFilter)
        {
            var replyData = new List<string[]>();

            List<ReplyModel> lista = new List<ReplyModel>();

            if (RepliesList.Instance.Count < 1)
            {
                TicketHelper.createReplies();
            }


            lista = TicketHelper.searchForResultReply(lista, sSearch);

            int br = 0;
            int i = Int32.Parse(iDisplayStart);
            while (i < lista.Count)
            {
                br++;
                var data = new string[4];
                data[0] = lista[i].From;
                data[1] = lista[i].Date.ToString();
                data[2] = lista[i].Message;
                data[3] = lista[i].Attachement;
                replyData.Add(data);

                i++;

                if (br == Int32.Parse(iDisplayLength))
                {
                    break;
                }
            }
            var dataToReturn = new
            {
                sEcho,
                iTotalRecords = lista.Count,
                iTotalDisplayRecords = lista.Count,
                aaData = replyData.ToArray()
            };

            return Json(dataToReturn, JsonRequestBehavior.AllowGet);
        }

    }
}
